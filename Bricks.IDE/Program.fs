module BricksIDE

open Bricks
open BricksUI
open BricksUIRuntime

let myWindow = {Window.id = "1"; title = "Bricks IDE"; width = 800; height = 600}
let myApplication = {Application.windows = Window.makeSet [myWindow]}

run (value myApplication)

