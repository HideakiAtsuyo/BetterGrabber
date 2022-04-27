using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterGrabber_Builder.JustForFun
{
    internal class StringsLmao
    {
        internal static void Execute(ModuleDef mod, string[] excluded)
        {
            MethodDefUser methodDefUser = new MethodDefUser("InitializeStrings", MethodSig.CreateStatic(mod.CorLibTypes.Void), MethodImplAttributes.IL, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static);
            methodDefUser.Body = new CilBody();
            mod.GlobalType.Methods.Add(methodDefUser);
            var globalTypeCctor = mod.GlobalType.FindOrCreateStaticConstructor();

            ModuleDefMD typeModule = ModuleDefMD.Load(typeof(StringsLmao).Module);
            TypeDef typeDef = typeModule.ResolveTypeDef(MDToken.ToRID(typeof(Runtime.StringRuntime).MetadataToken));
            IEnumerable<IDnlibDef> members = Helpers.InjectHelper.Inject(typeDef, mod.GlobalType, mod);
            MethodDef init = (MethodDef)members.Single(method => method.Name == "OhMyGod");

            foreach (IDnlibDef member in members)
                member.Name = String.Format("Omg_No_{0}", Helpers.Others.RandomString(16));

            foreach (var type in mod.GetTypes().Where(x => !x.IsGlobalModuleType))
            {
                //var cctor = type.FindOrCreateStaticConstructor();
                var cctor = mod.GlobalType.FindOrCreateStaticConstructor();
                foreach (var method in type.Methods.Where(x => x.HasBody && x.Body.HasInstructions && !x.DeclaringType.IsGlobalModuleType && !x.IsConstructor && !excluded.arrayContains(x.Name)))
                {
                    for (var i = 0; i < method.Body.Instructions.Count(); i++)
                    {
                        if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                        {
                            string name = String.Format("Omg_Str_{0}", Helpers.Others.RandomString(16));
                            int key = Helpers.Others.rnd.Next(1337, 7331);
                            string newString = Runtime.StringRuntime.OhMyGod(method.Body.Instructions[i].Operand.ToString(), key);

                            var field = new FieldDefUser(name, new FieldSig(mod.CorLibTypes.String), FieldAttributes.Public | FieldAttributes.Static);
                            type.Fields.Add(field);

                            //cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Stsfld, field));
                            //cctor.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldstr, newString));
                            methodDefUser.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Stsfld, field));
                            methodDefUser.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldstr, newString));
                            Console.WriteLine(String.Format("Successfully processed string {0}", method.Body.Instructions[i].Operand));

                            method.Body.Instructions[i].OpCode = OpCodes.Ldsfld;
                            method.Body.Instructions[i].Operand = field;

                            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(key));
                            method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(init));
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
