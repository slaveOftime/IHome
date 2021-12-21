namespace IHome.Server.Services

open System
open System.IO
open System.Threading.Tasks
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Hosting
open Iot.Device.CpuTemperature
open Fun.Blazor


type BoardBackgroundService(logger: ILogger<BoardBackgroundService>, store: IGlobalStore) =
    inherit BackgroundService()

    let delay = 1000


    let readCPUTemp () =
        try
            use cpu = new CpuTemperature()

            if cpu.IsAvailable then
                cpu.ReadTemperatures()
                |> Seq.map (fun struct (n, t) ->
                    logger.LogInformation $"Temp {n}: {t.DegreesCelsius} C"
                    t.DegreesCelsius
                )
                |> Seq.tryHead
            else
                logger.LogError "Temperature is not available"
                None
        with
            | e ->
                logger.LogError $"Read CPU Temperature failed {e.Message}"
                None


    override _.ExecuteAsync token =
        task {
            while not token.IsCancellationRequested do
                readCPUTemp () |> Option.iter (store.UseCPUTemperature().Publish)

                do! Task.Delay delay
        }
