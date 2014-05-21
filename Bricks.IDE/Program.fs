module BricksIDE

open Bricks
open BricksUI
open BricksUIRuntime

open System.Drawing;

let textStyle = { fontName = "Lucida console"; size = 14.; color = Color.formRGB(1., 1., 1.) }

let lines = Channel.source<string IList.change>()

let mkTextBox text = brick {
        return TextBox { style = textStyle; text = text }
    }

let textBoxes = lines |> map mkTextBox

let sizes = textBoxes |> map (lift sizeOf)

let positions = sizes |> scan (lift (fun pos (sz:SizeF) -> pos + sz.Height)) (lift (float32(0)))

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
    return { Application.windows = w |> ISet.ofSeq }
}

run application

