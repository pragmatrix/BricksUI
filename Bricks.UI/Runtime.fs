module BricksUIRuntime

open Bricks
open BricksUI
open BricksUIOpenTK
open System.Windows

let run (application:Application brick) = 

    let windows = application |> convert (fun app -> app.windows)

    let windowChanges = windows |> IdSet.trackChanges Window.identify
    
    let windowProjector = _Window.projector()

    let p = program {
        let! windowChanges = windowChanges
        windowChanges |> windowProjector.project
    }

    let initialized = p Program.empty
    Forms.Application.Run()
