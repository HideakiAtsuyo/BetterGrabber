using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace FVM.Runtime
{
    public static class VMRuntime
    {
        public static void Initialize(string resource)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream(resource);
            MemoryStream memStream = new MemoryStream();
            stream.CopyTo(memStream);
            VMRuntime.Reader = new BinaryReader(memStream);
            VMRuntime.OpCodeList = new Dictionary<short, OpCode>();
            foreach (FieldInfo f in typeof(OpCodes).GetFields())
            {
                bool flag = f.FieldType == typeof(OpCode);
                if (flag)
                {
                    OpCode opCode = (OpCode)f.GetValue(null);
                    VMRuntime.OpCodeList.Add(opCode.Value, opCode);
                }
            }
        }

        public static object Execute(int offset, object[] parameters)
        {
            VMRuntime.Reader.BaseStream.Position = (long)offset;
            Module module = typeof(VMRuntime).Module;
            int token = VMRuntime.Reader.ReadInt32();
            MethodBase method = module.ResolveMethod(token);
            int instrs = VMRuntime.Reader.ReadInt32();
            int targets = VMRuntime.Reader.ReadInt32();
            ParameterInfo[] methodParams = method.GetParameters();
            IList<LocalVariableInfo> methodLocals = method.GetMethodBody().LocalVariables;
            Dictionary<int, Label> labels = new Dictionary<int, Label>();
            List<LocalBuilder> dynLocals = new List<LocalBuilder>();
            Type[] dynParams = new Type[methodParams.Length];
            int loopIndex = 0;
            bool flag = !method.IsStatic;
            if (flag)
            {
                loopIndex = 1;
                dynParams = new Type[methodParams.Length + 1];
                dynParams[0] = method.DeclaringType;
            }
            while (loopIndex < methodParams.Length)
            {
                dynParams[loopIndex] = methodParams[loopIndex].ParameterType;
                loopIndex++;
            }
            Type returnType = (method as MethodInfo).ReturnType;
            DynamicMethod dynMethod = new DynamicMethod("", returnType, dynParams, true);
            ILGenerator il = dynMethod.GetILGenerator();
            for (int i = 0; i < methodLocals.Count; i++)
            {
                dynLocals.Add(il.DeclareLocal(methodLocals[i].LocalType, methodLocals[i].IsPinned));
            }
            for (int j = 0; j < targets; j++)
            {
                labels.Add(VMRuntime.Reader.ReadInt32(), il.DefineLabel());
            }
            for (int k = 0; k < instrs; k++)
            {
                bool flag2 = labels.ContainsKey(k);
                if (flag2)
                {
                    il.MarkLabel(labels[k]);
                }
                int exceptionLoop = VMRuntime.Reader.ReadInt32();
                int e = 0;
                while (e < exceptionLoop)
                {
                    switch (VMRuntime.Reader.ReadInt32())
                    {
                        case 0:
                            il.BeginExceptionBlock();
                            break;
                        case 1:
                            il.BeginExceptFilterBlock();
                            break;
                        case 2:
                            {
                                int catchTypeToken = VMRuntime.Reader.ReadInt32();
                                bool flag3 = catchTypeToken == -1;
                                if (flag3)
                                {
                                    il.BeginCatchBlock(typeof(object));
                                }
                                else
                                {
                                    Type catchType = module.ResolveType(catchTypeToken);
                                    il.BeginCatchBlock(catchType);
                                }
                                break;
                            }
                        case 3:
                            il.BeginFinallyBlock();
                            break;
                        case 4:
                            il.BeginFaultBlock();
                            break;
                        case 5:
                            il.EndExceptionBlock();
                            break;
                    }
                IL_254:
                    e++;
                    continue;
                    goto IL_254;
                }
                short s = VMRuntime.Reader.ReadInt16();
                OpCode opCode = VMRuntime.OpCodeList[s];
                switch (VMRuntime.Reader.ReadInt32())
                {
                    case 0:
                        il.Emit(opCode);
                        break;
                    case 1:
                        il.Emit(opCode, VMRuntime.Reader.ReadString());
                        break;
                    case 2:
                        il.Emit(opCode, VMRuntime.Reader.ReadDouble());
                        break;
                    case 3:
                        il.Emit(opCode, VMRuntime.Reader.ReadInt64());
                        break;
                    case 4:
                        il.Emit(opCode, VMRuntime.Reader.ReadInt32());
                        break;
                    case 5:
                        il.Emit(opCode, VMRuntime.Reader.ReadSingle());
                        break;
                    case 6:
                        il.Emit(opCode, VMRuntime.Reader.ReadByte());
                        break;
                    case 7:
                        {
                            int typeToken = VMRuntime.Reader.ReadInt32();
                            Type type = module.ResolveType(typeToken);
                            il.Emit(opCode, type);
                            break;
                        }
                    case 8:
                        {
                            int fieldToken = VMRuntime.Reader.ReadInt32();
                            FieldInfo field = module.ResolveField(fieldToken);
                            il.Emit(opCode, field);
                            break;
                        }
                    case 9:
                        {
                            int methodToken = VMRuntime.Reader.ReadInt32();
                            MethodBase cmethod = module.ResolveMethod(methodToken);
                            MethodInfo mInfo = cmethod as MethodInfo;
                            bool flag4 = mInfo != null;
                            if (flag4)
                            {
                                il.Emit(opCode, mInfo);
                            }
                            else
                            {
                                il.Emit(opCode, cmethod as ConstructorInfo);
                            }
                            break;
                        }
                    case 10:
                        {
                            int tokToken = VMRuntime.Reader.ReadInt32();
                            switch (VMRuntime.Reader.ReadInt32())
                            {
                                case 0:
                                    {
                                        FieldInfo tokRField = module.ResolveField(tokToken);
                                        il.Emit(opCode, tokRField);
                                        break;
                                    }
                                case 1:
                                    {
                                        Type tokRType = module.ResolveType(tokToken);
                                        il.Emit(opCode, tokRType);
                                        break;
                                    }
                                case 2:
                                    {
                                        MethodBase tokRMethod = module.ResolveMethod(tokToken);
                                        MethodInfo info = tokRMethod as MethodInfo;
                                        bool flag5 = info != null;
                                        if (flag5)
                                        {
                                            il.Emit(opCode, info);
                                        }
                                        else
                                        {
                                            il.Emit(opCode, tokRMethod as ConstructorInfo);
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    case 11:
                        {
                            int target = VMRuntime.Reader.ReadInt32();
                            Label label = labels[target];
                            il.Emit(opCode, label);
                            break;
                        }
                    case 12:
                        {
                            int index = VMRuntime.Reader.ReadInt32();
                            int indexType = VMRuntime.Reader.ReadInt32();
                            int num = indexType;
                            int num2 = num;
                            if (num2 != 0)
                            {
                                if (num2 == 1)
                                {
                                    il.Emit(opCode, index);
                                }
                            }
                            else
                            {
                                il.Emit(opCode, dynLocals[index]);
                            }
                            break;
                        }
                    case 13:
                        {
                            int switchCount = VMRuntime.Reader.ReadInt32();
                            Label[] switchLabels = new Label[switchCount];
                            for (int u = 0; u < switchCount; u++)
                            {
                                switchLabels[u] = labels[VMRuntime.Reader.ReadInt32()];
                            }
                            il.Emit(opCode, switchLabels);
                            break;
                        }
                }
            }
            return dynMethod.Invoke(null, parameters);
        }

        public static BinaryReader Reader;

        public static Dictionary<short, OpCode> OpCodeList;
    }
}
