using System;
using System.Globalization;

namespace Puppets.Utils
{
    public static class DateUtils
    {
        public static string PATTERN = "yyyy-MM-ddTHH:mm:sszzz";

        public static TimeSpan TimeTill(DateTime date)
        {
            return date - SystemTime.NowUTC().ToUniversalTime();
        }

        public static DateTime Parse(string dateString)
        {
            return DateTime.ParseExact(dateString, PATTERN, CultureInfo.InvariantCulture).ToUniversalTime();
        }

        public static string ToString(DateTime dateTime)
        {
            return dateTime.ToString(PATTERN);
        }
    }
}
