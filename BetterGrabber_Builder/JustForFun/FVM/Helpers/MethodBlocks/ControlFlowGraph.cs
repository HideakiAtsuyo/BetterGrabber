using System;
using System.Collections;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.MethodBlocks
{
    public class ControlFlowGraph : IEnumerable<ControlFlowBlock>, IEnumerable
    {
        private ControlFlowGraph(CilBody body)
        {
            this.body = body;
            this.instrBlocks = new int[body.Instructions.Count];
            this.blocks = new List<ControlFlowBlock>();
            this.indexMap = new Dictionary<Instruction, int>();
            for (int i = 0; i < body.Instructions.Count; i++)
            {
                this.indexMap.Add(body.Instructions[i], i);
            }
        }

        public int Count
        {
            get
            {
                return this.blocks.Count;
            }
        }

        public ControlFlowBlock this[int id]
        {
            get
            {
                return this.blocks[id];
            }
        }

        public CilBody Body
        {
            get
            {
                return this.body;
            }
        }

        IEnumerator<ControlFlowBlock> IEnumerable<ControlFlowBlock>.GetEnumerator()
        {
            return this.blocks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.blocks.GetEnumerator();
        }

        public ControlFlowBlock GetContainingBlock(int instrIndex)
        {
            return this.blocks[this.instrBlocks[instrIndex]];
        }

        public int IndexOf(Instruction instr)
        {
            return this.indexMap[instr];
        }

        private void PopulateBlockHeaders(HashSet<Instruction> blockHeaders, HashSet<Instruction> entryHeaders)
        {
            for (int i = 0; i < this.body.Instructions.Count; i++)
            {
                Instruction instr = this.body.Instructions[i];
                bool flag = instr.Operand is Instruction;
                if (flag)
                {
                    blockHeaders.Add((Instruction)instr.Operand);
                    bool flag2 = i + 1 < this.body.Instructions.Count;
                    if (flag2)
                    {
                        blockHeaders.Add(this.body.Instructions[i + 1]);
                    }
                }
                else
                {
                    bool flag3 = instr.Operand is Instruction[];
                    if (flag3)
                    {
                        foreach (Instruction target in (Instruction[])instr.Operand)
                        {
                            blockHeaders.Add(target);
                        }
                        bool flag4 = i + 1 < this.body.Instructions.Count;
                        if (flag4)
                        {
                            blockHeaders.Add(this.body.Instructions[i + 1]);
                        }
                    }
                    else
                    {
                        bool flag5 = (instr.OpCode.FlowControl == FlowControl.Throw || instr.OpCode.FlowControl == FlowControl.Return) && i + 1 < this.body.Instructions.Count;
                        if (flag5)
                        {
                            blockHeaders.Add(this.body.Instructions[i + 1]);
                        }
                    }
                }
            }
            blockHeaders.Add(this.body.Instructions[0]);
            foreach (ExceptionHandler eh in this.body.ExceptionHandlers)
            {
                blockHeaders.Add(eh.TryStart);
                blockHeaders.Add(eh.HandlerStart);
                blockHeaders.Add(eh.FilterStart);
                entryHeaders.Add(eh.HandlerStart);
                entryHeaders.Add(eh.FilterStart);
            }
        }

        private void SplitBlocks(HashSet<Instruction> blockHeaders, HashSet<Instruction> entryHeaders)
        {
            int nextBlockId = 0;
            int currentBlockId = -1;
            Instruction currentBlockHdr = null;
            for (int i = 0; i < this.body.Instructions.Count; i++)
            {
                Instruction instr = this.body.Instructions[i];
                bool flag = blockHeaders.Contains(instr);
                if (flag)
                {
                    bool flag2 = currentBlockHdr != null;
                    if (flag2)
                    {
                        Instruction footer = this.body.Instructions[i - 1];
                        ControlFlowBlockType type = ControlFlowBlockType.Normal;
                        bool flag3 = entryHeaders.Contains(currentBlockHdr) || currentBlockHdr == this.body.Instructions[0];
                        if (flag3)
                        {
                            type |= ControlFlowBlockType.Entry;
                        }
                        bool flag4 = footer.OpCode.FlowControl == FlowControl.Return || footer.OpCode.FlowControl == FlowControl.Throw;
                        if (flag4)
                        {
                            type |= ControlFlowBlockType.Exit;
                        }
                        this.blocks.Add(new ControlFlowBlock(currentBlockId, type, currentBlockHdr, footer));
                    }
                    currentBlockId = nextBlockId++;
                    currentBlockHdr = instr;
                }
                this.instrBlocks[i] = currentBlockId;
            }
            bool flag5 = this.blocks.Count == 0 || this.blocks[this.blocks.Count - 1].Id != currentBlockId;
            if (flag5)
            {
                Instruction footer2 = this.body.Instructions[this.body.Instructions.Count - 1];
                ControlFlowBlockType type2 = ControlFlowBlockType.Normal;
                bool flag6 = entryHeaders.Contains(currentBlockHdr) || currentBlockHdr == this.body.Instructions[0];
                if (flag6)
                {
                    type2 |= ControlFlowBlockType.Entry;
                }
                bool flag7 = footer2.OpCode.FlowControl == FlowControl.Return || footer2.OpCode.FlowControl == FlowControl.Throw;
                if (flag7)
                {
                    type2 |= ControlFlowBlockType.Exit;
                }
                this.blocks.Add(new ControlFlowBlock(currentBlockId, type2, currentBlockHdr, footer2));
            }
        }

        private void LinkBlocks()
        {
            for (int i = 0; i < this.body.Instructions.Count; i++)
            {
                Instruction instr = this.body.Instructions[i];
                bool flag = instr.Operand is Instruction;
                if (flag)
                {
                    ControlFlowBlock srcBlock = this.blocks[this.instrBlocks[i]];
                    ControlFlowBlock dstBlock = this.blocks[this.instrBlocks[this.indexMap[(Instruction)instr.Operand]]];
                    dstBlock.Sources.Add(srcBlock);
                    srcBlock.Targets.Add(dstBlock);
                }
                else
                {
                    bool flag2 = instr.Operand is Instruction[];
                    if (flag2)
                    {
                        foreach (Instruction target in (Instruction[])instr.Operand)
                        {
                            ControlFlowBlock srcBlock2 = this.blocks[this.instrBlocks[i]];
                            ControlFlowBlock dstBlock2 = this.blocks[this.instrBlocks[this.indexMap[target]]];
                            dstBlock2.Sources.Add(srcBlock2);
                            srcBlock2.Targets.Add(dstBlock2);
                        }
                    }
                }
            }
            for (int j = 0; j < this.blocks.Count; j++)
            {
                bool flag3 = this.blocks[j].Footer.OpCode.FlowControl != FlowControl.Branch && this.blocks[j].Footer.OpCode.FlowControl != FlowControl.Return && this.blocks[j].Footer.OpCode.FlowControl != FlowControl.Throw;
                if (flag3)
                {
                    this.blocks[j].Targets.Add(this.blocks[j + 1]);
                    this.blocks[j + 1].Sources.Add(this.blocks[j]);
                }
            }
        }

        public static ControlFlowGraph Construct(CilBody body)
        {
            ControlFlowGraph graph = new ControlFlowGraph(body);
            bool flag = body.Instructions.Count == 0;
            ControlFlowGraph result;
            if (flag)
            {
                result = graph;
            }
            else
            {
                HashSet<Instruction> blockHeaders = new HashSet<Instruction>();
                HashSet<Instruction> entryHeaders = new HashSet<Instruction>();
                graph.PopulateBlockHeaders(blockHeaders, entryHeaders);
                graph.SplitBlocks(blockHeaders, entryHeaders);
                graph.LinkBlocks();
                result = graph;
            }
            return result;
        }

        private readonly List<ControlFlowBlock> blocks;

        private readonly CilBody body;

        private readonly int[] instrBlocks;

        private readonly Dictionary<Instruction, int> indexMap;
    }
}
