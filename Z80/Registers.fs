namespace Z80.Registers

type SetRegisters = {
        // 8 bit registers 
        mutable A: uint8
        mutable B: uint8
        mutable C: uint8
        mutable D: uint8
        mutable E: uint8
        mutable H: uint8
        mutable L: uint8
(*
        // Flag Register

        The following flags are available:

        S               Sign flag (bit 7)
        Z               Zero flag (bit 6)
        H               Half Carry flag (bit 4)
        P               Parity/Overflow flag (bit 2)
        N               Add/Subtract flag (bit 1)
        C               Carry flag (bit 0)

*)
        mutable F: uint8

        // probably just derived?
        // // 16 bit registers:
        // // A and flags
        // AF: uint16
        // // B and C
        // BC: uint16
        // // D and E
        // DE: uint16
        // // used for addressing
        // HL: uint16
        // // 16-bit index registers
}

type Registers =
    {      
        // 8-bit Interrupt Page address register
        mutable I: uint8
        // 8-bit Memory Refresh register 
        mutable R: uint8
       
        mutable IX: uint16
        mutable IY: uint16
        // program counter
        mutable PC: uint16

        // stack pointer
        mutable SP: uint16
    }

module Build = 
        let emptyRegisters () = {
            I = 0uy;
            R = 0uy;
            IX = 0us;
            IY = 0us;
            PC = 0us;
            SP = 0us;
        }

        let emptySet () = {
                A = 0uy;
                B = 0uy;
                C = 0uy;
                D = 0uy;
                E = 0uy;
                H = 0uy;
                L = 0uy;
                F = 0uy;
        }