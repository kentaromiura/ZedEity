namespace Z80.CPU
open Z80.Bit8
open Z80.Registers
open Z80.Instructions

type Z80(regs: Registers, main: SetRegisters, alt: SetRegisters) = 
    let mutable isFlipFlopEnabled = true
    let mutable _main = main
    let mutable _alt = alt
    let mutable _memory: uint8 array = Array.zeroCreate 65536
    let mutable isRunning = true
    member this.regs = regs
    member this.main 
        with get() = _main
        and set(value) = _main <- value
    member this.alt 
        with get() = _alt
        and set(value) = _alt <- value


    member private this.isMainSelected = true

    member private this.FlipFlopEnabled 
        with get() = isFlipFlopEnabled
        and set(value) = isFlipFlopEnabled <- value
    
    member private this.getActiveRegs () =
        match this.isMainSelected with
            | true -> this.main
            | false -> this.alt
    member private this.getF () = 
        this.getActiveRegs().F

    member private this.setCarry () =
        let F = this.getF()
        this.getActiveRegs().F <- F ||| uint8(Bit8.Zero)


    member this.reset () = 
        this.FlipFlopEnabled <- true
        this.regs.PC <- 0us
        this.regs.I <- 0uy
        this.regs.R <- 0uy
        isRunning <- true
        
    member this.next () =
        match isRunning with
        | false -> false
        | true ->
            let (instruction, nextPC) = fetchNextInstruction this.memory this.regs.PC
            let mutable skipPC = false
            match instruction with
            | Instruction.HALT ->
                isRunning <- false
            | Instruction.DJNZ (jump) -> 
                let reljump = int8(jump)
                this.getActiveRegs().B <- this.getActiveRegs().B - 1uy
                // probably set flag if underflow
                if this.getActiveRegs().B <> 0uy then do
                    this.regs.PC <- uint16(int32(nextPC) + int32(reljump))
                    skipPC <- true
                
                ()
            | Instruction.OUTXA (port) ->
                // currently ignore port and
                // simulate port 0 with STDOUT
                (printf "%c" (char(this.getActiveRegs().A))) |> ignore
                ()
            | Instruction.INCHL ->
                match this.getActiveRegs().L = 255uy with
                | true -> 
                    this.getActiveRegs().L <- 0uy
                    this.getActiveRegs().H  <- this.getActiveRegs().H + 1uy
                    if this.getActiveRegs().H = 0uy then this.setCarry()
                | false ->
                    this.getActiveRegs().L <- this.getActiveRegs().L + 1uy
                ()
            | Instruction.LDAPHL ->
                let mutable tmp = 0us;
                tmp <- tmp + uint16(this.getActiveRegs().L) + uint16(this.getActiveRegs().H <<< 8)
                this.getActiveRegs().A <- this.memory[int tmp]
                ()
            | Instruction.LDBX (v) ->
                this.getActiveRegs().B <- v
                ()
            | Instruction.LDHLXXXX (h, l) -> 
                this.getActiveRegs().H <- h
                this.getActiveRegs().L <- l
                ()
            | Instruction.Unknown -> ()
            if not skipPC then this.regs.PC <- nextPC
            true
    
    member this.memory 
        with get() = _memory
        and set(value) = _memory <- value

    member this.isCarrySet = (this.getF() &&& uint8 Bit8.Zero) = uint8 Bit8.Zero
    member this.isAddSubtractSet = (this.getF() &&& uint8 Bit8.One) = uint8 Bit8.One
    member this.isParitySet = (this.getF() &&& uint8 Bit8.Two) = uint8 Bit8.Two

    member this.isUndocumented3Set = (this.getF() &&& uint8 Bit8.Three) = uint8 Bit8.Three
    member this.isHalfCarrySet = (this.getF() &&& uint8 Bit8.Four) = uint8 Bit8.Four

    member this.isUndocumented5Set = (this.getF() &&& uint8 Bit8.Five) = uint8 Bit8.Five

    member this.isZeroSet = (this.getF() &&& uint8 Bit8.Six) = uint8 Bit8.Six

    member this.isSignSet = (this.getF() &&& uint8 Bit8.Seven) = uint8 Bit8.Seven
