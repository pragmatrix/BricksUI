module BricksUI

open Bricks

type WindowEvent = 
    CloseWindow

type EventHandler<'e> = 'e -> Transaction

type Window = { id: obj; title:string; width: int; height: int; eventHandler: EventHandler<WindowEvent> }
    with
    static member identify = fun w -> w.id
    static member makeSet windows = windows |> IdSet.fromSeq Window.identify   

type Application = { windows: Window idset }

