namespace IHome.Server.UI

open System
open Microsoft.AspNetCore.Components
open Fun.Blazor


type SlChangeEventArgs() =
    inherit EventArgs()

    member val Value = null with get, set


[<EventHandler("onsl-change", typeof<SlChangeEventArgs>, enableStopPropagation = true, enablePreventDefault = true)>]
[<AbstractClass; Sealed>]
type EventHandlers = class end


module Shoelace =
    let registerEvents =
        js """
            Blazor.registerCustomEventType('sl-change', {
                browserEventName: 'sl-change',
                createEventArgs: event => {
                    return {
                        value: event.target.value
                    };
                }
            });
        """
        