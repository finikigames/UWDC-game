using System;

namespace Core.Extensions {
    public static class TimeExtensions {
        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp) {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dateTime;
        }

        public static long ToUnixTimeStamp(this DateTime dt) {
            var offset = (DateTimeOffset) dt;

            return offset.ToUnixTimeSeconds();
        }
    }
}