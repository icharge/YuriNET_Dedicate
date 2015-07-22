using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace YuriNET {
    class NativeMethods {
        // For DPI Aware
        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();
    }
}
