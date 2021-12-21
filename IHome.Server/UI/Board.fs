[<AutoOpen>]
module IHome.Server.UI.Board

open Fun.Blazor
open IHome.Server.Services


let board =
    html.inject (fun (globalStore: IGlobalStore) ->
        adaptiview () {
            let! cpu = globalStore.UseCPUTemperature()
            Template.html $"""
                <div class="m-5">
                    <span class="text-3xl {if cpu > 60 then "text-warning-700" else "text-success-600"} font-extrabold">{int cpu}</span>
                    <span class="opacity-50 dark:text-white/80">℃</span>
                </div>
            """
        }
    )