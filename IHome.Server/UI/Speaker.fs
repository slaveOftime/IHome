[<AutoOpen>]
module IHome.Server.UI.Speaker

open Fun.Blazor


let speaker =
    html.inject (fun () ->

        Template.html
            $"""
            <div>speaker</div>
        """
    )
