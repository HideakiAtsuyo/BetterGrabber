using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetterGrabber_Builder
{
    internal static class Utils
    {
        public static bool arrayContains(this string[] x, string z)
        {
            return Array.IndexOf(x, z) != -1;
        }
    }
}
