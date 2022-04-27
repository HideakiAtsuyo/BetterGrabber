using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.MethodBlocks
{
    public struct Trace
    {
        private static void Increment(Dictionary<uint, int> counts, uint key)
        {
            int value;
            bool flag = !counts.TryGetValue(key, out value);
            if (flag)
            {
                value = 0;
            }
            counts[key] = value + 1;
        }

        public Trace(CilBody body, bool hasReturnValue)
        {
            this.RefCount = new Dictionary<uint, int>();
            this.BrRefs = new Dictionary<uint, List<Instruction>>();
            this.BeforeStack = new Dictionary<uint, int>();
            this.AfterStack = new Dictionary<uint, int>();
            body.UpdateInstructionOffsets();
            foreach (ExceptionHandler eh in body.ExceptionHandlers)
            {
                this.BeforeStack[eh.TryStart.Offset] = 0;
                this.BeforeStack[eh.HandlerStart.Offset] = ((eh.HandlerType != ExceptionHandlerType.Finally) ? 1 : 0);
                bool flag = eh.FilterStart != null;
                if (flag)
                {
                    this.BeforeStack[eh.FilterStart.Offset] = 1;
                }
            }
            int currentStack = 0;
            int i = 0;
            while (i < body.Instructions.Count)
            {
                Instruction instr = body.Instructions[i];
                bool flag2 = this.BeforeStack.ContainsKey(instr.Offset);
                if (flag2)
                {
                    currentStack = this.BeforeStack[instr.Offset];
                }
                this.BeforeStack[instr.Offset] = currentStack;
                instr.UpdateStack(ref currentStack, hasReturnValue);
                this.AfterStack[instr.Offset] = currentStack;
                switch (instr.OpCode.FlowControl)
                {
                    case FlowControl.Branch:
                        {
                            uint offset = ((Instruction)instr.Operand).Offset;
                            bool flag3 = !this.BeforeStack.ContainsKey(offset);
                            if (flag3)
                            {
                                this.BeforeStack[offset] = currentStack;
                            }
                            Trace.Increment(this.RefCount, offset);
                            this.BrRefs.AddListEntry(offset, instr);
                            currentStack = 0;
                            break;
                        }
                    case FlowControl.Break:
                    case FlowControl.Meta:
                    case FlowControl.Next:
                        goto IL_2F9;
                    case FlowControl.Call:
                        {
                            bool flag4 = instr.OpCode.Code == Code.Jmp;
                            if (flag4)
                            {
                                currentStack = 0;
                            }
                            goto IL_2F9;
                        }
                    case FlowControl.Cond_Branch:
                        {
                            bool flag5 = instr.OpCode.Code == Code.Switch;
                            if (flag5)
                            {
                                foreach (Instruction target in (Instruction[])instr.Operand)
                                {
                                    bool flag6 = !this.BeforeStack.ContainsKey(target.Offset);
                                    if (flag6)
                                    {
                                        this.BeforeStack[target.Offset] = currentStack;
                                    }
                                    Trace.Increment(this.RefCount, target.Offset);
                                    this.BrRefs.AddListEntry(target.Offset, instr);
                                }
                            }
                            else
                            {
                                uint offset = ((Instruction)instr.Operand).Offset;
                                bool flag7 = !this.BeforeStack.ContainsKey(offset);
                                if (flag7)
                                {
                                    this.BeforeStack[offset] = currentStack;
                                }
                                Trace.Increment(this.RefCount, offset);
                                this.BrRefs.AddListEntry(offset, instr);
                            }
                            goto IL_2F9;
                        }
                    case FlowControl.Phi:
                        goto IL_2F3;
                    case FlowControl.Return:
                    case FlowControl.Throw:
                        break;
                    default:
                        goto IL_2F3;
                }
            IL_337:
                i++;
                continue;
            IL_2F9:
                bool flag8 = i + 1 < body.Instructions.Count;
                if (flag8)
                {
                    uint offset = body.Instructions[i + 1].Offset;
                    Trace.Increment(this.RefCount, offset);
                }
                goto IL_337;
            IL_2F3:
                throw new NotSupportedException();
            }
        }

        public bool IsBranchTarget(uint offset)
        {
            List<Instruction> src;
            bool flag = this.BrRefs.TryGetValue(offset, out src);
            return flag && src.Count > 0;
        }

        public bool HasMultipleSources(uint offset)
        {
            int src;
            bool flag = this.RefCount.TryGetValue(offset, out src);
            return flag && src > 1;
        }

        public Dictionary<uint, int> RefCount;

        public Dictionary<uint, List<Instruction>> BrRefs;

        public Dictionary<uint, int> BeforeStack;

        public Dictionary<uint, int> AfterStack;
    }
}
