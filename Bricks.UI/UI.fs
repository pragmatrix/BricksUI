module BricksUI

open Bricks

type Window = { id: obj; title:string; width: int; height: int}
    with
    static member identify = fun w -> w.id
    static member makeSet windows = windows |> IdSet.fromSeq Window.identify   

type Application = { windows: Window idset }

