namespace IHome.Server.Services

open System
open System.Device.Gpio
open FSharp.Control.Reactive
open Fun.Blazor


type InfradAvoidService(store: IGlobalStore) =
    let sensorLeft = 16
    let sensorRight = 12

    let gpio = new GpioController()
    
    do
        gpio.OpenPin(sensorLeft, PinMode.Input)
        gpio.OpenPin(sensorRight, PinMode.Input)


        let leftSubject = Subject<bool>.broadcast
        let rightSubject = Subject<bool>.broadcast

        leftSubject
        |> Observable.throttle (TimeSpan.FromMilliseconds 50)
        |> Observable.add (fun _ ->
            store.UseHasObstacleOnLeft().Publish(gpio.Read sensorLeft = PinValue.Low)
        )

        rightSubject
        |> Observable.throttle (TimeSpan.FromMilliseconds 50)
        |> Observable.add (fun _ ->
            store.UseHasObstacleOnRight().Publish(gpio.Read sensorRight = PinValue.Low)
        )


        gpio.RegisterCallbackForPinValueChangedEvent(
            sensorLeft, 
            PinEventTypes.Rising ||| PinEventTypes.Falling ||| PinEventTypes.None,
            PinChangeEventHandler(fun _ e ->
                match e.ChangeType with
                | PinEventTypes.Falling -> leftSubject.OnNext true
                | _  -> leftSubject.OnNext false
            )
        )

        gpio.RegisterCallbackForPinValueChangedEvent(
            sensorRight, 
            PinEventTypes.Rising ||| PinEventTypes.Falling ||| PinEventTypes.None,
            PinChangeEventHandler(fun _ e ->
                match e.ChangeType with
                | PinEventTypes.Falling -> rightSubject.OnNext true
                | _ -> rightSubject.OnNext false
            )
        )
