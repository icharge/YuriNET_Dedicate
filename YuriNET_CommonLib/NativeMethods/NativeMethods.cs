using System.Runtime.InteropServices;

namespace YuriNET.Common {

    public static class NativeMethods {

        // For DPI Aware
        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();
    }
}