using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonsUtil
{
    public class UnixTime
    {
        private static DateTime _baseTime = new DateTime(1970, 1, 1);

        public static DateTime FromUnixTime(long timeStamp)
        {
            return new DateTime((timeStamp + 28800L) * 10000000L + UnixTime._baseTime.Ticks);
        }

        public static long FromDateTime(DateTime dateTime)
        {
            return (dateTime.Ticks - UnixTime._baseTime.Ticks) / 10000000L - 28800L;
        }

        public static long FromDateTimeByGMT8(DateTime dateTime)
        {
            return (dateTime.Ticks - UnixTime._baseTime.Ticks) / 10000000L;
        }
    }
}
