namespace IHome.Server.Controllers

open System.IO
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Authorization
open IHome.Server.Services


[<Authorize>]
[<ApiController>]
[<Route("[controller]")>]
type PicturesController(camera: CameraService) as this =
    inherit ControllerBase()


    [<HttpGet>]
    member _.GetPictures() = camera.GetPictureNames()


    [<HttpGet "{name}">]
    member _.GetPicture(name: string) : IActionResult =
        let file = Path.Combine(camera.PicturesFolder, name)

        if File.Exists file then
            this.PhysicalFile(file, "image/jpeg")
        else
            this.NotFound()
