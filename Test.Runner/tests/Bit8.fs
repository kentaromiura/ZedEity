namespace Tests.Z80
open Snapshot
open Z80.Bit8

module bit8 = 
    let testBit8 () =
        uint8(Bit8.Zero).ToMatchSnapshot() &&
        uint8(Bit8.One).ToMatchSnapshot() &&
        uint8(Bit8.Two).ToMatchSnapshot() &&
        uint8(Bit8.Three).ToMatchSnapshot() &&
        uint8(Bit8.Four).ToMatchSnapshot() &&
        uint8(Bit8.Five).ToMatchSnapshot() &&
        uint8(Bit8.Six).ToMatchSnapshot() &&
        uint8(Bit8.Seven).ToMatchSnapshot() 