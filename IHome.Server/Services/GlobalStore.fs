[<AutoOpen>]
module IHome.Server.Services.GlobalStore

open System
open System.Reflection
open Fun.Blazor


type StaticStore =    
    static member Version =
        #if DEBUG
        Random().Next()
        #else
        Assembly.GetExecutingAssembly().GetName().Version.ToString().GetHashCode()
        #endif


type IGlobalStore with

    member store.UseCPUTemperature() = store.CreateCVal("cpu-temperature", 40.)
    
    member store.UseHasObstacleOnLeft() = store.CreateCVal("has-obstacle-left", false)
    member store.UseHasObstacleOnRight() = store.CreateCVal("has-obstacle-right", false)

    member store.UseIgnoreObstacle() = store.CreateCVal("ignore-obstacle", false)

    member store.UseTopMotorAngle() = store.CreateCVal("top-motor-angle", 90)
    member store.UseBottomMotorAngle() = store.CreateCVal("bottom-motor-angle", 110)
