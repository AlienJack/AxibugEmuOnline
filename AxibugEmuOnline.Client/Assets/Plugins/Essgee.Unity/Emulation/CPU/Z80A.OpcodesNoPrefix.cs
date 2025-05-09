﻿using static Essgee.Emulation.Utilities;

namespace Essgee.Emulation.CPU
{
    public partial class Z80A
    {
        static SimpleOpcodeDelegate[] opcodesNoPrefix = new SimpleOpcodeDelegate[]
        {
			/* 0x00 */
			new SimpleOpcodeDelegate((c) => { /* NOP */ }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegisterImmediate16(ref c.bc.Word); }),
            new SimpleOpcodeDelegate((c) => { c.LoadMemory8(c.bc.Word, c.af.High); }),
            new SimpleOpcodeDelegate((c) => { c.Increment16(ref c.bc.Word); }),
            new SimpleOpcodeDelegate((c) => { c.Increment8(ref c.bc.High); }),
            new SimpleOpcodeDelegate((c) => { c.Decrement8(ref c.bc.High); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegisterImmediate8(ref c.bc.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.RotateLeftAccumulatorCircular(); }),
            new SimpleOpcodeDelegate((c) => { c.ExchangeRegisters16(ref c.af, ref c.af_); }),
            new SimpleOpcodeDelegate((c) => { c.Add16(ref c.hl, c.bc.Word, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegisterFromMemory8(ref c.af.High, c.bc.Word, false); }),
            new SimpleOpcodeDelegate((c) => { c.Decrement16(ref c.bc.Word); }),
            new SimpleOpcodeDelegate((c) => { c.Increment8(ref c.bc.Low); }),
            new SimpleOpcodeDelegate((c) => { c.Decrement8(ref c.bc.Low); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegisterImmediate8(ref c.bc.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.RotateRightAccumulatorCircular(); }),
			/* 0x10 */
			new SimpleOpcodeDelegate((c) => { c.DecrementJumpNonZero(); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegisterImmediate16(ref c.de.Word); }),
            new SimpleOpcodeDelegate((c) => { c.LoadMemory8(c.de.Word, c.af.High); }),
            new SimpleOpcodeDelegate((c) => { c.Increment16(ref c.de.Word); }),
            new SimpleOpcodeDelegate((c) => { c.Increment8(ref c.de.High); }),
            new SimpleOpcodeDelegate((c) => { c.Decrement8(ref c.de.High); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegisterImmediate8(ref c.de.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.RotateLeftAccumulator(); }),
            new SimpleOpcodeDelegate((c) => { c.Jump8(); }),
            new SimpleOpcodeDelegate((c) => { c.Add16(ref c.hl, c.de.Word, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegisterFromMemory8(ref c.af.High, c.de.Word, false); }),
            new SimpleOpcodeDelegate((c) => { c.Decrement16(ref c.de.Word); }),
            new SimpleOpcodeDelegate((c) => { c.Increment8(ref c.de.Low); }),
            new SimpleOpcodeDelegate((c) => { c.Decrement8(ref c.de.Low); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegisterImmediate8(ref c.de.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.RotateRightAccumulator(); }),
			/* 0x20 */
			new SimpleOpcodeDelegate((c) => { c.JumpConditional8(!c.IsFlagSet(Flags.Zero)); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegisterImmediate16(ref c.hl.Word); }),
            new SimpleOpcodeDelegate((c) => { c.LoadMemory16(c.ReadMemory16(c.pc), c.hl.Word); c.pc += 2; }),
            new SimpleOpcodeDelegate((c) => { c.Increment16(ref c.hl.Word); }),
            new SimpleOpcodeDelegate((c) => { c.Increment8(ref c.hl.High); }),
            new SimpleOpcodeDelegate((c) => { c.Decrement8(ref c.hl.High); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegisterImmediate8(ref c.hl.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.DecimalAdjustAccumulator(); }),
            new SimpleOpcodeDelegate((c) => { c.JumpConditional8(c.IsFlagSet(Flags.Zero)); }),
            new SimpleOpcodeDelegate((c) => { c.Add16(ref c.hl, c.hl.Word, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister16(ref c.hl.Word, c.ReadMemory16(c.ReadMemory16(c.pc))); c.pc += 2; }),
            new SimpleOpcodeDelegate((c) => { c.Decrement16(ref c.hl.Word); }),
            new SimpleOpcodeDelegate((c) => { c.Increment8(ref c.hl.Low); }),
            new SimpleOpcodeDelegate((c) => { c.Decrement8(ref c.hl.Low); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegisterImmediate8(ref c.hl.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.af.High ^= 0xFF; c.SetFlag(Flags.Subtract | Flags.HalfCarry); c.SetClearFlagConditional(Flags.UnusedBitY, IsBitSet(c.af.High, 5)); c.SetClearFlagConditional(Flags.UnusedBitX, IsBitSet(c.af.High, 3)); }),
			/* 0x30 */
			new SimpleOpcodeDelegate((c) => { c.JumpConditional8(!c.IsFlagSet(Flags.Carry)); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegisterImmediate16(ref c.sp); }),
            new SimpleOpcodeDelegate((c) => { c.LoadMemory8(c.ReadMemory16(c.pc), c.af.High); c.pc += 2; }),
            new SimpleOpcodeDelegate((c) => { c.Increment16(ref c.sp); }),
            new SimpleOpcodeDelegate((c) => { c.IncrementMemory8(c.hl.Word); }),
            new SimpleOpcodeDelegate((c) => { c.DecrementMemory8(c.hl.Word); }),
            new SimpleOpcodeDelegate((c) => { c.LoadMemory8(c.hl.Word, c.ReadMemory8(c.pc++)); }),
            new SimpleOpcodeDelegate((c) => { c.SetFlag(Flags.Carry); c.ClearFlag(Flags.Subtract | Flags.HalfCarry); c.SetClearFlagConditional(Flags.UnusedBitY, IsBitSet(c.af.High, 5)); c.SetClearFlagConditional(Flags.UnusedBitX, IsBitSet(c.af.High, 3)); }),
            new SimpleOpcodeDelegate((c) => { c.JumpConditional8(c.IsFlagSet(Flags.Carry)); }),
            new SimpleOpcodeDelegate((c) => { c.Add16(ref c.hl, c.sp, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegisterFromMemory8(ref c.af.High, c.ReadMemory16(c.pc), false); c.pc += 2; }),
            new SimpleOpcodeDelegate((c) => { c.Decrement16(ref c.sp); }),
            new SimpleOpcodeDelegate((c) => { c.Increment8(ref c.af.High); }),
            new SimpleOpcodeDelegate((c) => { c.Decrement8(ref c.af.High); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegisterImmediate8(ref c.af.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.SetClearFlagConditional(Flags.HalfCarry, c.IsFlagSet(Flags.Carry)); c.SetClearFlagConditional(Flags.Carry, !c.IsFlagSet(Flags.Carry)); c.ClearFlag(Flags.Subtract); c.SetClearFlagConditional(Flags.UnusedBitY, IsBitSet(c.af.High, 5)); c.SetClearFlagConditional(Flags.UnusedBitX, IsBitSet(c.af.High, 3)); }),
			/* 0x40 */
			new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.bc.High, c.bc.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.bc.High, c.bc.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.bc.High, c.de.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.bc.High, c.de.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.bc.High, c.hl.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.bc.High, c.hl.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.bc.High = c.ReadMemory8(c.hl.Word); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.bc.High, c.af.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.bc.Low, c.bc.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.bc.Low, c.bc.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.bc.Low, c.de.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.bc.Low, c.de.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.bc.Low, c.hl.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.bc.Low, c.hl.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.bc.Low = c.ReadMemory8(c.hl.Word); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.bc.Low, c.af.High, false); }),
			/* 0x50 */
			new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.de.High, c.bc.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.de.High, c.bc.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.de.High, c.de.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.de.High, c.de.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.de.High, c.hl.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.de.High, c.hl.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.de.High = c.ReadMemory8(c.hl.Word); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.de.High, c.af.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.de.Low, c.bc.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.de.Low, c.bc.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.de.Low, c.de.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.de.Low, c.de.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.de.Low, c.hl.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.de.Low, c.hl.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.de.Low = c.ReadMemory8(c.hl.Word); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.de.Low, c.af.High, false); }),
			/* 0x60 */
			new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.hl.High, c.bc.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.hl.High, c.bc.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.hl.High, c.de.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.hl.High, c.de.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.hl.High, c.hl.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.hl.High, c.hl.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.hl.High = c.ReadMemory8(c.hl.Word); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.hl.High, c.af.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.hl.Low, c.bc.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.hl.Low, c.bc.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.hl.Low, c.de.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.hl.Low, c.de.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.hl.Low, c.hl.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.hl.Low, c.hl.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.hl.Low = c.ReadMemory8(c.hl.Word); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.hl.Low, c.af.High, false); }),
			/* 0x70 */
			new SimpleOpcodeDelegate((c) => { c.LoadMemory8(c.hl.Word, c.bc.High); }),
            new SimpleOpcodeDelegate((c) => { c.LoadMemory8(c.hl.Word, c.bc.Low); }),
            new SimpleOpcodeDelegate((c) => { c.LoadMemory8(c.hl.Word, c.de.High); }),
            new SimpleOpcodeDelegate((c) => { c.LoadMemory8(c.hl.Word, c.de.Low); }),
            new SimpleOpcodeDelegate((c) => { c.LoadMemory8(c.hl.Word, c.hl.High); }),
            new SimpleOpcodeDelegate((c) => { c.LoadMemory8(c.hl.Word, c.hl.Low); }),
            new SimpleOpcodeDelegate((c) => { c.EnterHaltState(); }),
            new SimpleOpcodeDelegate((c) => { c.LoadMemory8(c.hl.Word, c.af.High); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.af.High, c.bc.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.af.High, c.bc.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.af.High, c.de.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.af.High, c.de.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.af.High, c.hl.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.af.High, c.hl.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.af.High = c.ReadMemory8(c.hl.Word); }),
            new SimpleOpcodeDelegate((c) => { c.LoadRegister8(ref c.af.High, c.af.High, false); }),
			/* 0x80 */
			new SimpleOpcodeDelegate((c) => { c.Add8(c.bc.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.bc.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.de.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.de.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.hl.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.hl.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.ReadMemory8(c.hl.Word), false); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.af.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.bc.High, true); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.bc.Low, true); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.de.High, true); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.de.Low, true); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.hl.High, true); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.hl.Low, true); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.ReadMemory8(c.hl.Word), true); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.af.High, true); }),
			/* 0x90 */
			new SimpleOpcodeDelegate((c) => { c.Subtract8(c.bc.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.bc.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.de.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.de.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.hl.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.hl.Low, false); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.ReadMemory8(c.hl.Word), false); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.af.High, false); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.bc.High, true); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.bc.Low, true); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.de.High, true); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.de.Low, true); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.hl.High, true); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.hl.Low, true); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.ReadMemory8(c.hl.Word), true); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.af.High, true); }),
			/* 0xA0 */
			new SimpleOpcodeDelegate((c) => { c.And8(c.bc.High); }),
            new SimpleOpcodeDelegate((c) => { c.And8(c.bc.Low); }),
            new SimpleOpcodeDelegate((c) => { c.And8(c.de.High); }),
            new SimpleOpcodeDelegate((c) => { c.And8(c.de.Low); }),
            new SimpleOpcodeDelegate((c) => { c.And8(c.hl.High); }),
            new SimpleOpcodeDelegate((c) => { c.And8(c.hl.Low); }),
            new SimpleOpcodeDelegate((c) => { c.And8(c.ReadMemory8(c.hl.Word)); }),
            new SimpleOpcodeDelegate((c) => { c.And8(c.af.High); }),
            new SimpleOpcodeDelegate((c) => { c.Xor8(c.bc.High); }),
            new SimpleOpcodeDelegate((c) => { c.Xor8(c.bc.Low); }),
            new SimpleOpcodeDelegate((c) => { c.Xor8(c.de.High); }),
            new SimpleOpcodeDelegate((c) => { c.Xor8(c.de.Low); }),
            new SimpleOpcodeDelegate((c) => { c.Xor8(c.hl.High); }),
            new SimpleOpcodeDelegate((c) => { c.Xor8(c.hl.Low); }),
            new SimpleOpcodeDelegate((c) => { c.Xor8(c.ReadMemory8(c.hl.Word)); }),
            new SimpleOpcodeDelegate((c) => { c.Xor8(c.af.High); }),
			/* 0xB0 */
			new SimpleOpcodeDelegate((c) => { c.Or8(c.bc.High); }),
            new SimpleOpcodeDelegate((c) => { c.Or8(c.bc.Low); }),
            new SimpleOpcodeDelegate((c) => { c.Or8(c.de.High); }),
            new SimpleOpcodeDelegate((c) => { c.Or8(c.de.Low); }),
            new SimpleOpcodeDelegate((c) => { c.Or8(c.hl.High); }),
            new SimpleOpcodeDelegate((c) => { c.Or8(c.hl.Low); }),
            new SimpleOpcodeDelegate((c) => { c.Or8(c.ReadMemory8(c.hl.Word)); }),
            new SimpleOpcodeDelegate((c) => { c.Or8(c.af.High); }),
            new SimpleOpcodeDelegate((c) => { c.Cp8(c.bc.High); }),
            new SimpleOpcodeDelegate((c) => { c.Cp8(c.bc.Low); }),
            new SimpleOpcodeDelegate((c) => { c.Cp8(c.de.High); }),
            new SimpleOpcodeDelegate((c) => { c.Cp8(c.de.Low); }),
            new SimpleOpcodeDelegate((c) => { c.Cp8(c.hl.High); }),
            new SimpleOpcodeDelegate((c) => { c.Cp8(c.hl.Low); }),
            new SimpleOpcodeDelegate((c) => { c.Cp8(c.ReadMemory8(c.hl.Word)); }),
            new SimpleOpcodeDelegate((c) => { c.Cp8(c.af.High); }),
			/* 0xC0 */
			new SimpleOpcodeDelegate((c) => { c.ReturnConditional(!c.IsFlagSet(Flags.Zero)); }),
            new SimpleOpcodeDelegate((c) => { c.Pop(ref c.bc); }),
            new SimpleOpcodeDelegate((c) => { c.JumpConditional16(!c.IsFlagSet(Flags.Zero)); }),
            new SimpleOpcodeDelegate((c) => { c.JumpConditional16(true); }),
            new SimpleOpcodeDelegate((c) => { c.CallConditional16(!c.IsFlagSet(Flags.Zero)); }),
            new SimpleOpcodeDelegate((c) => { c.Push(c.bc); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.ReadMemory8(c.pc++), false); }),
            new SimpleOpcodeDelegate((c) => { c.Restart(0x0000); }),
            new SimpleOpcodeDelegate((c) => { c.ReturnConditional(c.IsFlagSet(Flags.Zero)); }),
            new SimpleOpcodeDelegate((c) => { c.Return(); }),
            new SimpleOpcodeDelegate((c) => { c.JumpConditional16(c.IsFlagSet(Flags.Zero)); }),
            new SimpleOpcodeDelegate((c) => { /* CB - handled elsewhere */ }),
            new SimpleOpcodeDelegate((c) => { c.CallConditional16(c.IsFlagSet(Flags.Zero)); }),
            new SimpleOpcodeDelegate((c) => { c.Call16(); }),
            new SimpleOpcodeDelegate((c) => { c.Add8(c.ReadMemory8(c.pc++), true); }),
            new SimpleOpcodeDelegate((c) => { c.Restart(0x0008); }),
			/* 0xD0 */
			new SimpleOpcodeDelegate((c) => { c.ReturnConditional(!c.IsFlagSet(Flags.Carry)); }),
            new SimpleOpcodeDelegate((c) => { c.Pop(ref c.de); }),
            new SimpleOpcodeDelegate((c) => { c.JumpConditional16(!c.IsFlagSet(Flags.Carry)); }),
            new SimpleOpcodeDelegate((c) => { c.WritePort(c.ReadMemory8(c.pc++), c.af.High); }),
            new SimpleOpcodeDelegate((c) => { c.CallConditional16(!c.IsFlagSet(Flags.Carry)); }),
            new SimpleOpcodeDelegate((c) => { c.Push(c.de); }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.ReadMemory8(c.pc++), false); }),
            new SimpleOpcodeDelegate((c) => { c.Restart(0x0010); }),
            new SimpleOpcodeDelegate((c) => { c.ReturnConditional(c.IsFlagSet(Flags.Carry)); }),
            new SimpleOpcodeDelegate((c) => { c.ExchangeRegisters16(ref c.bc, ref c.bc_); c.ExchangeRegisters16(ref c.de, ref c.de_); c.ExchangeRegisters16(ref c.hl, ref c.hl_); }),
            new SimpleOpcodeDelegate((c) => { c.JumpConditional16(c.IsFlagSet(Flags.Carry)); }),
            new SimpleOpcodeDelegate((c) => { c.af.High = c.ReadPort(c.ReadMemory8(c.pc++)); }),
            new SimpleOpcodeDelegate((c) => { c.CallConditional16(c.IsFlagSet(Flags.Carry)); }),
            new SimpleOpcodeDelegate((c) => { /* DD - handled elsewhere */ }),
            new SimpleOpcodeDelegate((c) => { c.Subtract8(c.ReadMemory8(c.pc++), true); }),
            new SimpleOpcodeDelegate((c) => { c.Restart(0x0018); }),
			/* 0xE0 */
			new SimpleOpcodeDelegate((c) => { c.ReturnConditional(!c.IsFlagSet(Flags.ParityOrOverflow)); }),
            new SimpleOpcodeDelegate((c) => { c.Pop(ref c.hl); }),
            new SimpleOpcodeDelegate((c) => { c.JumpConditional16(!c.IsFlagSet(Flags.ParityOrOverflow)); }),
            new SimpleOpcodeDelegate((c) => { c.ExchangeStackRegister16(ref c.hl); }),
            new SimpleOpcodeDelegate((c) => { c.CallConditional16(!c.IsFlagSet(Flags.ParityOrOverflow)); }),
            new SimpleOpcodeDelegate((c) => { c.Push(c.hl); }),
            new SimpleOpcodeDelegate((c) => { c.And8(c.ReadMemory8(c.pc++)); }),
            new SimpleOpcodeDelegate((c) => { c.Restart(0x0020); }),
            new SimpleOpcodeDelegate((c) => { c.ReturnConditional(c.IsFlagSet(Flags.ParityOrOverflow)); }),
            new SimpleOpcodeDelegate((c) => { c.pc = c.hl.Word; }),
            new SimpleOpcodeDelegate((c) => { c.JumpConditional16(c.IsFlagSet(Flags.ParityOrOverflow)); }),
            new SimpleOpcodeDelegate((c) => { c.ExchangeRegisters16(ref c.de, ref c.hl); }),
            new SimpleOpcodeDelegate((c) => { c.CallConditional16(c.IsFlagSet(Flags.ParityOrOverflow)); }),
            new SimpleOpcodeDelegate((c) => { /* ED - handled elsewhere */ }),
            new SimpleOpcodeDelegate((c) => { c.Xor8(c.ReadMemory8(c.pc++)); }),
            new SimpleOpcodeDelegate((c) => { c.Restart(0x0028); }),
			/* 0xF0 */
			new SimpleOpcodeDelegate((c) => { c.ReturnConditional(!c.IsFlagSet(Flags.Sign)); }),
            new SimpleOpcodeDelegate((c) => { c.Pop(ref c.af); }),
            new SimpleOpcodeDelegate((c) => { c.JumpConditional16(!c.IsFlagSet(Flags.Sign)); }),
            new SimpleOpcodeDelegate((c) => { c.iff1 = c.iff2 = false; }),
            new SimpleOpcodeDelegate((c) => { c.CallConditional16(!c.IsFlagSet(Flags.Sign)); }),
            new SimpleOpcodeDelegate((c) => { c.Push(c.af); }),
            new SimpleOpcodeDelegate((c) => { c.Or8(c.ReadMemory8(c.pc++)); }),
            new SimpleOpcodeDelegate((c) => { c.Restart(0x0030); }),
            new SimpleOpcodeDelegate((c) => { c.ReturnConditional(c.IsFlagSet(Flags.Sign)); }),
            new SimpleOpcodeDelegate((c) => { c.sp = c.hl.Word; }),
            new SimpleOpcodeDelegate((c) => { c.JumpConditional16(c.IsFlagSet(Flags.Sign)); }),
            new SimpleOpcodeDelegate((c) => { c.eiDelay = true; }),
            new SimpleOpcodeDelegate((c) => { c.CallConditional16(c.IsFlagSet(Flags.Sign)); }),
            new SimpleOpcodeDelegate((c) => { /* FD - handled elsewhere */ }),
            new SimpleOpcodeDelegate((c) => { c.Cp8(c.ReadMemory8(c.pc++)); }),
            new SimpleOpcodeDelegate((c) => { c.Restart(0x0038); })
        };
    }
}
