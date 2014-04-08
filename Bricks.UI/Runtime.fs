module BricksUIRuntime

open Bricks
open BricksHost
open BricksUI
open BricksUIOpenTK
open System.Windows
open System.Linq

let run (application:Application brick) = 

    let windows = application |> convert (fun app -> app.windows)

    let windowChanges = windows |> IdSet.trackChanges Window.identify
    
    let host = ProgramHost()
    let windowProjector = _Window.projector host

    let p = program {
        let! wc = windowChanges
        do! wc |> windowProjector.project
        if windowProjector.empty then
            Forms.Application.Exit()
    }

    host.run p
    let windows = windowProjector.values
    if windows.Count() = 0 then () else
    let mainWindow = windows.First()
    mainWindow.Run(30.0, 0.0)
    