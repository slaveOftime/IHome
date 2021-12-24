[<AutoOpen>]
module IHome.Server.UI.App

open Microsoft.AspNetCore.Http
open Fun.Blazor
open Fun.Blazor.Router


let app =
    html.inject (fun (ctx: IHttpContextAccessor) ->
        let ctx = ctx.HttpContext

        let main =
            Template.html $"""
                <div class="p-5 h-full w-full grid justify-items-center grid-cols-1 grid-rows-2 sm:grid-cols-2 sm:grid-rows-1 md:grid-cols-2 md:grid-rows-1 overflow-hidden">
                    <div class="row-start-2 col-start-1 sm:row-start-1 sm:col-start-1 md:row-start-1 md:col-start-1 flex flex-col justify-center items-center">
                        <div class="flex flex-row items-center my-5">
                            {board}
                            {logoutBtn}
                        </div>
                        {wheelController}
                    </div>
                    <div class="row-start-1 col-start-1 sm:row-start-1 sm:col-start-2 md:row-start-1 md:col-start-2 flex flex-col justify-center items-center">
                        {camera}
                    </div>
                </div>
            """

        html.route [
            routeCiWithQueries "/login" (fun qs ->
                let error = qs |> Map.tryFind "error" |> Option.map string
                login error
            )
            if ctx.User.Identity.IsAuthenticated then
                routeCi "/pictures-logger" pictureLogger
                routeCi "/" main
            routeAny (login None)
        ]
    )
