[<AutoOpen>]
module IHome.Server.UI.Controller

open FSharp.Data.Adaptive
open Fun.Blazor
open IHome.Server.Services
open IHome.Server.UI.Shoelace


let wheelController =
    html.inject (fun (wheel: WheelService, globalStore: IGlobalStore) ->
        let speed = cval 0.5

        let increaseSpeed () =
            if speed.Value > 0.9 then speed.Publish 0.1 else speed.Publish((+) 0.1)

        let moveBtn direction =
            Template.html $"""
                <sl-icon-button
                    onmousedown="{fun _ -> wheel.Move(direction, speed.Value)}"
                    onmouseup="{ignore >> wheel.Stop}"
                    ontouchstart="{fun _ -> wheel.Move(direction, speed.Value)}"
                    ontouchend="{ignore >> wheel.Stop}"
                    class="font-bold text-4xl"
                    name="{match direction with
                           | Left -> "chevron-left"
                           | Forward -> "chevron-up"
                           | Right -> "chevron-right"
                           | Back -> "chevron-down"}">
                </sl-icon-button>
            """

        let speedBtn =
            adaptiview () {
                let! speed = speed
                Template.html $"""
                    <div
                        class="h-full w-full rounded-full bg-danger-900 text-center flex flex-col items-center justify-center font-bold"
                        style="opacity: {speed};"
                        onclick="{ignore >> increaseSpeed}"
                    >
                        {int (speed * 10.)}
                    </div>
                """
            }

        let obstacleDetection =
            adaptiview () {
                let! left = globalStore.UseHasObstacleOnLeft()
                let! right = globalStore.UseHasObstacleOnRight()
                let! ignoreObstacle, setIgnoreObstacle = globalStore.UseIgnoreObstacle().WithSetter()
                Template.html $"""
                    <div class="flex flex-row items-center my-8">
                        <div class="w-[30px] h-[30px] rounded-full {if ignoreObstacle then "bg-gray-400" elif left then "bg-danger-600" else "bg-success-600"}"></div>
                        <sl-radio checked="{ignoreObstacle}" onsl-change="{callback (fun (_: SlChangeEventArgs) -> setIgnoreObstacle (not ignoreObstacle))}" class="ml-5 mr-2"></sl-radio>
                        <div class="w-[30px] h-[30px] rounded-full {if ignoreObstacle then "bg-gray-400" elif right then "bg-danger-600" else "bg-success-600"}"></div>
                    </div>
                """
            }

        Template.html $"""
            {obstacleDetection}
            <div class="grid grid-cols-3 grid-rows-3 rounded-full bg-neutral-300 dark:bg-neutral-700 border-success-300 w-[156px] h-[156px] shadow-lg shadow-amber-400/10 touch-none select-none">
                <div class="row-start-2 col-start-1">{moveBtn Left}</div>
                <div class="row-start-1 col-start-2">{moveBtn Forward}</div>
                <div class="row-start-2 col-start-3">{moveBtn Right}</div>
                <div class="row-start-3 col-start-2">{moveBtn Back}</div>
                <div class="row-start-2 col-start-2 w-full h-full">{speedBtn}</div>
            </div>
        """
    )
