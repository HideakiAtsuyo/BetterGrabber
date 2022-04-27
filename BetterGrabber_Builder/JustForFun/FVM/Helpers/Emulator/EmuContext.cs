using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator
{
    public class EmuContext
    {
        public EmuContext(List<Instruction> instructions, List<Local> locals)
        {
            this.Stack = new Stack<object>();
            this.Instructions = instructions;
            this._locals = new Dictionary<Local, object>();
            foreach (Local local in locals)
            {
                this._locals.Add(local, null);
            }
        }

        internal object GetLocalValue(Local local)
        {
            Type type = Type.GetType(local.Type.AssemblyQualifiedName);
            return Convert.ChangeType(this._locals[local], type);
        }

        internal void SetLocalValue(Local local, object val)
        {
            this._locals[local] = val;
        }

        internal Stack<object> Stack;

        internal List<Instruction> Instructions;

        internal int InstructionPointer = 0;

        public Dictionary<Local, object> _locals;
    }
}
