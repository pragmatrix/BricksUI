module BricksIDE

open Bricks
open BricksUI
open BricksUIRuntime


let myWindowVisible = value true

let myWindow = {
    Window.id = "1"; 
    title = "Bricks IDE"; 
    width = 800; height = 600;
    eventHandler = fun ev ->
        match ev with
        | CloseWindow ->
            transaction { write myWindowVisible false }
    }

let myApplication = brick {
        let! visible = myWindowVisible
        return {Application.windows = Window.makeSet (if visible then [myWindow] else [])}
    }

run myApplication

