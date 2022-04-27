using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
    internal class Stloc : EmuInstruction
    {
        internal override OpCode OpCode
        {
            get
            {
                return OpCodes.Stloc_S;
            }
        }

        internal override void Emulate(EmuContext context, Instruction instr)
        {
            object val = context.Stack.Pop();
            context.SetLocalValue((Local)instr.Operand, val);
        }
    }
}
