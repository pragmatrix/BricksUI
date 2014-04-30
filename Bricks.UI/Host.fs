module BricksHost

open Bricks

type EventHandler = interface end

type EventHandler<'e> = 'e -> Transaction

type ProgramHost() =
    let mutable _activeHandlers: HashMap<obj, obj -> Transaction> = HashMap.Empty
    let mutable _program: Program option = None

    member this.activate eventHost eventHandler =
        _activeHandlers <- _activeHandlers.SetItem(eventHost, unbox >> eventHandler)

    member this.deactivate eventHost = 
        _activeHandlers <- _activeHandlers.Remove eventHost

    member this.get eventHost =
        _activeHandlers.get eventHost

    member this.dispatch eventHost event = 
        match this.get eventHost with
        | None -> ()
        | Some handler -> 
            let transaction = handler event
            // run the transaction and the program
            transaction ()
            _program.Value.run()

    member this.run program =
        _program <- Some program
        program.run()
