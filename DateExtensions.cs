using System;
using TimeZoneConverter;

namespace DateConversion.Extensions
{
    public static class DateExtensions
    {
        /// <summary>
        /// Sets time for a date. The previous time will be cleared.
        /// </summary>
        /// <param name="baseDate">The date which its time will be set.</param>
        /// <param name="time">New time</param>
        /// <returns>A new DateTime that its time is set.</returns>
        public static DateTime SetTime(this DateTime baseDate, TimeSpan time) => baseDate.Date.Add(time);

        public static DateTimeOffset SetTime(this DateTimeOffset baseDate, TimeSpan time) =>
            new(baseDate.Year, baseDate.Month, baseDate.Day, time.Hours, time.Minutes, time.Seconds, baseDate.Offset);

        public static DateTimeOffset AddTimeZone(this DateTime baseDate, TimeSpan offset) => new(baseDate.Ticks, offset);

        /// <summary>
        /// Converts a DateTime to another DateTimeOffset
        /// </summary>
        /// <param name="baseDate">The date which its time will be set.</param>
        /// <param name="targetZone">The preferred target time zone.</param>
        /// <returns>A new DateTime that its date and time is changed to the target time zone.</returns>
        public static DateTimeOffset ConvertByTimeZone(this DateTime baseDate, string targetZone) =>
            ConvertByTimeZone(baseDate, TimeSpan.Zero, targetZone);

        
        /// <summary>
        /// Converts a DateTime to another DateTimeOffset and adds a time span to it
        /// </summary>
        /// <param name="baseDate">The date which its time will be set.</param>
        /// <param name="offset">Adds an offset to the converted DateTimeOffset.</param>
        /// <param name="targetZone">The preferred target time zone.</param>
        /// <returns>A new DateTime that its date and time is changed to the target time zone.</returns>
        public static DateTimeOffset ConvertByTimeZone(this DateTime baseDate, TimeSpan offset, string targetZone)
        {
            TimeZoneInfo zone = TZConvert.GetTimeZoneInfo(targetZone);
            var targetOffset = zone.GetUtcOffset(baseDate);
            var d = DateTime.SpecifyKind(baseDate, DateTimeKind.Unspecified);
            return new DateTimeOffset(d.Add(targetOffset).Add(offset), targetOffset);
        }

        public static DateTimeOffset ConvertByTimeZone(this DateTimeOffset baseDate, string targetZone) =>
            ConvertByTimeZone(baseDate.UtcDateTime, TimeSpan.Zero, targetZone);

        /// <summary>
        /// Gets the time zone offset with is different from UTC
        /// </summary>
        /// <param name="zone">Standard form of time zone</param>
        /// <param name="respectDayLightSaving">If true, the default time zone offset is returned and the daylight saving
        /// is considered. If false is passed, the default base time zone is returned and the daylight saving
        /// is not considered</param>
        /// <param name="preferredDate">The date in which the daylight is checked. The default value will be DateTime.UtcNow</param>
        /// <returns></returns>
        public static TimeSpan GetTimeZoneOffset(string zone, bool respectDayLightSaving = true, DateTime preferredDate = default)
        {
            var targetDate = preferredDate == default ? DateTime.UtcNow : preferredDate;
            return respectDayLightSaving 
                ? TZConvert.GetTimeZoneInfo(zone).GetUtcOffset(targetDate)
                : TZConvert.GetTimeZoneInfo(zone).BaseUtcOffset;
        }

        /// <summary>
        /// The offset between two time zones are calculated
        /// </summary>
        /// <example>The difference between Iran Standard time and UTC is 3:30 hours.</example>
        /// <code>GetTimeZoneOffset("Iran Standard time", "UTC") = 3:30</code>
        /// <param name="sourceZone"></param>
        /// <param name="destinationZone"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeZoneOffset(string sourceZone, string destinationZone) =>
            GetTimeZoneOffset(sourceZone) - GetTimeZoneOffset(destinationZone);
    }
}
