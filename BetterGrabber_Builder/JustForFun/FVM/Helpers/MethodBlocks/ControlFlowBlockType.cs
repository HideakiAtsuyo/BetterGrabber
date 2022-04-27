using System;

namespace FVM.Helpers.MethodBlocks
{
    [Flags]
    public enum ControlFlowBlockType
    {
        Normal = 0,
        Entry = 1,
        Exit = 2
    }
}
