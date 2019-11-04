using System;

namespace CriptoExchengLib.Classes.Helper
{
    public static class DateTimeExtensions
    {
        public static long ToUnixTimestamp(this DateTime d)
        {
            var epoch = d - new DateTime(1970, 1, 1, 0, 0, 0);

            return (long)epoch.TotalSeconds;
        }

        public static DateTime FromUnixTimestamp(this DateTime d,Int64 timestamp)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            return epoch.AddSeconds(timestamp);
        }
    }
}
