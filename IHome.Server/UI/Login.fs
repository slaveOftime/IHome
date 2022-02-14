[<AutoOpen>]
module IHome.Server.UI.Login

open Microsoft.AspNetCore.Components
open Microsoft.Extensions.Configuration
open FSharp.Data.Adaptive
open Fun.Blazor
open Fun.Blazor.Validators


type IComponentHook with
    member hook.UseLoginForm () =
        hook
            .UseAdaptiveForm<_, _>({| Name = ""; Password = "" |})
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


let login error =
    html.inject (fun (hook: IComponentHook, config: IConfiguration) ->
        let knocked = cval false
        let formData = hook.UseLoginForm()
        let title = config.GetValue<string>("Home:Title")

        let errorsView es =
            Template.html $"""
                <div class="rounded-md p-2 mt-5 text-danger-600/50 text-xs">
                    {es |> String.concat ", "}
                </div>
            """

        let loginForm =
            adaptiview () {
                let! name, setName = formData.UseField(fun x -> x.Name)
                let! pwd, setPwd = formData.UseField(fun x -> x.Password)
                let! errors = formData.UseErrors()
                let hasErrors = errors |> Seq.isEmpty |> not

                Template.html $"""
                    <form action="/user/login" method="post" class="w-[450px] p-10 rounded-md shadow-lg bg-gradient-to-br from-primary-100/10 to-primary-400/10">
                        <p class="text-primary-500 font-extrabold text-3xl mb-5">Welcome to</p>
                        <div class="rounded-md shadow-sm -space-y-px">
                            <div>
                                <label for="username" class="sr-only">Name</label>
                                <input value="{name}" oninput="{callback(fun (e: ChangeEventArgs) -> e.Value |> string |> setName)}" id="username" name="name" type="text" autocomplete="name" placeholder="Name" required autofocus
                                    class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-800 rounded-t-md focus:outline-none focus:ring-primary-500 focus:border-primary-500 focus:z-10">
                            </div>
                            <div>
                                <label for="password" class="sr-only">Password</label>
                                <input value="{pwd}" oninput="{callback(fun (e: ChangeEventArgs) -> e.Value |> string |> setPwd)}" id="password" name="password" type="password" autocomplete="current-password" placeholder="Password" required
                                    class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-800 rounded-b-md focus:outline-none focus:ring-primary-500 focus:border-primary-500 focus:z-10">
                            </div>
                        </div>
                        <div class="mt-5">
                            <button {if hasErrors then "disabled" else ""} type="submit" class="w-full py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-warning-500">
                                Open the door
                            </button>
                        </div>
                        {if hasErrors then errorsView errors else html.none}
                    </form>
                """
            }

        let welcome =
            Template.html $"""
                <div
                    ondblclick="{fun _ -> knocked.Publish true}"
                    class="p-8 rounded-lg shadow-lg bg-gradient-to-br from-primary-100/10 to-primary-400/10 mx-20 sm:mx-5 md:mx-5 cursor-pointer select-none"
                >
                    <p class="text-primary-500 font-extrabold text-3xl">Welcome to</p>
                    <p class="text-success-600 text-5xl opacity-70">{title}</p>
                    <p class="text-gray-400/40 mt-5 animate-bounce">Knock knock ...</p>
                    {match error with
                     | Some e -> errorsView [e]
                     | None -> html.none}
                </div>
            """

        adaptiview () {
            let! knocked = knocked

            Template.html $"""
                <div class="h-full flex flex-col justify-center items-center bg-gradient-to-br from-primary-300/10 to-primary-100/10">
                    {if knocked then loginForm else welcome}
                </div>
            """
        }
    )


let logoutBtn =
    Template.html $"""
        <form action="/user/logout" method="post" style="margin: 0px;">
            <button type="submit" class="py-2 px-5 border border-transparent text-sm font-medium rounded-md text-white bg-success-600 hover:bg-success-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-warning-500 opacity-60 hover:opacity-90">
                EXIT
            </button>
        </form>
    """
