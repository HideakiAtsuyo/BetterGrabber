using System.Reflection;
using System.Text;

namespace BetterGrabber_Builder.JustForFun.Runtime
{
    internal static class StringRuntime
    {
        public static string OhMyGod(string txt, int key)
        {
            StringBuilder Input = new StringBuilder(txt);
            StringBuilder Output = new StringBuilder(txt.Length);
            if (Assembly.GetExecutingAssembly().Equals(Assembly.GetCallingAssembly()))
            {
                for (int i = 0; i < txt.Length; i++)
                {
                    Output.Append((char)(Input[i] ^ key));
                }
            } else
            {
                return "Hello World";
            }
            return Output.ToString();
        }
    }
}
