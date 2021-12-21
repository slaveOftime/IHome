namespace IHome.Server.Controllers

open System
open System.Security.Claims
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Caching.Memory
open Fun.Result
open IHome.Server.Services


[<CLIMutable>]
type User = { Name: string; Password: string }

[<ApiController>]
[<Route("[controller]")>]
type UserController(guard: GuardService, config: IConfiguration, cache: IMemoryCache, logger: ILogger<UserController>) as this =
    inherit ControllerBase()

    let users =
        cache.GetOrCreate(
            "users",
            fun entry ->
                let data = config.GetSection("Home:Users").Get<User []>()
                entry.SetValue data |> ignore
                data
        )

    [<HttpPost "login">]
    member _.Login([<FromForm>] loginDto: User): Task<IActionResult> =
        task {
            logger.LogInformation $"{loginDto.Name} is trying to login"

            let clientIP = this.HttpContext.Connection.RemoteIpAddress.ToString()
            let retryCount = guard.GetLoginRetryCount clientIP

            if retryCount > 3 then
                return this.Redirect "/login?error=Retry in 5 minutes" :> IActionResult
            else
                let check user =
                    user.Name = loginDto.Name && user.Password + DateTime.Now.ToString("MMdd") = loginDto.Password

                if users |> Seq.exists check |> not then
                    return this.Redirect "/login?error=Your credential is not correct" :> IActionResult
                else
                    let issuer = "ihome"

                    let claims = [ Claim(ClaimTypes.Name, loginDto.Name, ClaimValueTypes.String, issuer) ]
                    let userIdentity = ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
                    let userPrincipal = ClaimsPrincipal(userIdentity)

                    do!
                        this.HttpContext.SignInAsync(
                            userPrincipal,
                            AuthenticationProperties(ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60), IsPersistent = true, AllowRefresh = false)
                        )

                    return this.Redirect "/"  :> IActionResult
        }

    
    [<HttpPost "logout">]
    member _.Logout () =
        task {
            do! this.HttpContext.SignOutAsync()
            return this.Redirect "/"
        }
