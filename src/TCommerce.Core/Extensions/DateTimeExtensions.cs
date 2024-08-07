using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCommerce.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ConvertToUserTime(this DateTime dateTime, TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        }

        public static DateTime ConvertToUtcTime(this DateTime dateTime, TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
        }

        public static TimeZoneInfo GetCurrentTimeZone()
        {
            // Implement logic to get current user's timezone
            return TimeZoneInfo.Local; // Example: default to local timezone
        }
    }
}
