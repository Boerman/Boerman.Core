using System;
using System.Globalization;

namespace Boerman.Core.Extensions
{
    public static partial class Extensions
    {
        public static int ToUnixTime(this DateTime dateTime)
        {
            return (Int32)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static long ToUnixMilliseconds(this DateTime dateTime)
        {
            return (long)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        // <3 you http://stackoverflow.com/a/2883645/1720761
        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        /// <summary>
        /// Gets the first day of the current week
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfWeek(this DateTime dt)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
                diff += 7;
            return dt.AddDays(-diff).Date;
        }

        public static DateTime LastDayOfWeek(this DateTime dt)
        {
            return dt.FirstDayOfWeek().AddDays(6);
        }

        public static DateTime FirstDayOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static DateTime LastDayOfMonth(this DateTime dt)
        {
            return dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);
        }

        public static DateTime FirstDayOfNextMonth(this DateTime dt)
        {
            return dt.FirstDayOfMonth().AddMonths(1);
        }

        public static DateTime NextQuarter(this DateTime dt)
        {
            if (dt.Millisecond != 0) dt = dt.AddMilliseconds(1000 - dt.Millisecond);
            if (dt.Second != 0) dt = dt.AddSeconds(60 - dt.Second);
            dt = dt.AddMinutes((60 - dt.Minute) % 15);
            
            return dt;
        }

        public static string ToHighchartsXAxisTime(this DateTime dt)
        {
            NumberFormatInfo nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
            return ((dt - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds * 1000).ToString(nfi);
        }
        
        public static DateTime SafeUniversal(this DateTime inTime)
        {
            return (DateTimeKind.Unspecified == inTime.Kind)
                ? new DateTime(inTime.Ticks, DateTimeKind.Utc)
                : inTime.ToUniversalTime();
        }
    }
}
