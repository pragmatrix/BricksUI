module BricksUIOpenTK

open Bricks
open BricksUI
open BricksHost
open OpenTK
open System.Windows.Forms
open System.Diagnostics

type _Window(model: Window, host: ProgramHost) as this = 
    inherit GameWindow(model.width, model.height)
    let mutable model = model
    do
        this.Width <- model.width
        this.Height <- model.height
        // this. <- model.title

    member this.update(u: Window) =
        model <- u
        this.Width <- u.width
        this.Height <- u.height
        // this.Title <- u.title
        this

    override this.OnClosing args =
        args.Cancel <- true
        base.OnClosing args
        host.dispatch model CloseWindow

    static member projector(host: ProgramHost) = IdSet.Projector(
        (Window.identify),
        (fun w -> 
            let _w = new _Window(w, host)
            _w.Visible <- true
            host.activate w (w.eventHandler)
            _w
            ),
        (fun w _w -> 
            let r = _w.update w
            host.activate w (w.eventHandler)
            r
            ),
        (fun w _w -> 
            host.deactivate w
            _w.Dispose()))
