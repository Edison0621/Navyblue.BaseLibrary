// *****************************************************************************************************************
// Project          : NavyBlue
// File             : DateTime.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:51
// *****************************************************************************************************************
// <copyright file="DateTime.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;
using System.Diagnostics.CodeAnalysis;

namespace Navyblue.BaseLibrary
{
    /// <summary>
    ///     Extensions of <see cref="System.DateTime" /> type.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        ///     Durations to now.
        /// </summary>
        /// <param name="startDateTime">The start date time.</param>
        /// <returns>TimeSpan.</returns>
        public static TimeSpan DurationToNow(this DateTime startDateTime)
        {
            return (DateTime.Now - startDateTime);
        }

        /// <summary>
        ///     Determines whether the specified destination is after.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns><c>true</c> if the specified destination is after; otherwise, <c>false</c>.</returns>
        public static bool IsAfter(this DateTime source, DateTime destination)
        {
            return DateTimeUtility.IsAfter(source, destination);
        }

        /// <summary>
        ///     Determines whether the specified destination is after.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="redundancy">The redundancy.</param>
        /// <returns><c>true</c> if the specified destination is after; otherwise, <c>false</c>.</returns>
        public static bool IsAfter(this DateTime source, DateTime destination, TimeSpan redundancy)
        {
            return DateTimeUtility.IsAfter(source, destination, redundancy);
        }

        /// <summary>
        ///     Determines whether [is after or equal] [the specified destination].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns><c>true</c> if [is after or equal] [the specified destination]; otherwise, <c>false</c>.</returns>
        public static bool IsAfterOrEqual(this DateTime source, DateTime destination)
        {
            return DateTimeUtility.IsAfterOrEqual(source, destination);
        }

        /// <summary>
        ///     Determines whether [is after or equal] [the specified destination].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="redundancy">The redundancy.</param>
        /// <returns><c>true</c> if [is after or equal] [the specified destination]; otherwise, <c>false</c>.</returns>
        public static bool IsAfterOrEqual(this DateTime source, DateTime destination, TimeSpan redundancy)
        {
            return DateTimeUtility.IsAfterOrEqual(source, destination, redundancy);
        }

        /// <summary>
        ///     Determines whether the specified destination is before.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns><c>true</c> if the specified destination is before; otherwise, <c>false</c>.</returns>
        public static bool IsBefore(this DateTime source, DateTime destination)
        {
            return DateTimeUtility.IsBefore(source, destination);
        }

        /// <summary>
        ///     Determines whether the specified destination is before.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="redundancy">The redundancy.</param>
        /// <returns><c>true</c> if the specified destination is before; otherwise, <c>false</c>.</returns>
        public static bool IsBefore(this DateTime source, DateTime destination, TimeSpan redundancy)
        {
            return DateTimeUtility.IsBefore(source, destination, redundancy);
        }

        /// <summary>
        ///     Determines whether [is before or equal] [the specified destination].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns><c>true</c> if [is before or equal] [the specified destination]; otherwise, <c>false</c>.</returns>
        public static bool IsBeforeOrEqual(this DateTime source, DateTime destination)
        {
            return DateTimeUtility.IsBeforeOrEqual(source, destination);
        }

        /// <summary>
        ///     Determines whether [is before or equal] [the specified destination].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="redundancy">The redundancy.</param>
        /// <returns><c>true</c> if [is before or equal] [the specified destination]; otherwise, <c>false</c>.</returns>
        public static bool IsBeforeOrEqual(this DateTime source, DateTime destination, TimeSpan redundancy)
        {
            return DateTimeUtility.IsBeforeOrEqual(source, destination, redundancy);
        }

        /// <summary>
        ///     Gets the JS datetime of the specified date time.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>System.Int64.</returns>
        public static long JSDate(this DateTime dateTime)
        {
            return DateTimeUtility.GetJSDate(dateTime);
        }

        /// <summary>
        ///     Gets the unix timestamp of the specified date time.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>System.Int64.</returns>
        public static long UnixTimestamp(this DateTime dateTime)
        {
            return DateTimeUtility.GetUnixTimestamp(dateTime);
        }

