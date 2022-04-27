using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace FVM
{
    public class Injector
    {
        public ModuleDefMD TargetModule { get; }

        public Type RuntimeType { get; }

        public List<IDnlibDef> Members { get; }

        public Injector(ModuleDefMD targetModule, Type type, bool injectType = true)
        {
            this.TargetModule = targetModule;
            this.RuntimeType = type;
            this.Members = new List<IDnlibDef>();
            if (injectType)
            {
                this.InjectType();
            }
        }

        public void InjectType()
        {
            ModuleDefMD typeModule = ModuleDefMD.Load(this.RuntimeType.Module);
            TypeDef typeDefs = typeModule.ResolveTypeDef(MDToken.ToRID(this.RuntimeType.MetadataToken));
            this.Members.AddRange(BetterGrabber_Builder.JustForFun.Helpers.InjectHelper.Inject(typeDefs, this.TargetModule.GlobalType, this.TargetModule).ToList<IDnlibDef>());
        }

        public IDnlibDef FindMember(string name)
        {
            foreach (IDnlibDef member in this.Members)
            {
                bool flag = member.Name == name;
                if (flag)
                {
                    return member;
                }
            }
            throw new Exception("Error to find member.");
        }

        public void Rename()
        {
            foreach (IDnlibDef mem in this.Members)
            {
                MethodDef method = mem as MethodDef;
                bool flag = method != null;
                if (flag)
                {
                    bool hasImplMap = method.HasImplMap;
                    if (hasImplMap)
                    {
                        continue;
                    }
                    bool isDelegate = method.DeclaringType.IsDelegate;
                    if (isDelegate)
                    {
                        continue;
                    }
                }
                mem.Name = BetterGrabber_Builder.JustForFun.Helpers.Others.RandomString(BetterGrabber_Builder.JustForFun.Helpers.Others.rnd.Next(10, 100));
            }
        }
    }
}
