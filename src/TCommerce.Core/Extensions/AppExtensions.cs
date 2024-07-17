using NodaTime;
using System.Reflection.Metadata;

namespace TCommerce.Core.Extensions
{
    public static class AppExtensions
    {
        public static DateTime GetDateTimeNow()
        {
            // Lấy thời gian hiện tại
            Instant now = SystemClock.Instance.GetCurrentInstant();

            // Chuyển đổi sang giờ Việt Nam
            DateTimeZone vietnamTimeZone = DateTimeZoneProviders.Tzdb["Asia/Saigon"];
            DateTime vietnamTime = now.InZone(vietnamTimeZone).ToDateTimeUnspecified();
            return vietnamTime;
        }
    }
}
