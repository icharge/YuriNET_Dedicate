using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuriNET.Utils {
    static class DateTimeUtil {

        public static long GetTimestamp(DateTime value) {
            return long.Parse(value.ToString("yyyyMMddHHmmss"));//yyyyMMddHHmmssffff
        }

        public static int TimeDiffBySec(long t1, long t2) {
            return (int) ((t1 - t2));
        }

        public static int TimeDiffBySec(DateTime d1, DateTime d2) {
            return TimeDiffBySec(GetTimestamp(d1), GetTimestamp(d2));
        }
    }
}
