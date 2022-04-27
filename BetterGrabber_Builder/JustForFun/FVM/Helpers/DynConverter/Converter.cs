using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.DynConverter
{
    public class Converter
    {
        public MethodDef Method { get; }

        public BinaryWriter Writer { get; }

        public Converter(MethodDef method, BinaryWriter writer)
        {
            this.Method = method;
            this.Writer = writer;
            method.Body.SimplifyMacros(method.Parameters);
            method.Body.SimplifyBranches();
        }

        public void ConvertToBytes()
        {
            ExceptionMapper mapper = new ExceptionMapper(this.Method);
            IList<Instruction> instrs = this.Method.Body.Instructions;
            int count = instrs.Count;
            List<int> targets = new List<int>();
            this.Writer.Write(count);
            int i = 0;
            while (i < count)
            {
                OperandType type = instrs[i].OpCode.OperandType;
                OperandType operandType = type;
                OperandType operandType2 = operandType;
                if (operandType2 == OperandType.InlineBrTarget)
                {
                    goto IL_73;
                }
                if (operandType2 != OperandType.InlineSwitch)
                {
                    if (operandType2 == OperandType.ShortInlineBrTarget)
                    {
                        goto IL_73;
                    }
                }
                else
                {
                    Instruction[] operand = instrs[i].Operand as Instruction[];
                    foreach (Instruction ins in operand)
                    {
                        targets.Add(instrs.IndexOf(ins));
                    }
                }
            IL_D8:
                i++;
                continue;
            IL_73:
                targets.Add(instrs.IndexOf(instrs[i].Operand as Instruction));
                goto IL_D8;
            }
            this.Writer.Write(targets.Count);
            foreach (int target in targets)
            {
                this.Writer.Write(target);
            }
            for (int j = 0; j < count; j++)
            {
                Instruction instr = instrs[j];
                short value = instr.OpCode.Value;
                OperandType type2 = instr.OpCode.OperandType;
                object operand2 = instr.Operand;
                mapper.MapAndWrite(this.Writer, instr);
                this.Writer.Write(value);
                switch (type2)
                {
                    case OperandType.InlineBrTarget:
                    case OperandType.ShortInlineBrTarget:
                        this.Writer.EmitBr(instrs.IndexOf(operand2 as Instruction));
                        break;
                    case OperandType.InlineField:
                        this.Writer.EmitField(instr);
                        break;
                    case OperandType.InlineI:
                        this.Writer.EmitI(instr);
                        break;
                    case OperandType.InlineI8:
                        this.Writer.EmitI8(instr);
                        break;
                    case OperandType.InlineMethod:
                        this.Writer.EmitMethod(instr);
                        break;
                    case OperandType.InlineNone:
                        this.Writer.EmitNone();
                        break;
                    case OperandType.InlineR:
                        this.Writer.EmitR(instr);
                        break;
                    case OperandType.InlineString:
                        this.Writer.EmitString(instr);
                        break;
                    case OperandType.InlineSwitch:
                        this.Writer.EmitSwitch(instrs.ToList<Instruction>(), instr);
                        break;
                    case OperandType.InlineTok:
                        this.Writer.EmitTok(instr);
                        break;
                    case OperandType.InlineType:
                        this.Writer.EmitType(instr);
                        break;
                    case OperandType.InlineVar:
                    case OperandType.ShortInlineVar:
                        this.Writer.EmitVar(instr);
                        break;
                    case OperandType.ShortInlineI:
                        this.Writer.EmitShortI(instr);
                        break;
                    case OperandType.ShortInlineR:
                        this.Writer.EmitShortR(instr);
                        break;
                }
            }
        }

        private class ExceptionInfo
        {
            public int Type { get; }

            public int Action { get; }

            public ExceptionInfo(int type, int action)
            {
                this.Type = type;
                this.Action = action;
            }
        }
    }
}
