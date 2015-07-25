using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuriNET.Utils {
    static class BytesUtil {
        /// <summary>
        /// Singleton Class  Can't instantiate
        /// </summary>

        public static short ToShort(byte byte1, byte byte2) {
            return (short) ((byte2 << 8) + byte1);
        }

        public static void FromShort(short number, out byte byte1, out byte byte2) {
            byte2 = (byte) (number >> 8);
            byte1 = (byte) (number & 255);
        }


    }
}
