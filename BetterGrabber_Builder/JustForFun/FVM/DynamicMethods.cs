using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using FVM.Helpers.DynConverter;
using FVM.Runtime;
using BetterGrabber_Builder;

namespace FVM.Protections.VM
{
    public class DynamicMethods
    {
        string[] methods = new string[] { "InitializeStrings" };
        public void Execute(ModuleDefMD mod, string[] excluded)
        {
            MethodDefUser methodDefUser = new MethodDefUser("InitializeDM", MethodSig.CreateStatic(mod.CorLibTypes.Void), MethodImplAttributes.IL, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static);
            methodDefUser.Body = new CilBody();
            mod.GlobalType.Methods.Add(methodDefUser);

            Injector injector = new Injector(mod, typeof(VMRuntime), true);
            MethodDef executeCall = injector.FindMember("Execute") as MethodDef;
            MethodDef initCall = injector.FindMember("Initialize") as MethodDef;
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            foreach (TypeDef type in mod.GetTypes())
            {
                foreach (MethodDef method in type.Methods.Where(x => !excluded.arrayContains(x.Name)))
                {
                    if (methods.Contains(method.Name.ToString()) || !method.DeclaringType.IsGlobalModuleType)
                    {
                        if (!method.IsConstructor)
                        {
                            bool flag = !method.HasBody;
                            if (!flag)
                            {
                                bool flag2 = !method.Body.HasInstructions;
                                if (!flag2)
                                {
                                    bool hasExceptionHandlers = method.Body.HasExceptionHandlers;
                                    if (!hasExceptionHandlers)
                                    {
                                        int pos = (int)writer.BaseStream.Position;
                                        int token = method.MDToken.ToInt32();
                                        writer.Write(token);
                                        writer.ConvertToBytes(method);
                                        this.SetUpMethod(method, pos, mod, executeCall);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            string resourceName = "HelloWorld";
            EmbeddedResource resource = new EmbeddedResource(resourceName, stream.ToArray(), ManifestResourceAttributes.Public);
            mod.Resources.Add(resource);

            MethodDef cctor = methodDefUser;
            IList<Instruction> instrs = cctor.Body.Instructions;
            instrs.Add(OpCodes.Ldstr.ToInstruction(resourceName));
            instrs.Add(OpCodes.Call.ToInstruction(initCall));
            instrs.Add(OpCodes.Ret.ToInstruction());

            MethodDef globaalCctor = mod.GlobalType.FindOrCreateStaticConstructor();
            globaalCctor.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(cctor));
        }

        public void SetUpMethod(MethodDef meth, int pos, ModuleDefMD module, MethodDef executeCall)
        {
            bool containsOut = false;
            meth.Body.Instructions.Clear();
            IEnumerable<Parameter> rrr = from i in meth.Parameters where i.Type.FullName.EndsWith("&") select i;
            bool flag = rrr.Count<Parameter>() != 0;
            if (flag)
            {
                containsOut = true;
            }
            SZArraySig rrg = module.CorLibTypes.Object.ToSZArraySig();
            Local loc2 = new Local(new SZArraySig(module.CorLibTypes.Object));
            CilBody cli = new CilBody();
            foreach (Local bodyVariable in meth.Body.Variables)
            {
                cli.Variables.Add(bodyVariable);
            }
            cli.Variables.Add(loc2);
            List<Local> outParams = new List<Local>();
            Dictionary<Parameter, Local> testerDictionary = new Dictionary<Parameter, Local>();
            bool flag2 = containsOut;
            if (flag2)
            {
                foreach (Parameter parameter in rrr)
                {
                    Local locf = new Local(parameter.Type.Next);
                    testerDictionary.Add(parameter, locf);
                    cli.Variables.Add(locf);
                }
            }
            if (meth.Parameters.Count > 0)
            {
                int outp = 0;
                cli.Instructions.Add(new Instruction(OpCodes.Ldc_I4, meth.Parameters.Count));
                cli.Instructions.Add(new Instruction(OpCodes.Newarr, module.CorLibTypes.Object.ToTypeDefOrRef()));
                for (int j = 0; j < meth.Parameters.Count; j++)
                {
                    Parameter par = meth.Parameters[j];
                    cli.Instructions.Add(new Instruction(OpCodes.Dup));
                    cli.Instructions.Add(new Instruction(OpCodes.Ldc_I4, j));
                    bool flag3 = containsOut;
                    if (flag3)
                    {
                        bool flag4 = rrr.Contains(meth.Parameters[j]);
                        if (flag4)
                        {
                            cli.Instructions.Add(new Instruction(OpCodes.Ldloc, testerDictionary[meth.Parameters[j]]));
                            outp++;
                        }
                        else
                        {
                            cli.Instructions.Add(new Instruction(OpCodes.Ldarg, meth.Parameters[j]));
                        }
                    }
                    else
                    {
                        cli.Instructions.Add(new Instruction(OpCodes.Ldarg, meth.Parameters[j]));
                    }
                    cli.Instructions.Add(par.Type.FullName.EndsWith("&") ? new Instruction(OpCodes.Box, par.Type.Next.ToTypeDefOrRef()) : new Instruction(OpCodes.Box, par.Type.ToTypeDefOrRef()));
                    cli.Instructions.Add(new Instruction(OpCodes.Stelem_Ref));
                }
                cli.Instructions.Add(new Instruction(OpCodes.Stloc, loc2));
                cli.Instructions.Add(new Instruction(OpCodes.Ldc_I4, pos));
                cli.Instructions.Add(new Instruction(OpCodes.Ldloc, loc2));
            }
            else
            {
                cli.Instructions.Add(new Instruction(OpCodes.Ldc_I4, pos));
                cli.Instructions.Add(new Instruction(OpCodes.Ldnull));
            }
            cli.Instructions.Add(Instruction.Create(OpCodes.Call, executeCall));
            bool flag5 = meth.ReturnType.ElementType == ElementType.Void;
            if (flag5)
            {
                cli.Instructions.Add(Instruction.Create(OpCodes.Pop));
            }
            else
            {
                bool isValueType = meth.ReturnType.IsValueType;
                if (isValueType)
                {
                    cli.Instructions.Add(Instruction.Create(OpCodes.Unbox_Any, meth.ReturnType.ToTypeDefOrRef()));
                }
                else
                {
                    cli.Instructions.Add(Instruction.Create(OpCodes.Castclass, meth.ReturnType.ToTypeDefOrRef()));
                }
            }
            bool flag6 = containsOut;
            if (flag6)
            {
                foreach (Parameter parameter2 in rrr)
                {
                    cli.Instructions.Add(new Instruction(OpCodes.Ldarg, parameter2));
                    cli.Instructions.Add(new Instruction(OpCodes.Ldloc, loc2));
                    cli.Instructions.Add(new Instruction(OpCodes.Ldc_I4, meth.Parameters.IndexOf(parameter2)));
                    cli.Instructions.Add(new Instruction(OpCodes.Ldelem_Ref));
                    cli.Instructions.Add(new Instruction(OpCodes.Unbox_Any, parameter2.Type.Next.ToTypeDefOrRef()));
                    cli.Instructions.Add(new Instruction(OpCodes.Stind_Ref));
                }
                cli.Instructions.Add(new Instruction(OpCodes.Ret));
            }
            else
            {
                cli.Instructions.Add(new Instruction(OpCodes.Ret));
            }
            meth.Body = cli;
            meth.Body.UpdateInstructionOffsets();
            CilBody body = meth.Body;
            body.MaxStack += 10;
        }
    }
}