module BricksUIRuntime

open Bricks
open BrickTime
open BricksHost
open BricksUI
open BricksUIOpenTK
open System.Windows
open System.Linq
open OpenTK

let run (application:Application brick) = 

    let windows = application |> convert (fun app -> app.windows)

    let host = ProgramHost<_>()

    let windowMapper w =
        brick {
            let! w = w
            let! sysw = manifest (fun () -> new _Window(w, host))
            return sysw.update(w)
        }

    let systemWindows = windows |> b.track |> b.map windowMapper |> b.materialize
 
    // tbd: the seq of windows may be a brick -> program can be converted to a brick
 
    let rootBrick = brick {
        let! windows = systemWindows
        let! realized = windows
        return realized |> Seq.toList
    }

    let program = rootBrick |> toProgram

    let windows = host.run program
    if windows.Count() = 0 then () else
    let mainWindow = windows.First()
    mainWindow.Run(30.0, 0.0)
