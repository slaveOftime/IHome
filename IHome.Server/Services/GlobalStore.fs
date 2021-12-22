[<AutoOpen>]
module IHome.Server.Services.GlobalStore

open System.Reflection
open Fun.Blazor


type StaticStore =    
    static member Version = Assembly.GetExecutingAssembly().GetName().Version.ToString().GetHashCode()


type IGlobalStore with

    member store.UseCPUTemperature() = store.CreateCVal("cpu-temperature", 40.)
    
    member store.UseHasObstacleOnLeft() = store.CreateCVal("has-obstacle-left", false)
    member store.UseHasObstacleOnRight() = store.CreateCVal("has-obstacle-right", false)
