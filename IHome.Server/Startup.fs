open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.Extensions.DependencyInjection
open Bolero.Server
open IHome.Server.Services
open IHome.Server.UI


let builder = WebApplication.CreateBuilder(Environment.GetCommandLineArgs())
let services = builder.Services


services.AddControllersWithViews() |> ignore
services.AddHttpContextAccessor() |> ignore
services.AddMemoryCache() |> ignore
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie() |> ignore
services.AddAuthorization() |> ignore
services.AddServerSideBlazor().Services.AddFunBlazorServer() |> ignore
services.AddHostedService<PictureBackgroundService>() |> ignore
services.AddHostedService<PictureCleanupBackgroundService>() |> ignore
services.AddHostedService<BoardBackgroundService>() |> ignore
services.AddSingleton<InfradAvoidService>() |> ignore
services.AddSingleton<ServoMotorService>() |> ignore
services.AddSingleton<GuardService>() |> ignore
services.AddSingleton<WheelService>() |> ignore
services.AddSingleton<CameraService>() |> ignore


let application = builder.Build()

application.Services.GetService<InfradAvoidService>() |> ignore
application.Services.GetService<ServoMotorService>() |> ignore

application.UseStaticFiles() |> ignore
    
application.UseAuthentication().UseAuthorization() |> ignore

application.MapControllers() |> ignore
application.MapBlazorHub() |> ignore
application.MapFunBlazor(Index.page) |> ignore

application.Run()
