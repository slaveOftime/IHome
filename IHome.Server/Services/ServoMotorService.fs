namespace IHome.Server.Services

open System.Device.I2c
open Microsoft.Extensions.Logging
open Iot.Device.Pwm
open Iot.Device.ServoMotor
open Fun.Blazor


type ServoMotorService (globalStore: IGlobalStore, logger: ILogger<ServoMotorService>) =
    
    let topPIN = 4
    let bottomPIN = 5

    let settings = I2cConnectionSettings(1, 64)
    let device = I2cDevice.Create settings
    let pca9685 = new Pca9685(device, pwmFrequency = 60)

    let topChannel = pca9685.CreatePwmChannel topPIN
    let bottomChannel = pca9685.CreatePwmChannel bottomPIN
    let topMotor = new ServoMotor(topChannel)
    let bottomMotor = new ServoMotor(bottomChannel)

    do
        topMotor.Start()
        bottomMotor.Start()

        let topAngle = globalStore.UseTopMotorAngle()
        topAngle.AddInstantCallback(fun angle ->
            try
                logger.LogInformation $"Change top motor to {angle}"
                if angle < 50 then topAngle.Publish 50
                elif angle > 180 then topAngle.Publish 180
                else topMotor.WriteAngle angle
            with
                | ex -> logger.LogError $"Change top motor failed {ex.Message}"
        ) |> ignore

        let bottomAngle = globalStore.UseBottomMotorAngle()
        bottomAngle.AddInstantCallback(fun angle ->
            try
                logger.LogInformation $"Change bottom motor to {angle}"
                if angle < 0 then bottomAngle.Publish 0
                elif angle > 180 then bottomAngle.Publish 180
                else bottomMotor.WriteAngle angle
            with
                | ex -> logger.LogError $"Change bottom motor failed {ex.Message}"
        ) |> ignore
