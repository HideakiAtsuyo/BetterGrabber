using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator
{
    public class Emulator
    {
        public Emulator(List<Instruction> instructions, List<Local> locals)
        {
            this._context = new EmuContext(instructions, locals);
            this._emuInstructions = new Dictionary<OpCode, EmuInstruction>();
            List<EmuInstruction> emuInstructions = (from t in typeof(EmuInstruction).Assembly.GetTypes()
                                                    where t.IsSubclassOf(typeof(EmuInstruction)) && !t.IsAbstract
                                                    select (EmuInstruction)Activator.CreateInstance(t)).ToList<EmuInstruction>();
            foreach (EmuInstruction instrEmu in emuInstructions)
            {
                this._emuInstructions.Add(instrEmu.OpCode, instrEmu);
            }
        }

        internal int Emulate()
        {
            for (int i = this._context.InstructionPointer; i < this._context.Instructions.Count; i++)
            {
                Instruction current = this._context.Instructions[i];
                bool flag = current.OpCode == OpCodes.Stloc_S;
                if (flag)
                {
                    break;
                }
                bool flag2 = current.OpCode != OpCodes.Nop;
                if (flag2)
                {
                    this._emuInstructions[current.OpCode].Emulate(this._context, current);
                }
            }
            return (int)this._context.Stack.Pop();
        }

        public EmuContext _context;

        private Dictionary<OpCode, EmuInstruction> _emuInstructions;
    }
}
