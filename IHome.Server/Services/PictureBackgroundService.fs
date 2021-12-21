namespace IHome.Server.Services

open System
open System.IO
open System.Threading.Tasks
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Hosting
open Fun.Result


type PictureBackgroundService (logger: ILogger<PictureBackgroundService>, camera: CameraService) =
    inherit BackgroundService()

    let size = 1280, 720
    let delay = 1000 * 60 * 5

    override _.ExecuteAsync token =
        task {
            while not token.IsCancellationRequested do
                try
                    do! camera.Capture size |> Task.map ignore
                with
                    | e -> logger.LogError $"Take picture failed {e.Message}"
                    
                do! Task.Delay delay
        }


type PictureCleanupBackgroundService (logger: ILogger<PictureCleanupBackgroundService>, camera: CameraService) =
    inherit BackgroundService()

    let delay = 1000 * 60 * 60 * 24

    override _.ExecuteAsync token =
        task {
            while not token.IsCancellationRequested do
                logger.LogInformation "Clean pictures ..."

                Directory.EnumerateFiles camera.PicturesFolder
                |> Seq.map FileInfo
                |> Seq.filter (fun f -> (DateTime.Now - f.CreationTime).Days > 7)
                |> Seq.iter (fun f ->
                    try
                        File.Delete f.FullName
                        logger.LogInformation $"Removed picture {f.Name}"
                    with e ->
                        logger.LogError $"Remove picture failed {e.Message}")

                do! Task.Delay delay
        }
