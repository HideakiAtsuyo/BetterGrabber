using System;
using System.Linq;

namespace BetterGrabber_Builder.JustForFun.Helpers
{
    internal class Others
    {
        public static Random rnd = new Random();
        public static string RandomString(int length)
        {
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length).Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}
