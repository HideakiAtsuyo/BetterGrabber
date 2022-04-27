using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
    internal class Br : EmuInstruction
    {
        internal override OpCode OpCode
        {
            get
            {
                return OpCodes.Br;
            }
        }

        internal override void Emulate(EmuContext context, Instruction instr)
        {
            context.InstructionPointer = context.Instructions.IndexOf((Instruction)instr.Operand);
        }
    }
}
