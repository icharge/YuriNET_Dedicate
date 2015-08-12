using System;

namespace YuriNET.Utils {

    /// <summary>
    /// Print Any logs output by level
    /// </summary>
    public static class Logger {

        public enum DisplayLevels {
            Fatal = 1,
            Error = 2,
            Warn = 4,
            Debug = 8,
            Info = 16,
            All = ~0
        };

        private static DisplayLevels displayLevel;

        public static DisplayLevels DisplayLevel {
            get {
                return displayLevel;
            }
            set {
                info("Logger Display level changed to : " + value);
                displayLevel = value;
            }
        }

        public static void info(Object str, params Object[] arg) {
            if ((displayLevel & DisplayLevels.Info) == DisplayLevels.Info) {
                Console.WriteLine(DateTime.Now + " [INFO] : " + str, arg);
            }
        }

        public static void debug(Object str, params Object[] arg) {
            if ((displayLevel & DisplayLevels.Debug) == DisplayLevels.Debug) {
                Console.WriteLine(DateTime.Now + " [DEBUG] : " + str, arg);
            }
        }

        public static void warn(Object str, params Object[] arg) {
            if ((displayLevel & DisplayLevels.Warn) == DisplayLevels.Warn) {
                Console.WriteLine(DateTime.Now + " [WARN] : " + str, arg);
            }
        }

        public static void error(Object str, params Object[] arg) {
            if ((displayLevel & DisplayLevels.Error) == DisplayLevels.Error) {
                Console.WriteLine(DateTime.Now + " [ERROR] : " + str, arg);
            }
        }

        public static void fatal(Object str, params Object[] arg) {
            if ((displayLevel & DisplayLevels.Fatal) == DisplayLevels.Fatal) {
                Console.WriteLine(DateTime.Now + " [FATAL] : " + str, arg);
            }
        }
    }
}