using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Linq;

namespace BetterGrabber_Builder.JustForFun
{
    internal class IntsLmao
    {
        internal static void Execute(ModuleDef mod, string[] excluded)
        {
            MethodDefUser methodDefUser = new MethodDefUser("InitializeInts", MethodSig.CreateStatic(mod.CorLibTypes.Void), MethodImplAttributes.IL, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static);
            methodDefUser.Body = new CilBody();
            mod.GlobalType.Methods.Add(methodDefUser);
            var globalTypeCctor = mod.GlobalType.FindOrCreateStaticConstructor();

            foreach (var type in mod.GetTypes().Where(x => !x.IsGlobalModuleType))
            {
                //var cctor = type.FindOrCreateStaticConstructor();

                var cctor = mod.GlobalType.FindOrCreateStaticConstructor();

                foreach (var method in type.Methods.Where(x => x.HasBody && x.Body.HasInstructions && !x.DeclaringType.IsGlobalModuleType && !x.IsConstructor && !excluded.arrayContains(x.Name)))
                {
                    for (var i = 0; i < method.Body.Instructions.Count(); i++)
                    {
                        if (method.Body.Instructions[i].Operand != null)
                        {
                            if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4)
                            {
                                string name = String.Format("Omg_Int_{0}", Helpers.Others.RandomString(16));

                                var field = new FieldDefUser(name, new FieldSig(mod.CorLibTypes.Int32), FieldAttributes.Public | FieldAttributes.Static);
                                type.Fields.Add(field);

                                //cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Stsfld, field));
                                //cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldc_I4, int.Parse(method.Body.Instructions[i].Operand.ToString())));
                                methodDefUser.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Stsfld, field));
                                methodDefUser.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldc_I4, int.Parse(method.Body.Instructions[i].Operand.ToString())));
                                Console.WriteLine(String.Format("Successfully processed int {0}", method.Body.Instructions[i].Operand));

                                method.Body.Instructions[i].OpCode = OpCodes.Ldsfld;
                                method.Body.Instructions[i].Operand = field;
                            }
                        }
                    }
                    method.Body.SimplifyBranches();
                    method.Body.OptimizeBranches();
                }
            }
            methodDefUser.Body.Instructions.Insert(methodDefUser.Body.Instructions.Count, Instruction.Create(OpCodes.Ret));
            globalTypeCctor.Body.Instructions.Insert(globalTypeCctor.Body.Instructions.Count, OpCodes.Call.ToInstruction(methodDefUser));
        }
    }
}