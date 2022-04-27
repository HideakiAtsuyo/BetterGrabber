using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
    internal class Xor : EmuInstruction
    {
        internal override OpCode OpCode
        {
            get
            {
                return OpCodes.Xor;
            }
        }

        internal override void Emulate(EmuContext context, Instruction instr)
        {
            int right = (int)context.Stack.Pop();
            int left = (int)context.Stack.Pop();
            context.Stack.Push(left ^ right);
        }
    }
}
