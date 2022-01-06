[<AutoOpen>]
module IHome.Server.UI.Camera

open Fun.Blazor
open IHome.Server.Services
open IHome.Server.UI.Shoelace


let camera =
    html.inject (fun (globalStore: IGlobalStore) ->
        let handleSlChangeEvent fn (e: SlChangeEventArgs) =
            try e.Value |> string |> int |> fn
            with _ -> ()

        let verticalSlider =
            adaptiview () {
                let! topMotorAngle, setTopMotorAngle = globalStore.UseTopMotorAngle().WithSetter()
                Template.html $"""
                    <sl-range
                        min="50" max="180"
                        value="{topMotorAngle}"
                        onsl-change="{callback (handleSlChangeEvent setTopMotorAngle)}"
                        help-text="look up or down"
                    ></sl-range>
                """
            }

        let horizontalSlider =
            adaptiview () {
                let! bottomMotorAngle, setBottomMotorAngle = globalStore.UseBottomMotorAngle().WithSetter()
                Template.html $"""
                    <sl-range
                        min="0" max="180"
                        value="{bottomMotorAngle}"
                        onsl-change="{callback (handleSlChangeEvent setBottomMotorAngle)}"
                        help-text="look left or right"
                    ></sl-range>
                """
            }

        Template.html $"""
            <div>
                <div class="shadow-lg rounded-md shadow-yellow-400/20 overflow-hidden">
                    <img src="/mjpeg" class="aspect-video w-[400px]">
                </div>
                <div class="my-3">{verticalSlider}</div>
                <div class="my-3">{horizontalSlider}</div>           
            </div>
        """
    )


// TODO: Use high resolution

let takePictureBtn =
    html.inject (fun (camera: CameraService) ->
        Template.html $"""
            <sl-icon-button name="camera2" onmousedown="{ignore >> camera.Capture}" class="text-primary-400 m-10 text-3xl font-extrabold"></sl-icon-button>
        """
    )    
