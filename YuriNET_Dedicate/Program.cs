using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using YuriNET.Components;

namespace YuriNET {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args) {
            if (Environment.OSVersion.Version.Major >= 6) NativeMethods.SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /**
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
            */

            Application.Run(new Form1());
        }



    }
}
