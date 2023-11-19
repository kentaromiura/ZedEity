namespace Test.Runner.SnapshotTests
open Snapshot
open Microsoft.FSharp.Quotations
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.IO
type Ref() = 
    member this.GetThisFilePath([<CallerFilePath; Optional; DefaultParameterValue("")>]?path: string) =
        path
    

module Runner = 
    
    let FixVariableNameFromCompiledName name: string =
        // Hack, currently the test will return a generated name such allTests@41-2
        // so we get the name from the line number, it works only because I've separated
        // each test in its own line. Note that probably it could work with multiples by splitting on ";"
        // 
        // This only need for test runner though so it's ok.
        let R: Ref = Ref()
        let thisFile = R.GetThisFilePath()
        match thisFile with
            | Some(path) ->
                let fileContent = File.ReadAllText path
                let lines = fileContent.Split("\n")
                
                if string(name).Contains("allTests@") then 
                    lines.[int (string(name).Split("allTests@").[1].Split("-").[0]) - 1].Trim()
                else name
            | None -> name
        

    let rec funName f = 
        
        match f with
        | Patterns.ValueWithName(obj,_type,name) -> FixVariableNameFromCompiledName (obj.GetType().Name)
        | Patterns.Call(None, methodInfo, _) -> methodInfo.Name
        | Patterns.Lambda(_, expr) -> funName expr
        | _ -> failwith "Unexpected input"

    let testTesterWorks () = 
        "This should work".ToMatchSnapshot() = true

    let runAll () = 
        // Important, because of how reflection works keep tests on separated lines.
        let allTests = [
            testTesterWorks; 
            Tests.Z80.bit8.testBit8;            
            Tests.Z80.Hello.testHelloWorld;
        ] 
        
        let ESCAPE_CHAR = (char 27)
        let results = allTests |> List.map (fun test -> 
            if test()
                then sprintf "%c[0;32mTest %c[1m%s%c[22m passed%c[0;37m" ESCAPE_CHAR ESCAPE_CHAR (funName <@ test @>) ESCAPE_CHAR ESCAPE_CHAR // this will not have access to the name inside the array
            else sprintf "%c[0;31mTest %c[1m%s%c[22m failed%c[0;37m" ESCAPE_CHAR ESCAPE_CHAR (funName <@ test @>) ESCAPE_CHAR ESCAPE_CHAR
        ) 
        // 3 lines.
        printfn("\n\n")
        results |> List.iter (fun res -> 
            printfn "%s" res
        )