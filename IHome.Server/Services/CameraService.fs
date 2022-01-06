namespace IHome.Server.Services

open System
open System.IO
open System.Reflection
open System.Threading
open System.Threading.Channels
open System.Collections.Concurrent
open Microsoft.Extensions.Logging
open Iot.Device.Media
open SixLabors.ImageSharp
open SixLabors.ImageSharp.PixelFormats
open Microsoft.IO
open System.Diagnostics
open System.Buffers

// TODO: enable user to change resolution

[<AutoOpen>]
module Helper =
    let inline clamp v =
        if v > 255.0 then 255.0
        elif v < 0.0 then 0.
        else v

    let inline yuvToRgb y u v =
        let red = clamp (float y + 1.4075 * float (v - 128))
        let green = clamp (float y - 0.3455 * float (u - 128) - 0.7169 * float (v - 128))
        let blue = clamp (float y + 1.779 * float (u - 128))
        Rgb24(byte red, byte green, byte blue)


type CameraService(logger: ILogger<CameraService>) as this =
    let (</>) str1 str2 = Path.Combine(str1, str2)

    let entryFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)
    let pictureTempFolder = entryFolder </> "picture-stream"

    let memoryManager = RecyclableMemoryStreamManager()
    let streamChannels = ConcurrentDictionary<Guid, Channel<byte[]>>()

    let width, height = 160u, 120u
    let settings = VideoConnectionSettings(0, (width, height), PixelFormat.YUYV)
    let device = VideoDevice.Create(settings)
    let mutable tokenSource = new CancellationTokenSource()

    
    let writeToChannel (e: NewImageBufferReadyEventArgs) =
        task {
            use stream = memoryManager.GetStream(e.ImageBuffer.AsMemory().Slice(0, e.Length).ToArray())

            let sw = Stopwatch.StartNew()

            let array = ArrayPool<Rgb24>.Shared.Rent (int stream.Length)
            let mutable index = 0

            while stream.Position <> stream.Length do
                let y = stream.ReadByte()
                let u = stream.ReadByte()
                let y2 = stream.ReadByte()
                let v = stream.ReadByte()
                array[index] <- yuvToRgb y u v
                array[index + 1] <- yuvToRgb y2 u v
                index <- index + 2

            use image = Image.LoadPixelData(array, int width, int height)

            logger.LogDebug $"Process frame in {sw.ElapsedMilliseconds} ms"

            use outStream = memoryManager.GetStream()
            image.SaveAsJpeg(outStream)
            let data = outStream.ToArray()

            for channel in streamChannels.Values do
                do! channel.Writer.WriteAsync data

            ArrayPool.Shared.Return array
            ArrayPool.Shared.Return e.ImageBuffer
        }
        |> ignore


    do
        if Directory.Exists pictureTempFolder |> not then
            Directory.CreateDirectory pictureTempFolder |> ignore

        device.ImageBufferPoolingEnabled <- true
        device.NewImageBufferReady.Add writeToChannel

        for resolution in device.GetPixelFormatResolutions(PixelFormat.YUYV) do
            printfn $"[{resolution.MinWidth}x{resolution.MinHeight}]->[{resolution.MaxWidth}x{resolution.MaxHeight}], Step [{resolution.StepWidth},{resolution.StepHeight}]"


    member _.Size = int width, int height
     
    member _.PicturesFolder = pictureTempFolder

    member _.ImageReady = device.NewImageBufferReady


    member _.GetStreamChannel(id) =
        this.StartCapture()
        streamChannels.GetOrAdd(id, (fun _ -> Channel.CreateBounded 2)).Reader

    member _.RemoveStreamChannel(id: Guid) =
        match streamChannels.TryRemove id with
        | _ -> ()
        if streamChannels.Count = 0 then
            this.StopCapture()


    member _.StopCapture() =
        if device.IsCapturing then
            tokenSource.Cancel()
            tokenSource <- new CancellationTokenSource()
            device.StopCaptureContinuous()

    member _.StartCapture() =
        if not device.IsOpen then device.StartCaptureContinuous()

        if not device.IsCapturing then
            Thread(fun () -> device.CaptureContinuous tokenSource.Token).Start()


    member _.GetPictureNames() =
        Directory.EnumerateFiles pictureTempFolder
        |> Seq.map FileInfo
        |> Seq.map (fun info ->
            {|
                Name = Path.GetFileName info.Name
                CreatedTime = info.CreationTime
            |}
        )
        |> Seq.sortByDescending (fun x -> x.CreatedTime)


    member _.Capture(?size) =
        task {
            use stream = new MemoryStream(device.Capture())
            let colors = VideoDevice.YuyvToRgb(stream)
            let bitmap = VideoDevice.RgbToBitmap(settings.CaptureSize, colors)

            let id = Guid.NewGuid().ToString()
            let file = pictureTempFolder </> id + ".jpg"
            bitmap.Save(file, Drawing.Imaging.ImageFormat.Jpeg)

            logger.LogInformation("Picture is taken")

            return file
        }
