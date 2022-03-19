[<AutoOpen>]
module IHome.Server.UI.Hooks

open Microsoft.AspNetCore.Components
open Microsoft.Extensions.Configuration
open FSharp.Data.Adaptive
open Fun.Blazor
open Fun.Blazor.Validators


type LoginForm = { Name: string; Password: string }


type IComponentHook with
    member hook.UseLoginForm () =
        hook
            .UseAdaptiveForm<_, _>({ Name = ""; Password = "" })
            .AddValidators(
                (fun x -> x.Name),
                true,
                [
                    minLength 2 (sprintf "Name leng cannot be shorter than %d")
                    maxLength 20 (sprintf "Name leng cannot be longer than %d")
                ]
            )
            .AddValidators(
                (fun x -> x.Password),
                true,
                [
                    required "Password is required"
                ]
            )
