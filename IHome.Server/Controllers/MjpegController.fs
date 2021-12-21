namespace IHome.Server.Controllers

open System
open System.Text
open System.Threading.Channels
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Authorization
open Microsoft.AspNetCore.Http.Features
open Microsoft.Extensions.Logging
open IHome.Server.Services


[<Authorize>]
[<ApiController>]
[<Route("[controller]")>]
type MjpegController(camera: CameraService, logger: ILogger<MjpegController>) as this =
    inherit ControllerBase()


    let createHeader (length: int) =
        let header =
            "--frame\r\n" + "Content-Type:image/jpeg\r\n" + "Content-Length:" + string length + "\r\n\r\n"

        Encoding.ASCII.GetBytes(header)

    let createFooter () = Encoding.ASCII.GetBytes("\r\n")

    let writeBufferBody (reader: ChannelReader<byte []>) =
        task {
            while not this.HttpContext.RequestAborted.IsCancellationRequested do
                let! _ = reader.WaitToReadAsync()

                match reader.TryRead() with
                | true, data ->
                    let! _ = this.HttpContext.Response.BodyWriter.WriteAsync(createHeader (int data.Length))
                    let! _ = this.HttpContext.Response.BodyWriter.WriteAsync(data)
                    let! _ = this.HttpContext.Response.BodyWriter.WriteAsync(createFooter ())
                    ()
                | _ -> ()
        }


    [<HttpGet>]
    member _.Stream() =
        task {
            let id = Guid.NewGuid()
            let channel = camera.GetStreamChannel id

            try
                let bufferingFeature = this.HttpContext.Response.HttpContext.Features.Get<IHttpResponseBodyFeature>()
                bufferingFeature.DisableBuffering()

                this.HttpContext.Response.StatusCode <- 200
                this.HttpContext.Response.ContentType <- "multipart/x-mixed-replace; boundary=--frame"
                this.HttpContext.Response.Headers.Add("Connection", "Keep-Alive")
                this.HttpContext.Response.Headers.Add("CacheControl", "no-cache")

                do! writeBufferBody channel

            finally
                camera.RemoveStreamChannel id
                this.HttpContext.Response.Body.Close()
        }
