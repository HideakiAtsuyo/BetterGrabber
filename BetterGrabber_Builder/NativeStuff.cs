using System;
using System.Runtime.InteropServices;

namespace BetterGrabber_Builder
{
    internal static class NativeStuff
    {
        /*==========Move Form==========*/
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        /*==========Move Form==========*/
    }
}
