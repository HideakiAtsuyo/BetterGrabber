using System;
using System.Collections.Generic;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.DynConverter
{
    public static class Emitter
    {
        public static void EmitNone(this BinaryWriter writer)
        {
            writer.Write(0);
        }

        public static void EmitString(this BinaryWriter writer, Instruction instr)
        {
            writer.Write(1);
            writer.Write(instr.Operand.ToString());
        }

        public static void EmitR(this BinaryWriter writer, Instruction instr)
        {
            writer.Write(2);
            writer.Write((double)instr.Operand);
        }

        public static void EmitI8(this BinaryWriter writer, Instruction instr)
        {
            writer.Write(3);
            writer.Write((long)instr.Operand);
        }

        public static void EmitI(this BinaryWriter writer, Instruction instr)
        {
            writer.Write(4);
            writer.Write(instr.GetLdcI4Value());
        }

        public static void EmitShortR(this BinaryWriter writer, Instruction instr)
        {
            writer.Write(5);
            writer.Write((float)instr.Operand);
        }

        public static void EmitShortI(this BinaryWriter writer, Instruction instr)
        {
            writer.Write(6);
            writer.Write((byte)instr.GetLdcI4Value());
        }

        public static void EmitType(this BinaryWriter writer, Instruction instr)
        {
            writer.Write(7);
            ITypeDefOrRef typeDeforRef = instr.Operand as ITypeDefOrRef;
            writer.Write(typeDeforRef.MDToken.ToInt32());
        }

        public static void EmitField(this BinaryWriter writer, Instruction instr)
        {
            writer.Write(8);
            IField field = instr.Operand as IField;
            writer.Write(field.MDToken.ToInt32());
        }

        public static void EmitMethod(this BinaryWriter writer, Instruction instr)
        {
            writer.Write(9);
            MethodSpec spec = instr.Operand as MethodSpec;
            bool flag = spec != null;
            if (flag)
            {
                writer.Write(spec.MDToken.ToInt32());
            }
            else
            {
                IMethodDefOrRef method = instr.Operand as IMethodDefOrRef;
                writer.Write(method.MDToken.ToInt32());
            }
        }

        public static void EmitTok(this BinaryWriter writer, Instruction instr)
        {
            writer.Write(10);
            object operand = instr.Operand;
            IField field = operand as IField;
            bool flag = field != null;
            if (flag)
            {
                writer.Write(field.MDToken.ToInt32());
                writer.Write(0);
            }
            else
            {
                ITypeDefOrRef type = operand as ITypeDefOrRef;
                bool flag2 = type != null;
                if (flag2)
                {
                    writer.Write(type.MDToken.ToInt32());
                    writer.Write(1);
                }
                else
                {
                    writer.Write((operand as IMethodDefOrRef).MDToken.ToInt32());
                    writer.Write(2);
                }
            }
        }

        public static void EmitBr(this BinaryWriter writer, int index)
        {
            writer.Write(11);
            writer.Write(index);
        }

        public static void EmitVar(this BinaryWriter writer, Instruction instr)
        {
            writer.Write(12);
            Local local = instr.Operand as Local;
            bool flag = local != null;
            if (flag)
            {
                writer.Write(local.Index);
                writer.Write(0);
            }
            else
            {
                Parameter param = instr.Operand as Parameter;
                bool flag2 = param != null;
                if (!flag2)
                {
                    throw new NotSupportedException();
                }
                writer.Write(param.Index);
                writer.Write(1);
            }
        }

        public static void EmitSwitch(this BinaryWriter writer, List<Instruction> instrs, Instruction instr)
        {
            writer.Write(13);
            Instruction[] instructions = instr.Operand as Instruction[];
            writer.Write(instructions.Length);
            foreach (Instruction i in instructions)
            {
                int index = instrs.IndexOf(i);
                writer.Write(index);
            }
        }
    }
}
