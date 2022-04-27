using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.MethodBlocks
{
    public class Block
    {
        public Block(Instruction[] Instructions, bool IsException = false, bool IsSafe = true, bool IsBranched = false, int initValue = -1)
        {
            this.Instructions = Instructions.ToList<Instruction>();
            this.IsSafe = IsSafe;
            this.IsBranched = IsBranched;
            this.IsException = IsException;
            this.Values = new List<int>
            {
                initValue
            };
        }

        public static Block Clone(Block block, bool all = false)
        {
            List<Instruction> instructions = new List<Instruction>();
            foreach (Instruction instr in block.Instructions)
            {
                instructions.Add(new Instruction
                {
                    OpCode = instr.OpCode,
                    Operand = instr.Operand
                });
                bool flag = !all && instr.OpCode == OpCodes.Stloc_S;
                if (flag)
                {
                    break;
                }
            }
            return new Block(instructions.ToArray(), block.IsException, block.IsSafe, block.IsBranched, -1);
        }

        public List<Instruction> Instructions;

        public bool IsSafe;

        public bool IsBranched;

        public bool IsException;

        public List<int> Values;
    }
}
