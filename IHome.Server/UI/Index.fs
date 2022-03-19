namespace IHome.Server.UI

open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Components
open Microsoft.AspNetCore.Mvc.Rendering
open Fun.Blazor
open IHome.Server.Services


type LoginPage() =
    inherit FunBlazorComponent()

    override _.Render() = 
        #if DEBUG
        html.hotReloadComp(loginPage, "IHome.Server.UI.Login.loginPage")
        #else
        loginPage
        #endif


type IndexPage() =
    inherit FunBlazorComponent()

    override _.Render() =
        #if DEBUG
        html.hotReloadComp(app, "IHome.Server.UI.App.app")
        #else
        app
        #endif

    static member page (ctx: HttpContext) =
        html.inject (fun (config: IConfiguration) ->
            let title = config.GetValue("Home:Title")
            let v = StaticStore.Version

            let root =
                if ctx.User <> null && ctx.User.Identity.IsAuthenticated then
                    rootComp<IndexPage> ctx RenderMode.ServerPrerendered
                else
                    rootComp<LoginPage> ctx RenderMode.ServerPrerendered

            Template.html $"""
<!DOCTYPE html>
<html class="bg-neutral-50 dark:bg-neutral-1000 h-full w-full overflow-hidden">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <title>{title}</title>
    <base href="/" />
    <link rel="stylesheet" media="(prefers-color-scheme:light)" href="https://unpkg.com/@shoelace-style/shoelace@2.0.0-beta.62/dist/themes/light.css">
    <link rel="stylesheet" media="(prefers-color-scheme:dark)"
        href="https://unpkg.com/@shoelace-style/shoelace@2.0.0-beta.62/dist/themes/dark.css"
        onload="document.documentElement.classList.add('sl-theme-dark');">
    <link href="app-generated.css?v={v}" rel="stylesheet">
    <link href="manifest.json?v={v}" rel="manifest" />
    <link rel="apple-touch-icon?v={v}" sizes="512x512" href="icon-512.png" />
    <link rel="apple-touch-icon?v={v}" sizes="192x192" href="icon-192.png" />
</head>
<body class="h-full w-full overflow-hidden">
    {root}
    <script src="_framework/blazor.server.js"></script>
    <script type="module" src="https://unpkg.com/@shoelace-style/shoelace@2.0.0-beta.62/dist/shoelace.js"></script>
    {Shoelace.registerEvents}
    {
        #if DEBUG
        html.hotReloadJSInterop
        #else
        html.none
        #endif
    }
    <script>
        document.addEventListener('contextmenu', event => event.preventDefault());
        navigator.serviceWorker.register('service-worker.js');
    </script>
</body>
</html>
            """
        )
