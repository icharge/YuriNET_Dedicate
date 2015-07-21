using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace YuriNET_Dedicate
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            if (Environment.OSVersion.Version.Major >= 6) SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!args.Contains("/ByPa55"))
            {
                string pw = "";
                InputBox.Prompt("Authentication", "Enter password :", ref pw);
                DateTime a = DateTime.Now;
                int y = int.Parse(a.ToString("yyyy"));
                int m = int.Parse(a.ToString("MM"));
                int d = int.Parse(a.ToString("dd"));
                if (pw != (y + m + d).ToString())
                {
                    MessageBox.Show("Wrong password !", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }
            }

            Application.Run(new Form1());
        }

        // For DPI Aware
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        public static long GetTimestamp(DateTime value)
        {
            return long.Parse(value.ToString("yyyyMMddHHmmss"));//yyyyMMddHHmmssffff
        }

        public static int TimeDiffBySec(long t1, long t2)
        {
            return (int)((t1 - t2));
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}
