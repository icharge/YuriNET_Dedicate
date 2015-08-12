using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuriNET.Common.Utils {
    public class ConvertUtil {

        /// <summary>
        /// Convert string numeric to Integer
        /// </summary>
        /// <param name="str">String numeric</param>
        /// <exception cref="FormatException">String is not number or invalid.</exception>
        /// <returns>Integer</returns>
        public static int ToInt(String str) {
            return Convert.ToInt32(str);
        }

        public static bool ToInt(String str, out int integer) {
            try {
                integer = Convert.ToInt32(str);
                return true;
            } catch {
                Logger.warn("Can't convert to Integer. Return 0 instead");
                integer = 0;
                return false;
            }
        }

        public static int ToInt(String str, int defaultValue) {
            try {
                return Convert.ToInt32(str);
            } catch {
                return defaultValue;
            }
        }
    }
}
