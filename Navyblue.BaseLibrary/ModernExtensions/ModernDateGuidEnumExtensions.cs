// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernDateGuidEnumExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:49
// ****************************************************************************************************************************************
// <copyright file="ModernDateGuidEnumExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
namespace Navyblue.BaseLibrary.Extensions;

public static class ModernDateTimeExtensions
{
    public static long ToUnixTimeMilliseconds(this DateTimeOffset value) => value.ToUniversalTime().ToUnixTimeMilliseconds();
    public static DateTimeOffset StartOfDay(this DateTimeOffset value) => new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset);
    public static DateTimeOffset EndOfDay(this DateTimeOffset value) => value.StartOfDay().AddDays(1).AddTicks(-1);
    public static bool IsBetween(this DateTimeOffset value, DateTimeOffset start, DateTimeOffset end, bool inclusive = true) => inclusive ? value >= start && value <= end : value > start && value < end;
}

public static class ModernGuidExtensions
{
    public static bool IsEmpty(this Guid value) => value == Guid.Empty;
    public static bool IsNotEmpty(this Guid value) => value != Guid.Empty;
    public static string ToNString(this Guid value) => value.ToString("N");
}

public static class ModernIntExtensions
{
    public static bool Between(this int value, int min, int max) => value >= min && value <= max;
    public static int ClampTo(this int value, int min, int max) => Math.Min(Math.Max(value, min), max);
}

public static class ModernEnumExtensions
{
    public static string GetName<TEnum>(this TEnum value) where TEnum : struct, Enum => Enum.GetName(typeof(TEnum), value) ?? value.ToString();
    public static bool HasFlagFast<TEnum>(this TEnum value, TEnum flag) where TEnum : struct, Enum => (Convert.ToUInt64(value) & Convert.ToUInt64(flag)) == Convert.ToUInt64(flag);
}