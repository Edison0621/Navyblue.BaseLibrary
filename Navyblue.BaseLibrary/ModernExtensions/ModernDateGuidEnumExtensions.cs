// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernDateGuidEnumExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:00
// ****************************************************************************************************************************************
// <copyright file="ModernDateGuidEnumExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
/// </summary>
public static class ModernDateTimeExtensions
{
    /// <summary>
    ///     Converts to unixtimemilliseconds.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static long ToUnixTimeMilliseconds(this DateTimeOffset value) => value.ToUniversalTime().ToUnixTimeMilliseconds();

    /// <summary>
    ///     Starts the of day.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static DateTimeOffset StartOfDay(this DateTimeOffset value) => new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset);

    /// <summary>
    ///     Ends the of day.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static DateTimeOffset EndOfDay(this DateTimeOffset value) => value.StartOfDay().AddDays(1).AddTicks(-1);

    /// <summary>
    ///     Determines whether the specified start is between.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <param name="inclusive">if set to <c>true</c> [inclusive].</param>
    /// <returns>
    ///     <c>true</c> if the specified start is between; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsBetween(this DateTimeOffset value, DateTimeOffset start, DateTimeOffset end, bool inclusive = true) => inclusive ? value >= start && value <= end : value > start && value < end;
}

/// <summary>
/// </summary>
public static class ModernGuidExtensions
{
    /// <summary>
    ///     Determines whether this instance is empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     <c>true</c> if the specified value is empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsEmpty(this Guid value) => value == Guid.Empty;

    /// <summary>
    ///     Determines whether [is not empty].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     <c>true</c> if [is not empty] [the specified value]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNotEmpty(this Guid value) => value != Guid.Empty;

    /// <summary>
    ///     Converts to nstring.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToNString(this Guid value) => value.ToString("N");
}

/// <summary>
/// </summary>
public static class ModernIntExtensions
{
    /// <summary>
    ///     Betweens the specified minimum.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <returns></returns>
    public static bool Between(this int value, int min, int max) => value >= min && value <= max;

    /// <summary>
    ///     Clamps to.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <returns></returns>
    public static int ClampTo(this int value, int min, int max) => Math.Min(Math.Max(value, min), max);
}

/// <summary>
/// </summary>
public static class ModernEnumExtensions
{
    /// <summary>
    ///     Gets the name.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string GetName<TEnum>(this TEnum value) where TEnum : struct, Enum => Enum.GetName(typeof(TEnum), value) ?? value.ToString();

    /// <summary>
    ///     Determines whether [has flag fast] [the specified flag].
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="flag">The flag.</param>
    /// <returns>
    ///     <c>true</c> if [has flag fast] [the specified flag]; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasFlagFast<TEnum>(this TEnum value, TEnum flag) where TEnum : struct, Enum => (Convert.ToUInt64(value) & Convert.ToUInt64(flag)) == Convert.ToUInt64(flag);
}