using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.MethodBlocks
{
    internal static class BlockParser
    {
        public static BlockParser.ScopeBlock ParseBody(CilBody body)
        {
            Dictionary<ExceptionHandler, Tuple<BlockParser.ScopeBlock, BlockParser.ScopeBlock, BlockParser.ScopeBlock>> ehScopes = new Dictionary<ExceptionHandler, Tuple<BlockParser.ScopeBlock, BlockParser.ScopeBlock, BlockParser.ScopeBlock>>();
            foreach (ExceptionHandler eh in body.ExceptionHandlers)
            {
                BlockParser.ScopeBlock tryBlock = new BlockParser.ScopeBlock(BlockParser.BlockType.Try, eh);
                BlockParser.BlockType handlerType = BlockParser.BlockType.Handler;
                bool flag = eh.HandlerType == ExceptionHandlerType.Finally;
                if (flag)
                {
                    handlerType = BlockParser.BlockType.Finally;
                }
                else
                {
                    bool flag2 = eh.HandlerType == ExceptionHandlerType.Fault;
                    if (flag2)
                    {
                        handlerType = BlockParser.BlockType.Fault;
                    }
                }
                BlockParser.ScopeBlock handlerBlock = new BlockParser.ScopeBlock(handlerType, eh);
                bool flag3 = eh.FilterStart != null;
                if (flag3)
                {
                    BlockParser.ScopeBlock filterBlock = new BlockParser.ScopeBlock(BlockParser.BlockType.Filter, eh);
                    ehScopes[eh] = Tuple.Create<BlockParser.ScopeBlock, BlockParser.ScopeBlock, BlockParser.ScopeBlock>(tryBlock, handlerBlock, filterBlock);
                }
                else
                {
                    ehScopes[eh] = Tuple.Create<BlockParser.ScopeBlock, BlockParser.ScopeBlock, BlockParser.ScopeBlock>(tryBlock, handlerBlock, null);
                }
            }
            BlockParser.ScopeBlock root = new BlockParser.ScopeBlock(BlockParser.BlockType.Normal, null);
            Stack<BlockParser.ScopeBlock> scopeStack = new Stack<BlockParser.ScopeBlock>();
            scopeStack.Push(root);
            foreach (Instruction instr in body.Instructions)
            {
                foreach (ExceptionHandler eh2 in body.ExceptionHandlers)
                {
                    Tuple<BlockParser.ScopeBlock, BlockParser.ScopeBlock, BlockParser.ScopeBlock> ehScope = ehScopes[eh2];
                    bool flag4 = instr == eh2.TryEnd;
                    if (flag4)
                    {
                        scopeStack.Pop();
                    }
                    bool flag5 = instr == eh2.HandlerEnd;
                    if (flag5)
                    {
                        scopeStack.Pop();
                    }
                    bool flag6 = eh2.FilterStart != null && instr == eh2.HandlerStart;
                    if (flag6)
                    {
                        Debug.Assert(scopeStack.Peek().Type == BlockParser.BlockType.Filter);
                        scopeStack.Pop();
                    }
                }
                foreach (ExceptionHandler eh3 in body.ExceptionHandlers.Reverse<ExceptionHandler>())
                {
                    Tuple<BlockParser.ScopeBlock, BlockParser.ScopeBlock, BlockParser.ScopeBlock> ehScope2 = ehScopes[eh3];
                    BlockParser.ScopeBlock parent = (scopeStack.Count > 0) ? scopeStack.Peek() : null;
                    bool flag7 = instr == eh3.TryStart;
                    if (flag7)
                    {
                        bool flag8 = parent != null;
                        if (flag8)
                        {
                            parent.Children.Add(ehScope2.Item1);
                        }
                        scopeStack.Push(ehScope2.Item1);
                    }
                    bool flag9 = instr == eh3.HandlerStart;
                    if (flag9)
                    {
                        bool flag10 = parent != null;
                        if (flag10)
                        {
                            parent.Children.Add(ehScope2.Item2);
                        }
                        scopeStack.Push(ehScope2.Item2);
                    }
                    bool flag11 = instr == eh3.FilterStart;
                    if (flag11)
                    {
                        bool flag12 = parent != null;
                        if (flag12)
                        {
                            parent.Children.Add(ehScope2.Item3);
                        }
                        scopeStack.Push(ehScope2.Item3);
                    }
                }
                BlockParser.ScopeBlock scope = scopeStack.Peek();
                BlockParser.InstrBlock block = scope.Children.LastOrDefault<BlockParser.BlockBase>() as BlockParser.InstrBlock;
                bool flag13 = block == null;
                if (flag13)
                {
                    scope.Children.Add(block = new BlockParser.InstrBlock());
                }
                block.Instructions.Add(instr);
            }
            foreach (ExceptionHandler eh4 in body.ExceptionHandlers)
            {
                bool flag14 = eh4.TryEnd == null;
                if (flag14)
                {
                    scopeStack.Pop();
                }
                bool flag15 = eh4.HandlerEnd == null;
                if (flag15)
                {
                    scopeStack.Pop();
                }
            }
            Debug.Assert(scopeStack.Count == 1);
            return root;
        }

        internal abstract class BlockBase
        {
            public BlockBase(BlockParser.BlockType type)
            {
                this.Type = type;
            }

            // (set) Token: 0x060000F9 RID: 249 RVA: 0x0000D2A8 File Offset: 0x0000B4A8
            public BlockParser.BlockType Type { get; private set; }

            public abstract void ToBody(CilBody body);
        }

        internal enum BlockType
        {
            Normal,
            Try,
            Handler,
            Finally,
            Filter,
            Fault
        }

        internal class ScopeBlock : BlockParser.BlockBase
        {
            public ScopeBlock(BlockParser.BlockType type, ExceptionHandler handler) : base(type)
            {
                this.Handler = handler;
                this.Children = new List<BlockParser.BlockBase>();
            }

            // (set) Token: 0x060000FD RID: 253 RVA: 0x0000D2D8 File Offset: 0x0000B4D8
            public ExceptionHandler Handler { get; private set; }

            // (set) Token: 0x060000FF RID: 255 RVA: 0x0000D2E9 File Offset: 0x0000B4E9
            public List<BlockParser.BlockBase> Children { get; set; }

            public override string ToString()
            {
                StringBuilder ret = new StringBuilder();
                bool flag = base.Type == BlockParser.BlockType.Try;
                if (flag)
                {
                    ret.Append("try ");
                }
                else
                {
                    bool flag2 = base.Type == BlockParser.BlockType.Handler;
                    if (flag2)
                    {
                        ret.Append("handler ");
                    }
                    else
                    {
                        bool flag3 = base.Type == BlockParser.BlockType.Finally;
                        if (flag3)
                        {
                            ret.Append("finally ");
                        }
                        else
                        {
                            bool flag4 = base.Type == BlockParser.BlockType.Fault;
                            if (flag4)
                            {
                                ret.Append("fault ");
                            }
                        }
                    }
                }
                ret.AppendLine("{");
                foreach (BlockParser.BlockBase child in this.Children)
                {
                    ret.Append(child);
                }
                ret.AppendLine("}");
                return ret.ToString();
            }

            public Instruction GetFirstInstr()
            {
                BlockParser.BlockBase firstBlock = this.Children.First<BlockParser.BlockBase>();
                bool flag = firstBlock is BlockParser.ScopeBlock;
                Instruction result;
                if (flag)
                {
                    result = ((BlockParser.ScopeBlock)firstBlock).GetFirstInstr();
                }
                else
                {
                    result = ((BlockParser.InstrBlock)firstBlock).Instructions.First<Instruction>();
                }
                return result;
            }

            public Instruction GetLastInstr()
            {
                BlockParser.BlockBase firstBlock = this.Children.Last<BlockParser.BlockBase>();
                bool flag = firstBlock is BlockParser.ScopeBlock;
                Instruction result;
                if (flag)
                {
                    result = ((BlockParser.ScopeBlock)firstBlock).GetLastInstr();
                }
                else
                {
                    result = ((BlockParser.InstrBlock)firstBlock).Instructions.Last<Instruction>();
                }
                return result;
            }

            public override void ToBody(CilBody body)
            {
                bool flag = base.Type > BlockParser.BlockType.Normal;
                if (flag)
                {
                    bool flag2 = base.Type == BlockParser.BlockType.Try;
                    if (flag2)
                    {
                        this.Handler.TryStart = this.GetFirstInstr();
                        this.Handler.TryEnd = this.GetLastInstr();
                    }
                    else
                    {
                        bool flag3 = base.Type == BlockParser.BlockType.Filter;
                        if (flag3)
                        {
                            this.Handler.FilterStart = this.GetFirstInstr();
                        }
                        else
                        {
                            this.Handler.HandlerStart = this.GetFirstInstr();
                            this.Handler.HandlerEnd = this.GetLastInstr();
                        }
                    }
                }
                foreach (BlockParser.BlockBase block in this.Children)
                {
                    block.ToBody(body);
                }
            }
        }

        internal class InstrBlock : BlockParser.BlockBase
        {
            public InstrBlock() : base(BlockParser.BlockType.Normal)
            {
                this.Instructions = new List<Instruction>();
            }

            // (set) Token: 0x06000106 RID: 262 RVA: 0x0000D57F File Offset: 0x0000B77F
            public List<Instruction> Instructions { get; set; }

            public override string ToString()
            {
                StringBuilder ret = new StringBuilder();
                foreach (Instruction instr in this.Instructions)
                {
                    ret.AppendLine(instr.ToString());
                }
                return ret.ToString();
            }

            public override void ToBody(CilBody body)
            {
                foreach (Instruction instr in this.Instructions)
                {
                    body.Instructions.Add(instr);
                }
            }
        }
    }
}
