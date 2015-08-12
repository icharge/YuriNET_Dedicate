using System;

namespace YuriNET.Utils {

    public static class DateTimeUtil {

        public static long GetTimestamp(DateTime date) {
            return date.Ticks;
        }

        // NOT WORK
        [Obsolete("Not work", true)]
        public static long TimeDiffBySec(long t1, long t2) {
            return (t1 - t2);
        }

        //// NOT WORK
        //[Obsolete("Not work", true)]
        //public static long TimeDiffBySec(DateTime d1, DateTime d2) {
        //    return TimeDiffBySec(GetTimestamp(d1), GetTimestamp(d2));
        //}

        public static bool checkTimeout(DateTime timestamp, long timeout) {
            return timestamp.AddMilliseconds(timeout).Ticks < DateTime.Now.Ticks;
        }
    }
}