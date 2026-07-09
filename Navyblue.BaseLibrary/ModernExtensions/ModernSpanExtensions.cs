// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernSpanExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:00
// ****************************************************************************************************************************************
// <copyright file="ModernSpanExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable

using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
/// </summary>
public ref struct ModernSpanSplitEnumerator(ReadOnlySpan<char> value, char separator, StringSplitOptions options)
{
    private bool _completed = false;
    private ReadOnlySpan<char> _remaining = value;

    /// <summary>
    ///     Gets the current.
    /// </summary>
    /// <value>
    ///     The current.
    /// </value>
    public ReadOnlySpan<char> Current { get; private set; } = default;

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public readonly ModernSpanSplitEnumerator GetEnumerator() => this;

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public bool MoveNext()
    {
        if (this._completed) return false;

        while (true)
        {
            int index = this._remaining.IndexOf(separator);
            ReadOnlySpan<char> segment;
            if (index < 0)
            {
                segment = this._remaining;
                this._remaining = default;
                this._completed = true;
            }
            else
            {
                segment = this._remaining.Slice(0, index);
                this._remaining = this._remaining.Slice(index + 1);
            }

            if ((options & StringSplitOptions.TrimEntries) != 0) segment = segment.Trim();
            if ((options & StringSplitOptions.RemoveEmptyEntries) != 0 && segment.IsEmpty)
            {
                if (this._completed) return false;
                continue;
            }

            this.Current = segment;
            return true;
        }
    }
}

/// <summary>
/// </summary>
public static class ModernSpanExtensions
{
    /// <summary>
    ///     Determines whether the specified any of contains any.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="anyOf">Any of.</param>
    /// <returns>
    ///     <c>true</c> if the specified any of contains any; otherwise, <c>false</c>.
    /// </returns>
    public static bool ContainsAny(this ReadOnlySpan<char> value, ReadOnlySpan<char> anyOf) => value.IndexOfAny(anyOf) >= 0;

    /// <summary>
    ///     Enumerates the lines.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="trimLineEnd">if set to <c>true</c> [trim line end].</param>
    /// <returns></returns>
    public static ModernSpanLineEnumerator EnumerateLines(this ReadOnlySpan<char> value, bool trimLineEnd = true) => new ModernSpanLineEnumerator(value, trimLineEnd);

    /// <summary>
    ///     Equalses the ordinal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns></returns>
    public static bool EqualsOrdinal(this ReadOnlySpan<char> value, ReadOnlySpan<char> other) => value.Equals(other, StringComparison.Ordinal);

    /// <summary>
    ///     Equalses the ordinal ignore case.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns></returns>
    public static bool EqualsOrdinalIgnoreCase(this ReadOnlySpan<char> value, ReadOnlySpan<char> other) => value.Equals(other, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     Fixeds the time equals.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns></returns>
    public static bool FixedTimeEquals(this ReadOnlySpan<byte> value, ReadOnlySpan<byte> other) => value.Length == other.Length && CryptographicOperations.FixedTimeEquals(value, other);

    /// <summary>
    ///     Hmacs the sha256.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public static byte[] HmacSha256(this ReadOnlySpan<byte> value, ReadOnlySpan<byte> key) => HMACSHA256.HashData(key, value);

    /// <summary>
    ///     Determines whether [is null or white space].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     <c>true</c> if [is null or white space] [the specified value]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrWhiteSpace(this ReadOnlySpan<char> value) => value.Trim().IsEmpty;

    /// <summary>
    ///     Determines whether [is white space].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     <c>true</c> if [is white space] [the specified value]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsWhiteSpace(this ReadOnlySpan<char> value) => value.Trim().IsEmpty;

    /// <summary>
    ///     Sha256s the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static byte[] Sha256(this ReadOnlySpan<byte> value) => SHA256.HashData(value);

    /// <summary>
    ///     Splits the specified separator.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public static ModernSpanSplitEnumerator Split(this ReadOnlySpan<char> value, char separator, StringSplitOptions options = StringSplitOptions.None) => new ModernSpanSplitEnumerator(value, separator, options);

    /// <summary>
    ///     Converts to base64string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToBase64String(this ReadOnlySpan<byte> value) => Convert.ToBase64String(value);

    /// <summary>
    ///     Converts to hexstringlower.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToHexStringLower(this ReadOnlySpan<byte> value) => Convert.ToHexString(value).ToLowerInvariant();

    /// <summary>
    ///     Converts to hexstringupper.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToHexStringUpper(this ReadOnlySpan<byte> value) => Convert.ToHexString(value);

    /// <summary>
    ///     Converts to utf8string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToUtf8String(this ReadOnlySpan<byte> value) => Encoding.UTF8.GetString(value);

    /// <summary>
    ///     Tries the parse decimal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <param name="style">The style.</param>
    /// <param name="provider">The provider.</param>
    /// <returns></returns>
    public static bool TryParseDecimal(this ReadOnlySpan<char> value, out decimal result, NumberStyles style = NumberStyles.Number, IFormatProvider? provider = null) => decimal.TryParse(value, style, provider, out result);

    /// <summary>
    ///     Tries the parse unique identifier.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    public static bool TryParseGuid(this ReadOnlySpan<char> value, out Guid result) => Guid.TryParse(value, out result);

    /// <summary>
    ///     Tries the parse int32.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <param name="style">The style.</param>
    /// <param name="provider">The provider.</param>
    /// <returns></returns>
    public static bool TryParseInt32(this ReadOnlySpan<char> value, out int result, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null) => int.TryParse(value, style, provider, out result);

    /// <summary>
    ///     Tries the parse int64.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <param name="style">The style.</param>
    /// <param name="provider">The provider.</param>
    /// <returns></returns>
    public static bool TryParseInt64(this ReadOnlySpan<char> value, out long result, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null) => long.TryParse(value, style, provider, out result);
}

/// <summary>
/// </summary>
public ref struct ModernSpanLineEnumerator(ReadOnlySpan<char> value, bool trimLineEnd)
{
    private bool _completed = false;
    private ReadOnlySpan<char> _remaining = value;

    /// <summary>
    ///     Gets the current.
    /// </summary>
    /// <value>
    ///     The current.
    /// </value>
    public ReadOnlySpan<char> Current { get; private set; } = default;

    /// <summary>
    ///     Gets the enumerator.
    /// </summary>
    /// <returns></returns>
    public readonly ModernSpanLineEnumerator GetEnumerator() => this;

    /// <summary>
    ///     Moves the next.
    /// </summary>
    /// <returns></returns>
    public bool MoveNext()
    {
        if (this._completed) return false;

        int index = this._remaining.IndexOf('\n');
        ReadOnlySpan<char> line;
        if (index < 0)
        {
            line = this._remaining;
            this._remaining = default;
            this._completed = true;
        }
        else
        {
            line = this._remaining.Slice(0, index);
            this._remaining = this._remaining.Slice(index + 1);
        }

        if (trimLineEnd && !line.IsEmpty && line[line.Length - 1] == '\r') line = line.Slice(0, line.Length - 1);
        this.Current = line;
        return true;
    }
}