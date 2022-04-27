﻿using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
    internal class Add : EmuInstruction
    {
        internal override OpCode OpCode
        {
            get
            {
                return OpCodes.Add;
            }
        }

        internal override void Emulate(EmuContext context, Instruction instr)
        {
            int right = (int)context.Stack.Pop();
            int left = (int)context.Stack.Pop();
            context.Stack.Push(left + right);
        }
    }
}
