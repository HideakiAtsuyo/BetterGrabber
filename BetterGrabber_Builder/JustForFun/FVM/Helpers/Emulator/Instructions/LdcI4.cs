using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
    internal class LdcI4 : EmuInstruction
    {
        internal override OpCode OpCode
        {
            get
            {
                return OpCodes.Ldc_I4;
            }
        }

        internal override void Emulate(EmuContext context, Instruction instr)
        {
            context.Stack.Push(instr.Operand);
        }
    }
}
