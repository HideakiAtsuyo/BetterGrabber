using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
    internal class And : EmuInstruction
    {
        internal override OpCode OpCode
        {
            get
            {
                return OpCodes.And;
            }
        }

        internal override void Emulate(EmuContext context, Instruction instr)
        {
            int right = (int)context.Stack.Pop();
            int left = (int)context.Stack.Pop();
            context.Stack.Push(left & right);
        }
    }
}
