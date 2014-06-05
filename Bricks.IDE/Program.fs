module BricksIDE

open Bricks
open BricksUI
open BricksUIRuntime

let textStyle = { fontName = "Lucida console"; size = 14.; color = Color.formRGB(1., 1., 1.) }

let lines = Channel.source (IList.reset IList.empty<string>)

let mkTextBox text = brick {
        return TextBox { style = textStyle; text = text }
    }

let textBoxes = lines |> map mkTextBox

let sizes = textBoxes |> map (lift sizeOf)

let alignBottom (r:Rect) (sz:Size) = Rect(0., r.bottom, sz.width, sz.height)

let rectangles = sizes |> scan (lift alignBottom) (lift (Rect()))

let zipped = zip rectangles (textBoxes |> materialize)

let textContainer = convert Container zipped

// let textContainer = Container <| zip rectangles textBoxes

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



