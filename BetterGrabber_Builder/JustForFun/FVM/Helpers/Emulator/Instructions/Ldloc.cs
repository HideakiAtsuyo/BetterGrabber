using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
    internal class Ldloc : EmuInstruction
    {
        internal override OpCode OpCode
        {
            get
            {
                return OpCodes.Ldloc_S;
            }
        }

        internal override void Emulate(EmuContext context, Instruction instr)
        {
            context.Stack.Push(context.GetLocalValue((Local)instr.Operand));
        }
    }
}
