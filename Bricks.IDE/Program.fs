module BricksIDE

open Bricks
open BrickCollections
open BricksUI
open BricksUIRuntime

let textStyle = { fontName = "Lucida console"; size = 14.; color = Color.formRGB(1., 1., 1.) }

let lines = Channel.source<string List.change>()

let helloWorld = brick {
        return TextBox { style = textStyle; text = "Hello World!"}
        }

let myWindowVisible = value true

let myWindow = value {
    title = "Bricks IDE"; 
    width = 800; height = 600;
    content = helloWorld;
    eventHandler = fun ev ->
        match ev with
        | CloseWindow ->
            transaction { write myWindowVisible false }
    } 

let allWindows = brick {
        let! visible = myWindowVisible
        return if visible then [myWindow] else []
    }

let application = brick {
    let! w = allWindows
    return { Application.windows = w |> Set.ofSeq }
}

run application

