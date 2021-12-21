[<AutoOpen>]
module IHome.Server.UI.Camera

open Fun.Blazor
open IHome.Server.Services


let camera =
    html.inject (fun (hook: IComponentHook) ->
        Template.html $"""
            <div class="shadow-lg rounded-md shadow-yellow-400/20 overflow-hidden">
                <img src="/mjpeg" class="aspect-video w-[400px]">
            </div>
        """
    )


let takePictureBtn =
    html.inject (fun (camera: CameraService) ->
        Template.html $"""
            <sl-icon-button name="camera2" onmousedown="{ignore >> camera.Capture}" class="text-primary-400 m-10 text-3xl font-extrabold"></sl-icon-button>
        """
    )    
