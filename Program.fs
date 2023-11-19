namespace Z80
open Test.Runner.SnapshotTests
// For more information see https://aka.ms/fsharp-console-apps
module Z80 =
    [<EntryPoint>]
    let main _ =
        //printfn "Hello from F#"
        ignore(Runner.runAll())
        0
    