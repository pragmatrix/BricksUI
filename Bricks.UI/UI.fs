module BricksUI

open Bricks

//
// Events
//

type EventHandler<'e> = 'e -> Transaction

type Color = { red: float; green: float; blue: float; alpha: float }
    with 
        static member formRGB(red, green, blue) = { red = red; green = green; blue = blue; alpha = 1. }

//
// Text
//

type TextStyle = { fontName: string; size: float; color: Color }

type Text = { style: TextStyle; text: string }

//
// Boxes
//

type Box =
    | TextBox of Text

//
// Window
//

type WindowEvent = | CloseWindow

type Window = { id: obj; title:string; width: int; height: int; content: Box brick; eventHandler: EventHandler<WindowEvent> }
    with
    static member identify = fun w -> w.id
    static member makeSet windows = windows |> IdSet.fromSeq Window.identify   


type Application = { windows: Window idset }

