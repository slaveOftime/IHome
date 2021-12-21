namespace IHome.Server.Services

open System
open System.Collections.Concurrent
open System.Threading.Tasks


type private LoginInfo = { Count: int; FirstTryTime: DateTime }

type GuardService() =
    let loginRetry = ConcurrentDictionary<string, LoginInfo>()


    do
        task {
            while true do
                for KeyValue (key, item) in loginRetry do
                    if (DateTime.Now - item.FirstTryTime).Minutes > 5 then
                        loginRetry.TryRemove key |> ignore
                do! Task.Delay 2000
        }
        |> ignore


    member _.GetLoginRetryCount id =
        let info =
            loginRetry.AddOrUpdate(
                id,
                (fun _ -> { Count = 1; FirstTryTime = DateTime.Now }),
                (fun _ info -> { info with Count = info.Count + 1 }))
                
        info.Count
