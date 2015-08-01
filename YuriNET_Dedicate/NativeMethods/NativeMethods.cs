using System.Runtime.InteropServices;

namespace YuriNET {

    internal static class NativeMethods {

        // For DPI Aware
        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();
    }
}