        /// <summary>
        ///     Gets the UTC of the specified date time.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>DateTime.</returns>
        public static DateTime Utc(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime();
        }
    }

    /// <summary>
    ///     Utilities for working with <see cref="System.DateTime" /> types.
    /// </summary>
    public static class DateTimeUtility
    {
        /// <summary>
        ///     The ticks of 1970
        /// </summary>
        private const long EPOCH_TICKS = 621355968000000000;

        /// <summary>
        ///     The file time offset
        /// </summary>
        private const long FILE_TIME_OFFSET = 504911232000000000;

        /// <summary>
        ///     The datetime maximum value minus one day
        /// </summary>
        private static readonly DateTime MaxValueMinusOneDay = DateTime.MaxValue.AddDays(-1);

        /// <summary>
        ///     The datetime minimum value plus one day
        /// </summary>
        private static readonly DateTime MinValuePlusOneDay = DateTime.MinValue.AddDays(1);

        /// <summary>
        ///     Gets the china standard time zone.
        /// </summary>
        /// <value>The china standard time zone.</value>
        public static TimeZoneInfo ChinaStandardTimeZone { get; } = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");

        /// <summary>
        ///     Converts to local time.
        /// </summary>
        /// <param name="utcTime">The UTC time.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ConvertToLocalTime(DateTime utcTime)
        {
            if (utcTime < MinValuePlusOneDay)
            {
                return DateTime.MinValue;
            }

            return utcTime > MaxValueMinusOneDay ? DateTime.MaxValue : utcTime.ToLocalTime();
        }

        /// <summary>
        ///     Converts to universal time.
        /// </summary>
        /// <param name="localTime">The local time.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ConvertToUniversalTime(DateTime localTime)
        {
            if (localTime < MinValuePlusOneDay)
            {
                return DateTime.MinValue;
            }

            return localTime > MaxValueMinusOneDay ? DateTime.MaxValue : localTime.ToUniversalTime();
        }

        public static TimeSpan Days(this int value)
        {
            return new TimeSpan(value);
        }

        /// <summary>
        ///     Gets the UTC from the file time.
        /// </summary>
        /// <param name="fileTime">The file time.</param>
        /// <returns>DateTime.</returns>
        public static DateTime FromFileTime(long fileTime)
        {
            long universalTicks = fileTime + FILE_TIME_OFFSET;
            // Dev10 733288: Caching: behavior change for CacheDependency when using UseMemoryCache=1
            // ObjectCacheHost converts DateTime to a DateTimeOffset, and the conversion requires
            // that DateTimeKind be set correctly
            return new DateTime(universalTicks, DateTimeKind.Utc);
        }

        /// <summary>
        ///     Gets the UTC from the js date string.
        /// </summary>
        /// <param name="dateMillisecondsAfter1970">The date milliseconds after1970.</param>
        /// <returns>UTC DateTime.</returns>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static DateTime FromJSDate(long dateMillisecondsAfter1970)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddMilliseconds(dateMillisecondsAfter1970);
        }

        /// <summary>
        ///     Froms the string.
        /// </summary>
        /// <param name="value">The date string.</param>
        /// <returns>DateTime.</returns>
        public static DateTime FromString(string value)
        {
            return DateTime.Parse(value);
        }

        /// <summary>
        ///     Froms the ticks.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        /// <returns>DateTime.</returns>
        public static DateTime FromTicks(int ticks)
        {
            return new DateTime(ticks);
        }

        /// <summary>
        ///     Froms the ticks.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        /// <returns>DateTime.</returns>
        public static DateTime FromTicks(long ticks)
        {
            return new DateTime(ticks);
        }

        /// <summary>
        ///     Froms the unix timestamp.
        /// </summary>
        /// <param name="timestamp">The time stamp.</param>
        /// <returns>DateTime.</returns>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static DateTime FromUnixTimestamp(long timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddSeconds(timestamp);
        }

        /// <summary>
        ///     Gets the js date.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>System.Int64.</returns>
        public static long GetJSDate(DateTime time)
        {
            DateTime utc = time.ToUniversalTime();
            return (utc.Ticks - EPOCH_TICKS) / 10000;
        }

        /// <summary>
        ///     Gets the js date of now.
        /// </summary>
        /// <returns>System.Int64.</returns>
        public static long GetJSDate()
        {
            DateTime utc = DateTime.UtcNow;
            return GetJSDate(utc);
        }

        /// <summary>
        ///     Gets the unix time stamp.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>System.Int64.</returns>
        public static long GetUnixTimestamp(DateTime time)
        {
            DateTime utc = time.ToUniversalTime();
            return (utc.Ticks - EPOCH_TICKS) / 10000000;
        }

        /// <summary>
        ///     Gets the unix time stamp of now.
        /// </summary>
        /// <returns>System.Int64.</returns>
        public static long GetUnixTimestamp()
        {
            DateTime utc = DateTime.UtcNow;
            return GetUnixTimestamp(utc);
        }

        public static TimeSpan Hours(this int value)
        {
            return new TimeSpan(value, 0, 0);
        }

        public static bool IsAfter(DateTime source, DateTime destination, TimeSpan redundancy)
        {
            return source - destination > redundancy;
        }

        public static bool IsAfter(DateTime source, DateTime destination)
        {
            return source > destination;
        }

        public static bool IsAfterOrEqual(DateTime source, DateTime destination, TimeSpan redundancy)
        {
            return source - destination >= redundancy;
        }

        public static bool IsAfterOrEqual(DateTime source, DateTime destination)
        {
            return source >= destination;
        }

        public static bool IsBefore(DateTime source, DateTime destination, TimeSpan redundancy)
        {
            return destination - source > redundancy;
        }

        public static bool IsBefore(DateTime source, DateTime destination)
        {
            return destination > source;
        }

        public static bool IsBeforeOrEqual(DateTime source, DateTime destination)
        {
            return destination >= source;
        }

        public static bool IsBeforeOrEqual(DateTime source, DateTime destination, TimeSpan redundancy)
        {
            return destination - source >= redundancy;
        }

        /// <summary>
        ///     Determines whether [is in the day] [the specified date].
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="date">The date.</param>
        /// <returns><c>true</c> if [is in the day] [the specified date]; otherwise, <c>false</c>.</returns>
        public static bool IsInTheDay(this DateTime time, DateTime date)
        {
            return time >= date.Date && time < date.Date.AddDays(1);
        }

        public static TimeSpan Milliseconds(this int value)
        {
            return new TimeSpan(0, 0, 0, 0, value);
        }

        public static TimeSpan Minutes(this int value)
        {
            return new TimeSpan(0, 0, value, 0);
        }

        public static TimeSpan Seconds(this int value)
        {
            return new TimeSpan(0, 0, value);
        }

        /// <summary>
        ///     To the china standard time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToChinaStandardTime(this DateTime time)
        {
            time = time.ToUniversalTime().AddHours(8);
            return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Local);
        }

        /// <summary>
        ///     To the datetime.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToDateTime(this int ticks)
        {
            return ((long)ticks).ToDateTime();
        }

        /// <summary>
        ///     To the datetime.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        /// <returns>DateTime.</returns>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public static DateTime ToDateTime(this long ticks)
        {
            return new DateTime(ticks);
        }

        /// <summary>
        ///     To the UTC from file time.
        /// </summary>
        /// <param name="fileTime">The file time.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToDateTimeFromFileTime(this long fileTime)
        {
            return FromFileTime(fileTime);
        }

        /// <summary>
        ///     To the UTC from js date.
        /// </summary>
        /// <param name="dateMillisecondsAfter1970">The date milliseconds after1970.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToDateTimeFromJSDate(this long dateMillisecondsAfter1970)
        {
            return FromJSDate(dateMillisecondsAfter1970);
        }

        /// <summary>
        ///     To the UTC from unix timestamp.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToDateTimeFromUnixTimestamp(this long timestamp)
        {
            return FromUnixTimestamp(timestamp);
        }
    }
}