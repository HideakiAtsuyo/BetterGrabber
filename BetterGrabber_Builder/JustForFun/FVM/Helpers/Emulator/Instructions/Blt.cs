using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
    internal class Blt : EmuInstruction
    {
        internal override OpCode OpCode
        {
            get
            {
                return OpCodes.Blt;
            }
        }

        internal override void Emulate(EmuContext context, Instruction instr)
        {
            int right = (int)context.Stack.Pop();
            int left = (int)context.Stack.Pop();
            bool flag = left < right;
            if (flag)
            {
                context.InstructionPointer = context.Instructions.IndexOf((Instruction)instr.Operand);
            }
        }
    }
}
