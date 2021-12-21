[<AutoOpen>]
module IHome.Server.UI.PictureLogger

open System
open System.Linq
open FSharp.Data.Adaptive
open FSharp.Control.Reactive
open Fun.Result
open Fun.Blazor
open IHome.Server.Services

// TODO: handle a lot of pictures with virtualize

let pictureLogger =
    html.inject (fun (hook: IComponentHook, camera: CameraService) ->
        let pictures = cval [||]

        hook.OnFirstAfterRender.Add (fun () ->
            camera.GetPictureNames() |> Seq.toArray |> pictures.Publish

            Observable.interval (TimeSpan.FromSeconds 10)
            |> Observable.subscribe (fun _ -> camera.GetPictureNames() |> Seq.toArray |> pictures.Publish)
            |> hook.AddDispose
        )

        let list =
            adaptiview () {
                let! pictures = pictures
                for pic in pictures do
                    Template.html $"""
                        <li class="flex flex-col items-center mt-20">
                            <img src="/pictures/{pic.Name}" class="w-[400px] rounded-md shadow-md">
                            <div class="mt-5 text-center dark:text-white font-bold">
                                {pic.CreatedTime.ToString("yy/MM/dd HH:mm:ss")}
                            </div>
                        </li>
                    """
            }

        Template.html $"""
            <div class="sm:mx-10 m-20 flex flex-col items-center h-full overflow-y-auto"">
                <ul class="max-w-[720px]">
                    {list}
                </ul>
            </div>
        """
    )


let pictureLoggerBtn =
    html.inject (fun (hook: IComponentHook, camera: CameraService) ->
        let pictures = cval DeferredState.NotStartYet

        hook.OnFirstAfterRender.Add (fun () ->
            camera.GetPictureNames().Take(5) |> Seq.toArray |> DeferredState.Loaded |> pictures.Publish
        )

        let imageCircle src =
            Template.html $"""
                <img src="{src}" class="rounded-full aspect-square w-[40px] -mr-5 shadow-md">
            """

        let main =
            adaptiview () {
                match! pictures with
                | DeferredState.Loading ->
                    Template.html $"""
                        <sl-progress-bar indeterminate style="width: 120px;"></sl-progress-bar>
                    """
                | DeferredState.Loaded ps ->
                    Template.html $"""
                        <div class="flex flex-row items-center">
                            <sl-button href="/pictures-logger">Pictures</sl-button>
                            <div class="flex flex-row items-center pr-15">
                                {
                                    html.fragment [
                                        for p in ps do
                                            imageCircle $"/pictures/{p.Name}"
                                    ]
                                }
                            </div>
                        </div>
                    """
                | _ ->
                    Template.html $"<div>...</div"
            }

        Template.html $"""
            <div class="m-10">{main}</div>
        """
    )
