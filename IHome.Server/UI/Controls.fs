[<AutoOpen>]
module IHome.Server.UI.Controls

open Microsoft.AspNetCore.Components

[<EventHandler("onsl-change", typeof<ChangeEventArgs>, enableStopPropagation = true, enablePreventDefault = true)>]
[<AbstractClass; Sealed>]
type EventHandlers =
    class
    end


let colorPirmary n = $"var(--sl-color-primary-{n})"
let colorSuccess n = $"var(--sl-color-success-{n})"
let colorWarning n = $"var(--sl-color-warning-{n})"
let colorDanger n = $"var(--sl-color-danger-{n})"
let colorNeutral n = $"var(--sl-color-neutral-{n})"
let colorBlack n = $"var(--sl-color-neutral-0)"
let colorWhite n = $"var(--sl-color-neutral-100)"


let space1 = "var(--sl-spacing-3x-small)"
let space2 = "var(--sl-spacing-2x-small)"
let space3 = "var(--sl-spacing-x-small)"
let space4 = "var(--sl-spacing-small)"
let space5 = "var(--sl-spacing-medium)"
let space6 = "var(--sl-spacing-large)"
let space7 = "var(--sl-spacing-x-large)"
let space8 = "var(--sl-spacing-2x-large)"
let space9 = "var(--sl-spacing-3x-large)"
let space10 = "var(--sl-spacing-4x-large)"
