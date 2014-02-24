module BricksUIOpenTK

open Bricks
open BricksUI
open OpenTK

type _Window(w: Window) as this = 
    inherit GameWindow()
    do
        this.Width <- w.width
        this.Height <- w.height
        this.Title <- w.title

    member this.update(u: Window) =
        this.Width <- u.width
        this.Height <- u.height
        this.Title <- u.title
        this

    static member projector() = IdSet.Projector(
        (Window.identify),
        (fun w -> 
            let w = new _Window(w)
            w.Visible <- true
            w
            ),
        (fun w _w -> _w.update w),
        (fun w _w -> _w.Dispose()))
