using System;
using System.IO;
using dnlib.DotNet;

namespace FVM.Helpers.DynConverter
{
    public static class Extension
    {
        public static void ConvertToBytes(this BinaryWriter writer, MethodDef method)
        {
            new Converter(method, writer).ConvertToBytes();
        }
    }
}
