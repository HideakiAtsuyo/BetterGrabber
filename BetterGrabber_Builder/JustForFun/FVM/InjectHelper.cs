using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace FVM
{
	// Token: 0x02000003 RID: 3
	public static class InjectHelper
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002110 File Offset: 0x00000310
		private static TypeDefUser Clone(TypeDef origin)
		{
			TypeDefUser ret = new TypeDefUser(origin.Namespace, origin.Name);
			ret.Attributes = origin.Attributes;
			bool flag = origin.ClassLayout != null;
			if (flag)
			{
				ret.ClassLayout = new ClassLayoutUser(origin.ClassLayout.PackingSize, origin.ClassSize);
			}
			foreach (GenericParam genericParam in origin.GenericParameters)
			{
				ret.GenericParameters.Add(new GenericParamUser(genericParam.Number, genericParam.Flags, "-"));
			}
			return ret;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000021D0 File Offset: 0x000003D0
		private static MethodDefUser Clone(MethodDef origin)
		{
			MethodDefUser ret = new MethodDefUser(origin.Name, null, origin.ImplAttributes, origin.Attributes);
			foreach (GenericParam genericParam in origin.GenericParameters)
			{
				ret.GenericParameters.Add(new GenericParamUser(genericParam.Number, genericParam.Flags, "-"));
			}
			return ret;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002260 File Offset: 0x00000460
		private static FieldDefUser Clone(FieldDef origin)
		{
			return new FieldDefUser(origin.Name, null, origin.Attributes);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002288 File Offset: 0x00000488
		private static TypeDef PopulateContext(TypeDef typeDef, InjectHelper.InjectContext ctx)
		{
			IDnlibDef existing;
			bool flag = !ctx.map.TryGetValue(typeDef, out existing);
			TypeDef ret;
			if (flag)
			{
				ret = InjectHelper.Clone(typeDef);
				ctx.map[typeDef] = ret;
			}
			else
			{
				ret = (TypeDef)existing;
			}
			foreach (TypeDef nestedType in typeDef.NestedTypes)
			{
				ret.NestedTypes.Add(InjectHelper.PopulateContext(nestedType, ctx));
			}
			foreach (MethodDef method in typeDef.Methods)
			{
				ret.Methods.Add((MethodDef)(ctx.map[method] = InjectHelper.Clone(method)));
			}
			foreach (FieldDef field in typeDef.Fields)
			{
				ret.Fields.Add((FieldDef)(ctx.map[field] = InjectHelper.Clone(field)));
			}
			return ret;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000023F4 File Offset: 0x000005F4
		private static void CopyTypeDef(TypeDef typeDef, InjectHelper.InjectContext ctx)
		{
			TypeDef newTypeDef = (TypeDef)ctx.map[typeDef];
			newTypeDef.BaseType = ctx.Importer.Import(typeDef.BaseType);
			foreach (InterfaceImpl iface in typeDef.Interfaces)
			{
				newTypeDef.Interfaces.Add(new InterfaceImplUser(ctx.Importer.Import(iface.Interface)));
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002490 File Offset: 0x00000690
		private static void CopyMethodDef(MethodDef methodDef, InjectHelper.InjectContext ctx)
		{
			MethodDef newMethodDef = (MethodDef)ctx.map[methodDef];
			newMethodDef.Signature = ctx.Importer.Import(methodDef.Signature);
			newMethodDef.Parameters.UpdateParameterTypes();
			bool flag = methodDef.ImplMap != null;
			if (flag)
			{
				newMethodDef.ImplMap = new ImplMapUser(new ModuleRefUser(ctx.TargetModule, methodDef.ImplMap.Module.Name), methodDef.ImplMap.Name, methodDef.ImplMap.Attributes);
			}
			foreach (CustomAttribute ca in methodDef.CustomAttributes)
			{
				newMethodDef.CustomAttributes.Add(new CustomAttribute((ICustomAttributeType)ctx.Importer.Import(ca.Constructor)));
			}
			bool hasBody = methodDef.HasBody;
			if (hasBody)
			{
				newMethodDef.Body = new CilBody(methodDef.Body.InitLocals, new List<Instruction>(), new List<ExceptionHandler>(), new List<Local>());
				newMethodDef.Body.MaxStack = methodDef.Body.MaxStack;
				Dictionary<object, object> bodyMap = new Dictionary<object, object>();
				foreach (Local local in methodDef.Body.Variables)
				{
					Local newLocal = new Local(ctx.Importer.Import(local.Type));
					newMethodDef.Body.Variables.Add(newLocal);
					newLocal.Name = local.Name;
					bodyMap[local] = newLocal;
				}
				foreach (Instruction instr in methodDef.Body.Instructions)
				{
					Instruction newInstr = new Instruction(instr.OpCode, instr.Operand);
					newInstr.SequencePoint = instr.SequencePoint;
					bool flag2 = newInstr.Operand is IType;
					if (flag2)
					{
						newInstr.Operand = ctx.Importer.Import((IType)newInstr.Operand);
					}
					else
					{
						bool flag3 = newInstr.Operand is IMethod;
						if (flag3)
						{
							newInstr.Operand = ctx.Importer.Import((IMethod)newInstr.Operand);
						}
						else
						{
							bool flag4 = newInstr.Operand is IField;
							if (flag4)
							{
								newInstr.Operand = ctx.Importer.Import((IField)newInstr.Operand);
							}
						}
					}
					newMethodDef.Body.Instructions.Add(newInstr);
					bodyMap[instr] = newInstr;
				}
				Func<Instruction, Instruction> <>9__0;
				foreach (Instruction instr2 in newMethodDef.Body.Instructions)
				{
					bool flag5 = instr2.Operand != null && bodyMap.ContainsKey(instr2.Operand);
					if (flag5)
					{
						instr2.Operand = bodyMap[instr2.Operand];
					}
					else
					{
						bool flag6 = instr2.Operand is Instruction[];
						if (flag6)
						{
							Instruction instruction = instr2;
							IEnumerable<Instruction> source = (Instruction[])instr2.Operand;
							Func<Instruction, Instruction> selector;
							if ((selector = <>9__0) == null)
							{
								selector = (<>9__0 = ((Instruction target) => (Instruction)bodyMap[target]));
							}
							instruction.Operand = source.Select(selector).ToArray<Instruction>();
						}
					}
				}
				foreach (ExceptionHandler eh in methodDef.Body.ExceptionHandlers)
				{
					newMethodDef.Body.ExceptionHandlers.Add(new ExceptionHandler(eh.HandlerType)
					{
						CatchType = ((eh.CatchType == null) ? null : ctx.Importer.Import(eh.CatchType)),
						TryStart = (Instruction)bodyMap[eh.TryStart],
						TryEnd = (Instruction)bodyMap[eh.TryEnd],
						HandlerStart = (Instruction)bodyMap[eh.HandlerStart],
						HandlerEnd = (Instruction)bodyMap[eh.HandlerEnd],
						FilterStart = ((eh.FilterStart == null) ? null : ((Instruction)bodyMap[eh.FilterStart]))
					});
				}
				newMethodDef.Body.SimplifyMacros(newMethodDef.Parameters);
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002A18 File Offset: 0x00000C18
		private static void CopyFieldDef(FieldDef fieldDef, InjectHelper.InjectContext ctx)
		{
			FieldDef newFieldDef = (FieldDef)ctx.map[fieldDef];
			newFieldDef.Signature = ctx.Importer.Import(fieldDef.Signature);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002A54 File Offset: 0x00000C54
		private static void Copy(TypeDef typeDef, InjectHelper.InjectContext ctx, bool copySelf)
		{
			if (copySelf)
			{
				InjectHelper.CopyTypeDef(typeDef, ctx);
			}
			foreach (TypeDef nestedType in typeDef.NestedTypes)
			{
				InjectHelper.Copy(nestedType, ctx, true);
			}
			foreach (MethodDef method in typeDef.Methods)
			{
				InjectHelper.CopyMethodDef(method, ctx);
			}
			foreach (FieldDef field in typeDef.Fields)
			{
				InjectHelper.CopyFieldDef(field, ctx);
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002B3C File Offset: 0x00000D3C
		public static TypeDef Inject(TypeDef typeDef, ModuleDef target)
		{
			InjectHelper.InjectContext ctx = new InjectHelper.InjectContext(typeDef.Module, target);
			InjectHelper.PopulateContext(typeDef, ctx);
			InjectHelper.Copy(typeDef, ctx, true);
			return (TypeDef)ctx.map[typeDef];
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002B80 File Offset: 0x00000D80
		public static MethodDef Inject(MethodDef methodDef, ModuleDef target)
		{
			InjectHelper.InjectContext ctx = new InjectHelper.InjectContext(methodDef.Module, target);
			ctx.map[methodDef] = InjectHelper.Clone(methodDef);
			InjectHelper.CopyMethodDef(methodDef, ctx);
			return (MethodDef)ctx.map[methodDef];
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002BCC File Offset: 0x00000DCC
		public static IEnumerable<IDnlibDef> Inject(TypeDef typeDef, TypeDef newType, ModuleDef target)
		{
			InjectHelper.InjectContext ctx = new InjectHelper.InjectContext(typeDef.Module, target);
			ctx.map[typeDef] = newType;
			InjectHelper.PopulateContext(typeDef, ctx);
			InjectHelper.Copy(typeDef, ctx, false);
			return ctx.map.Values.Except(new TypeDef[]
			{
				newType
			});
		}

		// Token: 0x02000033 RID: 51
		private class InjectContext : ImportMapper
		{
			// Token: 0x060000BC RID: 188 RVA: 0x0000C4A0 File Offset: 0x0000A6A0
			public InjectContext(ModuleDef module, ModuleDef target)
			{
				this.OriginModule = module;
				this.TargetModule = target;
				this.importer = new Importer(target, ImporterOptions.TryToUseTypeDefs, default(GenericParamContext), this);
			}

			// Token: 0x1700001D RID: 29
			// (get) Token: 0x060000BD RID: 189 RVA: 0x0000C4E8 File Offset: 0x0000A6E8
			public Importer Importer
			{
				get
				{
					return this.importer;
				}
			}

			// Token: 0x060000BE RID: 190 RVA: 0x0000C500 File Offset: 0x0000A700
			public override ITypeDefOrRef Map(ITypeDefOrRef typeDefOrRef)
			{
				TypeDef typeDef = typeDefOrRef as TypeDef;
				bool flag = typeDef != null;
				if (flag)
				{
					bool flag2 = this.map.ContainsKey(typeDef);
					if (flag2)
					{
						return (TypeDef)this.map[typeDef];
					}
				}
				return null;
			}

			// Token: 0x060000BF RID: 191 RVA: 0x0000C548 File Offset: 0x0000A748
			public override IMethod Map(MethodDef methodDef)
			{
				bool flag = this.map.ContainsKey(methodDef);
				IMethod result;
				if (flag)
				{
					result = (MethodDef)this.map[methodDef];
				}
				else
				{
					result = null;
				}
				return result;
			}

			// Token: 0x060000C0 RID: 192 RVA: 0x0000C580 File Offset: 0x0000A780
			public override IField Map(FieldDef fieldDef)
			{
				bool flag = this.map.ContainsKey(fieldDef);
				IField result;
				if (flag)
				{
					result = (FieldDef)this.map[fieldDef];
				}
				else
				{
					result = null;
				}
				return result;
			}

			// Token: 0x04000043 RID: 67
			public readonly Dictionary<IDnlibDef, IDnlibDef> map = new Dictionary<IDnlibDef, IDnlibDef>();

			// Token: 0x04000044 RID: 68
			public readonly ModuleDef OriginModule;

			// Token: 0x04000045 RID: 69
			public readonly ModuleDef TargetModule;

			// Token: 0x04000046 RID: 70
			private readonly Importer importer;
		}
	}
}
