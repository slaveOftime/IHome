namespace IHome.Server.Services

open System
open System.Device.Gpio
open System.Device.Pwm.Drivers
open Fun.Blazor

type Direction =
    | Left
    | Right
    | Forward
    | Back

type WheelService(globalStore: IGlobalStore) as this =
    let PWMA = 18
    let PWMB = 23
    let AIN1 = 22
    let AIN2 = 27
    let BIN1 = 25
    let BIN2 = 24

    let gpio = new GpioController()

    let leftPWM = new SoftwarePwmChannel(PWMA, 100, 0)
    let rightPWM = new SoftwarePwmChannel(PWMB, 100, 0)

    do
        gpio.OpenPin(AIN1, PinMode.Output)
        gpio.OpenPin(AIN2, PinMode.Output)
        gpio.OpenPin(BIN1, PinMode.Output)
        gpio.OpenPin(BIN2, PinMode.Output)
        gpio.OpenPin(PWMA, PinMode.Output)
        gpio.OpenPin(PWMB, PinMode.Output)

        leftPWM.Start()
        rightPWM.Start()

        globalStore.UseHasObstacleOnLeft().AddLazyCallback(function
            | true when not (globalStore.UseIgnoreObstacle().Value) -> this.Stop()
            | _ -> ()
        ) |> ignore

        globalStore.UseHasObstacleOnRight().AddLazyCallback(function
            | true when not (globalStore.UseIgnoreObstacle().Value) -> this.Stop()
            | _ -> ()
        ) |> ignore


    member _.Move(direction, speed) =
        match direction with
        | Left when globalStore.UseIgnoreObstacle().Value || not(globalStore.UseHasObstacleOnLeft().Value) ->
            leftPWM.DutyCycle <- speed
            gpio.Write(AIN1, PinValue.Low)
            gpio.Write(AIN2, PinValue.High)

            rightPWM.DutyCycle <- speed
            gpio.Write(BIN1, PinValue.High)
            gpio.Write(BIN2, PinValue.Low)

        | Right when globalStore.UseIgnoreObstacle().Value || not(globalStore.UseHasObstacleOnRight().Value) ->
            leftPWM.DutyCycle <- speed
            gpio.Write(AIN1, PinValue.High)
            gpio.Write(AIN2, PinValue.Low)

            rightPWM.DutyCycle <- speed
            gpio.Write(BIN1, PinValue.Low)
            gpio.Write(BIN2, PinValue.High)

        | Forward when globalStore.UseIgnoreObstacle().Value || (
                    not(globalStore.UseHasObstacleOnLeft().Value)
                    && not(globalStore.UseHasObstacleOnRight().Value)) ->
            leftPWM.DutyCycle <- speed
            gpio.Write(AIN1, PinValue.High)
            gpio.Write(AIN2, PinValue.Low)

            rightPWM.DutyCycle <- speed
            gpio.Write(BIN1, PinValue.High)
            gpio.Write(BIN2, PinValue.Low)

        | Back ->
            leftPWM.DutyCycle <- speed
            gpio.Write(AIN1, PinValue.Low)
            gpio.Write(AIN2, PinValue.High)

            rightPWM.DutyCycle <- speed
            gpio.Write(BIN1, PinValue.Low)
            gpio.Write(BIN2, PinValue.High)

        | _ ->
            ()


    member _.Stop() =
        leftPWM.DutyCycle <- 0
        gpio.Write(AIN1, PinValue.Low)
        gpio.Write(AIN2, PinValue.Low)

        rightPWM.DutyCycle <- 0
        gpio.Write(BIN1, PinValue.Low)
        gpio.Write(BIN2, PinValue.Low)


    interface IDisposable with
        member _.Dispose() =
            gpio.Dispose()
            leftPWM.Dispose()
            rightPWM.Dispose()
