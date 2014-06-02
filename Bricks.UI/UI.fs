module BricksUI

open Bricks
open System.Drawing


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

type Size = 
    struct 
        new (w: float, h: float) = {
            width = w
            height = h
            }

        val public width : float
        val public height : float
    end

type Rect =
    struct
        new (l: float, t: float, w: float, h: float) = {
            left = l
            top = t
            width = w
            height = h }

        val public left : float
        val public top : float
        val public width : float
        val public height : float

        member this.bottom = this.top + this.height
        member this.right = this.left + this.width
    end

type Box =
    | TextBox of Text
    | Container of (Rect brick * Box brick) ilist

//
// Window
//

type WindowEvent = | CloseWindow

type Window = { title:string; width: int; height: int; content: Box brick; eventHandler: EventHandler<WindowEvent> }
(*    
    with
    static member identify = fun w -> w.id
    static member makeSet windows = windows |> IdSet.fromSeq Window.identify   
*)

type Application = { windows: Window brick iset }


//
// Layout
//

let private measureBitmap = new Bitmap(1, 1)

let bitmapSizeOfText font text = 
    use graphics = Graphics.FromImage(measureBitmap)
    let sz = graphics.MeasureString(text, font)
    Size(sz.Width |> float, sz.Height |> float)

// tbd: need some kind of a font cache, but I would assume that the system takes care of.

let fontOf style = new Font(style.fontName, float32(style.size))

let sizeOf b =
    match b with
    | TextBox t ->
        let font = fontOf t.style
        bitmapSizeOfText font t.text
    | Container _ ->
        Size(0.,0.)
