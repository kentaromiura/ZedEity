namespace Tests.Z80
open Snapshot
open Z80.CPU
open Z80.Registers

module Hello = 
    let testHelloWorld () : bool =
        // example hello world from https://stackoverflow.com/q/56070020
        let testASM = [
            0x21; 0x0C; 0x00 // ld hl; 000C
            0x06; 0x0F // ld b;0f

            0x7e; // ld a;(hl)

            0x23; // inc hl
            
            0xD3; 0x00; // out (00); a
            
            0x10; 0xFA; // djnz
            0x76; // halt
            0x48; 0x65; 0x6C; 0x6C; 0x6F; // Hello
            0x2D; 0x77; 0x6F; 0x72; 0x6C; //  Worl 
            0x64; 0x20; 0x21; 0x21//d !!
            0x21; // !
        ]
        
        let er = Build.emptyRegisters()
        let es = Build.emptySet()
        let es2 = Build.emptySet()
        let cpu = Z80(er, es, es2)
        testASM |> List.iteri ( 
            fun index byte -> 
                // next line is useful for implementing missing opcodes
                // by referencing the first HEX in https://github.com/toptensoftware/yazd/blob/master/instructions.txt
                // then adding it to `Instructions.fs` first and then implementing it in `Z80.fs`
                // printfn "index: %d %u(%x)" index byte byte
                cpu.memory.[index] <- uint8 byte
        ) 
        cpu.next() |> ignore
        (sprintf "%02X " es.H).ToMatchSnapshot() |> ignore
        (sprintf "%02X " es.L).ToMatchSnapshot() |> ignore
        cpu.next() |> ignore
        (sprintf "%02X " es.B).ToMatchSnapshot() |> ignore
        cpu.next() |> ignore
        (sprintf "%02X " es.A).ToMatchSnapshot() |> ignore
        cpu.next() |> ignore
        (sprintf "%02X " es.H).ToMatchSnapshot() |> ignore
        (sprintf "%02X " es.L).ToMatchSnapshot() |> ignore
        cpu.next() |> ignore // Print 'H' 
        (sprintf "b %02X " es.B).ToMatchSnapshot() |> ignore
        (sprintf "pc %02X " er.PC).ToMatchSnapshot() |> ignore
        cpu.next() |> ignore
        (sprintf "b %02X " es.B).ToMatchSnapshot() |> ignore
        (sprintf "pc %02X " er.PC).ToMatchSnapshot() |> ignore
        while cpu.next() do ()
        
        let memoryDump = ("> ", cpu.memory[0..39]) ||> Array.fold (fun acc byte ->
             match acc.Length % 27 = 23 with 
                | false -> acc + (sprintf "%02X " byte)
                | true -> acc + (sprintf "%02X \n> " byte)
        )
        memoryDump.ToMatchSnapshot()
