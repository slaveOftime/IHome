namespace IHome.Server.UI.Shoelace

open System
open Microsoft.AspNetCore.Components


type SlChangeEventArgs() =
    inherit EventArgs()

    member val Value = null with get, set


[<EventHandler("onsl-change", typeof<SlChangeEventArgs>, enableStopPropagation = true, enablePreventDefault = true)>]
[<AbstractClass; Sealed>]
type EventHandlers = class end
