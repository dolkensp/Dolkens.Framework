using System;

namespace Dolkens.Framework.Utilities
{
    public static class DateTimeUtilities
    {
        public static DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime StartOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Sunday) { return date.Date.AddDays(-(Int32)date.AddDays(-(Int32)startOfWeek).DayOfWeek); }

        public static DateTime EndOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Sunday) { return date.StartOfWeek(startOfWeek).AddDays(7).AddTicks(-1); }

        public static DateTime TrimTo(DateTime datetime, TimeSpan interval)
        {
            if (interval.TotalHours > 24)
                throw new ArgumentOutOfRangeException("Timespan too large, timespan must be less than 24 hours");

            return datetime.AddMilliseconds(0 - (datetime.TimeOfDay.TotalMilliseconds % interval.TotalMilliseconds));
        }

        public static DateTimeOffset StartOfWeek(DateTimeOffset date, DayOfWeek startOfWeek = DayOfWeek.Sunday) { return date.Date.AddDays(-(Int32)date.AddDays(-(Int32)startOfWeek).DayOfWeek); }

        public static DateTimeOffset EndOfWeek(DateTimeOffset date, DayOfWeek startOfWeek = DayOfWeek.Sunday) { return date.StartOfWeek(startOfWeek).AddDays(7); }

        public static DateTimeOffset TrimTo(DateTimeOffset datetime, TimeSpan interval)
        {
            if (interval.TotalHours > 24)
                throw new ArgumentOutOfRangeException("Timespan too large, timespan must be less than 24 hours");

            return datetime.AddMilliseconds(0 - (datetime.TimeOfDay.TotalMilliseconds % interval.TotalMilliseconds));
        }

        public static TimeSpan TrimTo(TimeSpan timespan, TimeSpan interval)
        {
            return TimeSpan.FromMilliseconds(timespan.TotalMilliseconds - (timespan.TotalMilliseconds % interval.TotalMilliseconds));
        }
    }
}

namespace System
{
    using DDRIT = Dolkens.Framework.Utilities.DateTimeUtilities;

    public static partial class _Proxy
    {
        /// <summary>
        /// Returns the date of the first day of the week starting on <paramref name="startOfWeek" />, which contains <paramref name="date"/>.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTime StartOfWeek(this DateTime date, DayOfWeek startOfWeek = DayOfWeek.Sunday) { return DDRIT.StartOfWeek(date, startOfWeek); }

        /// <summary>
        /// Returns the date of the first day of the week starting on <paramref name="startOfWeek" />, which contains <paramref name="date"/>.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTimeOffset StartOfWeek(this DateTimeOffset date, DayOfWeek startOfWeek = DayOfWeek.Sunday) { return DDRIT.StartOfWeek(date, startOfWeek); }

        /// <summary>
        /// Returns the date of the last day of the week ending on <paramref name="startOfWeek"/>, which contains <paramref name="date"/>.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTime EndOfWeek(this DateTime date, DayOfWeek startOfWeek = DayOfWeek.Sunday) { return DDRIT.EndOfWeek(date, startOfWeek); }

        /// <summary>
        /// Returns the date of the last day of the week ending on <paramref name="startOfWeek"/>, which contains <paramref name="date"/>.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTimeOffset EndOfWeek(this DateTimeOffset date, DayOfWeek startOfWeek = DayOfWeek.Sunday) { return DDRIT.EndOfWeek(date, startOfWeek); }

        /// <summary>
        /// Trim a DateTime to the smallest whole TimeSpan that fits within a 24 hour period.
        /// </summary>
        /// <param name="input">The time to trim</param>
        /// <param name="interval">The smallest timespan per day to allow</param>
        /// <returns></returns>
        public static DateTime TrimTo(this DateTime input, TimeSpan interval) { return DDRIT.TrimTo(input, interval); }

        /// <summary>
        /// Trim a DateTime to the smallest whole TimeSpan that fits within a 24 hour period.
        /// </summary>
        /// <param name="input">The time to trim</param>
        /// <param name="interval">The smallest timespan per day to allow</param>
        /// <returns></returns>
        public static DateTimeOffset TrimTo(this DateTimeOffset input, TimeSpan interval) { return DDRIT.TrimTo(input, interval); }

        /// <summary>
        /// Trim a TimeSpan to the smallest whole TimeSpan that fits within a 24 hour period.
        /// </summary>
        /// <param name="input">The time to trim</param>
        /// <param name="interval">The smallest timespan per day to allow</param>
        /// <returns></returns>
        public static TimeSpan TrimTo(this TimeSpan input, TimeSpan interval) { return DDRIT.TrimTo(input, interval); }
    }
}