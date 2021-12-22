open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.Extensions.DependencyInjection
open Bolero.Server
open IHome.Server.Services
open IHome.Server.UI


Host
    .CreateDefaultBuilder(Environment.GetCommandLineArgs())
    .ConfigureWebHostDefaults(fun builder ->
        builder
            .ConfigureServices(fun (services: IServiceCollection) ->
                services.AddControllersWithViews() |> ignore
                services.AddHttpContextAccessor() |> ignore
                services.AddMemoryCache() |> ignore
                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie() |> ignore
                services.AddAuthorization() |> ignore
                services.AddServerSideBlazor().Services.AddBoleroHost(true, true).AddFunBlazor() |> ignore
                services.AddHostedService<PictureBackgroundService>() |> ignore
                services.AddHostedService<PictureCleanupBackgroundService>() |> ignore
                services.AddHostedService<BoardBackgroundService>() |> ignore
                services.AddSingleton<InfradAvoidService>() |> ignore
                services.AddSingleton<GuardService>() |> ignore
                services.AddSingleton<WheelService>() |> ignore
                services.AddSingleton<CameraService>() |> ignore
            )
            .Configure(fun (application: IApplicationBuilder) ->
                application.ApplicationServices.GetService<InfradAvoidService>() |> ignore
                application
                    .UseStaticFiles()
                    .UseAuthentication()
                    .UseRouting()
                    .UseAuthorization()
                    .UseEndpoints(fun endpoints ->
                        endpoints.MapControllers() |> ignore
                        endpoints.MapBlazorHub() |> ignore
                        endpoints.MapFallbackToBolero(Index.page) |> ignore
                    )
                |> ignore
            )
        |> ignore
    )
    .Build()
    .Run()
