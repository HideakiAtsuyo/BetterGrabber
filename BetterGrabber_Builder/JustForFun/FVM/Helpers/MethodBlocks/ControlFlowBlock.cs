using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.MethodBlocks
{
    public class ControlFlowBlock
    {
        internal ControlFlowBlock(int id, ControlFlowBlockType type, Instruction header, Instruction footer)
        {
            this.Id = id;
            this.Type = type;
            this.Header = header;
            this.Footer = footer;
            this.Sources = new List<ControlFlowBlock>();
            this.Targets = new List<ControlFlowBlock>();
        }

        // (set) Token: 0x06000074 RID: 116 RVA: 0x0000AD27 File Offset: 0x00008F27
        public IList<ControlFlowBlock> Sources { get; private set; }

        // (set) Token: 0x06000076 RID: 118 RVA: 0x0000AD38 File Offset: 0x00008F38
        public IList<ControlFlowBlock> Targets { get; private set; }

        public override string ToString()
        {
            return string.Format("Block {0} => {1} {2}", this.Id, this.Type, string.Join(", ", (from block in this.Targets
                                                                                                select block.Id.ToString()).ToArray<string>()));
        }

        public readonly Instruction Footer;

        public readonly Instruction Header;

        public readonly int Id;

        public readonly ControlFlowBlockType Type;
    }
}
