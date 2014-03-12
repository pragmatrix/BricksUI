module BricksHost

open Bricks

type EventHandler = interface end

type EventHandler<'e> = 'e -> Transaction

type ProgramHost() =
    let mutable _activeHandlers: HashMap<obj, obj -> Transaction> = HashMap.Empty
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
            // run the transaction and the program
            transaction ()
            _program.Value.run()

    member this.run program =
        _program <- Some program
        program.run()
