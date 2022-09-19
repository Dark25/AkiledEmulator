using System;

namespace Akiled.Utilities
{
    internal static class UnixTimestamp
    {
        public static double GetNow() => (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

        public static DateTime FromUnixTimestamp(double Timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(Timestamp);
            return dateTime;
        }
    }
}