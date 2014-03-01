module BricksHost

open Bricks

type EventHandler = interface end

type EventHandler<'e> = 'e -> Transaction

type ProgramHost() =
    let mutable _activeHandlers: HashMap<obj, obj -> Transaction> = HashMap.Empty
    let mutable _programM: ProgramM option = None
    let mutable _program: Program option = None

    member this.activate host eventHandler =
        _activeHandlers <- _activeHandlers.SetItem(host, unbox >> eventHandler)

    member this.deactivate host = 
        _activeHandlers <- _activeHandlers.Remove host

    member this.get host =
        _activeHandlers.get host

    member this.dispatch host event = 
        match this.get host with
        | None -> ()
        | Some handler -> 
            let transaction = handler event
            let applyTransaction = program { apply transaction }
            // we apply the transaction & the program loop
            _program <- _program.Value |> (applyTransaction >> _programM.Value) |> Some

    member this.run programM =
        _programM <- Some programM
        _program <- programM Program.empty |> Some
