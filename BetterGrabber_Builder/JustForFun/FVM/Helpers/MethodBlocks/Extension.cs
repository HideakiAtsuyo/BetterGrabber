// FVM.Helpers.MethodBlocks.Extension
using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using FVM.Helpers.MethodBlocks;

public static class Extension
{
    public static List<Block> GetBlocks(this MethodDef method)
    {
        List<Block> blocks = new List<Block>();
        CilBody body = method.Body;
        body.SimplifyBranches();
        BlockParser.ScopeBlock root = BlockParser.ParseBody(body);
        List<Instruction> branchesInstructions = new List<Instruction>();
        IList<ExceptionHandler> exceptions = method.Body.ExceptionHandlers;
        int exceptionsCount = 0;
        Trace trace = new Trace(body, method.ReturnType.RemoveModifiers().ElementType != ElementType.Void);
        foreach (BlockParser.InstrBlock allBlock in GetAllBlocks(root))
        {
            BlockParser.InstrBlock block = allBlock;
            LinkedList<Instruction[]> statements = SplitStatements(block, trace);
            foreach (Instruction[] statement in statements)
            {
                HashSet<Instruction> statementLast = new HashSet<Instruction>(statements.Select((Instruction[] st) => st.Last()));
                int finished = 0;
                int finishedExceptions = 0;
                Instruction[] array = statement;
                foreach (Instruction instr2 in array)
                {
                    if (instr2.Operand is Instruction)
                    {
                        branchesInstructions.Add(instr2.Operand as Instruction);
                    }
                    if (branchesInstructions.Contains(instr2))
                    {
                        branchesInstructions.Remove(instr2);
                        finished++;
                    }
                    if (exceptions.Count <= 1)
                    {
                        if (exceptions.Any((ExceptionHandler x) => x.TryStart == instr2))
                        {
                            exceptionsCount++;
                        }
                        if (exceptions.Any((ExceptionHandler x) => x.HandlerEnd == instr2))
                        {
                            finishedExceptions++;
                            exceptionsCount--;
                        }
                    }
                }
                bool isInException = exceptionsCount > 0 || finishedExceptions > 0;
                bool isInBranch = branchesInstructions.Count > 0 || finished > 0;
                blocks.Add(new Block(statement, isInException, !hasUnknownSource(statement), isInBranch));
                bool hasUnknownSource(IList<Instruction> instrs)
                {
                    return instrs.Any(delegate (Instruction instr)
                    {
                        if (trace.HasMultipleSources(instr.Offset))
                        {
                            return true;
                        }
                        if (trace.BrRefs.TryGetValue(instr.Offset, out var value))
                        {
                            if (value.Any((Instruction src) => src.Operand is Instruction[]))
                            {
                                return true;
                            }
                            if (value.Any((Instruction src) => src.Offset <= statements.First.Value.Last().Offset || src.Offset >= block.Instructions.Last().Offset))
                            {
                                return true;
                            }
                            if (value.Any((Instruction src) => statementLast.Contains(src)))
                            {
                                return true;
                            }
                        }
                        return false;
                    });
                }
            }
        }
        return blocks;
    }

    private static LinkedList<Instruction[]> SplitStatements(BlockParser.InstrBlock block, Trace trace)
    {
        LinkedList<Instruction[]> statements = new LinkedList<Instruction[]>();
        List<Instruction> currentStatement = new List<Instruction>();
        HashSet<Instruction> requiredInstr = new HashSet<Instruction>();
        for (int i = 0; i < block.Instructions.Count; i++)
        {
            Instruction instr = block.Instructions[i];
            currentStatement.Add(instr);
            bool shouldSplit = i + 1 < block.Instructions.Count && trace.HasMultipleSources(block.Instructions[i + 1].Offset);
            FlowControl flowControl = instr.OpCode.FlowControl;
            FlowControl flowControl2 = flowControl;
            if (flowControl2 == FlowControl.Branch || flowControl2 == FlowControl.Cond_Branch || (uint)(flowControl2 - 7) <= 1u)
            {
                shouldSplit = true;
                if (trace.AfterStack[instr.Offset] != 0)
                {
                    if (instr.Operand is Instruction targetInstr)
                    {
                        requiredInstr.Add(targetInstr);
                    }
                    else if (instr.Operand is Instruction[] targetInstrs)
                    {
                        Instruction[] array = targetInstrs;
                        foreach (Instruction target in array)
                        {
                            requiredInstr.Add(target);
                        }
                    }
                }
            }
            requiredInstr.Remove(instr);
            if (instr.OpCode.OpCodeType != OpCodeType.Prefix && trace.AfterStack[instr.Offset] == 0 && requiredInstr.Count == 0 && (shouldSplit || 90.0 > new Random().NextDouble()) && (i == 0 || block.Instructions[i - 1].OpCode.Code != Code.Tailcall))
            {
                statements.AddLast(currentStatement.ToArray());
                currentStatement.Clear();
            }
        }
        if (currentStatement.Count > 0)
        {
            statements.AddLast(currentStatement.ToArray());
        }
        return statements;
    }

    private static IEnumerable<BlockParser.InstrBlock> GetAllBlocks(BlockParser.ScopeBlock scope)
    {
        foreach (BlockParser.BlockBase child in scope.Children)
        {
            if (child is BlockParser.InstrBlock)
            {
                yield return (BlockParser.InstrBlock)child;
                continue;
            }
            foreach (BlockParser.InstrBlock allBlock in GetAllBlocks((BlockParser.ScopeBlock)child))
            {
                yield return allBlock;
            }
        }
    }
}
