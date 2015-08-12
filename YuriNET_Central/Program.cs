using System;
using System.Windows.Forms;
using YuriNET.Common;

namespace YuriNET.Central {

    internal static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main() {
            if (Environment.OSVersion.Version.Major >= 6)
                NativeMethods.SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Form1());
        }
    }
}