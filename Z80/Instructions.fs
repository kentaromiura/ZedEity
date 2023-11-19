module Z80.Instructions

type Instruction = 
    | Unknown
    | LDHLXXXX of uint8 * uint8
    | LDBX of uint8
    | LDAPHL
    | INCHL
    | OUTXA of uint8
    | DJNZ of uint8
    | HALT


let nextInstruction (memory: uint8 array) (pc: uint16) =
  let instruction = memory.[int pc] in
  let pc' = pc + 1us in
  (instruction, pc')

let fetchNextInstruction memory pc = 
    let (instruction, nextPC) = nextInstruction memory pc

    match instruction with
    | 118uy -> (HALT, nextPC)
    | 16uy -> (DJNZ memory[int nextPC],  (nextPC + 1us))
    | 211uy -> (OUTXA memory[int nextPC], (nextPC + 1us))
    | 35uy -> (INCHL, nextPC)
    | 126uy -> (LDAPHL, nextPC)
    | 6uy -> (LDBX memory[int nextPC], (nextPC + 1us))
    | 33uy -> 
        let targetMem = (memory.[int(nextPC + 1us)], memory.[int nextPC])
        (LDHLXXXX targetMem, (nextPC + 2us))
    | _ -> 
        printfn "Invalid instruction at PC: %d %s " instruction (string pc);
        (Unknown, nextPC)