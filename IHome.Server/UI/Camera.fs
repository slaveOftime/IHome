[<AutoOpen>]
module IHome.Server.UI.Camera

open Fun.Blazor
open IHome.Server.Services


let camera =
    html.inject (fun (globalStore: IGlobalStore) ->
        let lookUpBtn =
            adaptiview () {
                let! topMotorAngle, setTopMotorAngle = globalStore.UseTopMotorAngle().WithSetter()
                Template.html $"""
                    <sl-icon-button
                        onclick="{fun _ -> setTopMotorAngle(topMotorAngle - 5)}"
                        class="font-bold text-4xl"
                        name="chevron-up">
                    </sl-icon-button>
                """
            }

        let lookBottomBtn =
            adaptiview () {
                let! topMotorAngle, setTopMotorAngle = globalStore.UseTopMotorAngle().WithSetter()
                Template.html $"""
                    <sl-icon-button
                        onclick="{fun _ -> setTopMotorAngle(topMotorAngle + 5)}"
                        class="font-bold text-4xl"
                        name="chevron-down">
                    </sl-icon-button>
                """
            }

        let lookLeftBtn =
            adaptiview () {
                let! bottomMotorAngle, setBottomMotorAngle = globalStore.UseBottomMotorAngle().WithSetter()
                Template.html $"""
                    <sl-icon-button
                        onclick="{fun _ -> setBottomMotorAngle(bottomMotorAngle - 10)}"
                        class="font-bold text-4xl"
                        name="chevron-left">
                    </sl-icon-button>
                """
            }

        let looRightBtn =
            adaptiview () {
                let! bottomMotorAngle, setBottomMotorAngle = globalStore.UseBottomMotorAngle().WithSetter()
                Template.html $"""
                    <sl-icon-button
                        onclick="{fun _ -> setBottomMotorAngle(bottomMotorAngle + 10)}"
                        class="font-bold text-4xl"
                        name="chevron-right">
                    </sl-icon-button>
                """
            }

        Template.html $"""
            <div class="">
                <div class="flex flex-row justify-center">{lookUpBtn}</div>
                <div class="flex flex-row items-center">
                    <div class="row-start-2 col-start-1">{lookLeftBtn}</div>
                    <div class="shadow-lg rounded-md shadow-yellow-400/20 overflow-hidden">
                        <img src="/mjpeg" class="aspect-video w-[400px]">
                    </div>
                    <div class="row-start-2 col-start-3">{looRightBtn}</div>
                </div>
                <div class="flex flex-row justify-center">{lookBottomBtn}</div>
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
