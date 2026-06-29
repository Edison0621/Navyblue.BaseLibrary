// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : SpanExtensions.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-29  13:02
// ****************************************************************************************************************************************
// <copyright file="SpanExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Buffers;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
///     The span extensions.
/// </summary>
public static class SpanExtensions
{
    /// <summary>
    ///     Checks if is white space.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A bool</returns>
    public static bool IsWhiteSpace(this ReadOnlySpan<char> value) => value.Trim().IsEmpty;

    /// <summary>
    ///     Checks if is null or white space.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A bool</returns>
    public static bool IsNullOrWhiteSpace(this ReadOnlySpan<char> value) => value.Trim().IsEmpty;

    /// <summary>
    ///     Equals ordinal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns>A bool</returns>
    public static bool EqualsOrdinal(this ReadOnlySpan<char> value, ReadOnlySpan<char> other)
    {
        return value.Equals(other, StringComparison.Ordinal);
    }

    /// <summary>
    ///     Equals ordinal ignore case.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns>A bool</returns>
    public static bool EqualsOrdinalIgnoreCase(this ReadOnlySpan<char> value, ReadOnlySpan<char> other)
    {
        return value.Equals(other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Contains any.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="searchValues">The search values.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A bool</returns>
    // ReSharper disable once FunctionRecursiveOnAllPaths
    public static bool ContainsAny(this ReadOnlySpan<char> value, SearchValues<char> searchValues)
    {
        ArgumentNullException.ThrowIfNull(searchValues);
        // ReSharper disable once TailRecursiveCall
        return value.ContainsAny(searchValues);
    }

    /// <summary>
    ///     Try parse int32.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <param name="style">The style.</param>
    /// <param name="provider">The provider.</param>
    /// <returns>A bool</returns>
    public static bool TryParseInt32(this ReadOnlySpan<char> value, out int result, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
    {
        return int.TryParse(value, style, provider, out result);
    }

    /// <summary>
    ///     Try parse int64.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <param name="style">The style.</param>
    /// <param name="provider">The provider.</param>
    /// <returns>A bool</returns>
    public static bool TryParseInt64(this ReadOnlySpan<char> value, out long result, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
    {
        return long.TryParse(value, style, provider, out result);
    }

    /// <summary>
    ///     Try parse decimal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <param name="style">The style.</param>
    /// <param name="provider">The provider.</param>
    /// <returns>A bool</returns>
    public static bool TryParseDecimal(this ReadOnlySpan<char> value, out decimal result, NumberStyles style = NumberStyles.Number, IFormatProvider? provider = null)
    {
        return decimal.TryParse(value, style, provider, out result);
    }

    /// <summary>
    ///     Try parse guid.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns>A bool</returns>
    public static bool TryParseGuid(this ReadOnlySpan<char> value, out Guid result)
    {
        return Guid.TryParse(value, out result);
    }

    /// <summary>
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="options">The options.</param>
    /// <returns>A SpanSplitEnumerator</returns>
    public static SpanSplitEnumerator Split(this ReadOnlySpan<char> value, char separator, StringSplitOptions options = StringSplitOptions.None)
    {
        return new SpanSplitEnumerator(value, separator, options);
    }

    /// <summary>
    ///     Enumerates the lines.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="trimLineEnd">If true, trim line end.</param>
    /// <returns>A SpanLineEnumerator</returns>
    public static SpanLineEnumerator EnumerateLines(this ReadOnlySpan<char> value, bool trimLineEnd = true)
    {
        return new SpanLineEnumerator(value, trimLineEnd);
    }

    /// <summary>
    ///     Converts to utf8 string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A string</returns>
    public static string ToUtf8String(this ReadOnlySpan<byte> value)
    {
        return Encoding.UTF8.GetString(value);
    }

    /// <summary>
    ///     Converts to hex string lower.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A string</returns>
    public static string ToHexStringLower(this ReadOnlySpan<byte> value)
    {
        return Convert.ToHexString(value).ToLowerInvariant();
    }

    /// <summary>
    ///     Converts to hex string upper.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A string</returns>
    public static string ToHexStringUpper(this ReadOnlySpan<byte> value)
    {
        return Convert.ToHexString(value);
    }

    /// <summary>
    ///     Converts to base64 string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A string</returns>
    public static string ToBase64String(this ReadOnlySpan<byte> value)
    {
        return Convert.ToBase64String(value);
    }

    /// <summary>
    ///     Fixed time equals.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="other">The other.</param>
    /// <returns>A bool</returns>
    public static bool FixedTimeEquals(this ReadOnlySpan<byte> value, ReadOnlySpan<byte> other)
    {
        return value.Length == other.Length && CryptographicOperations.FixedTimeEquals(value, other);
    }

    /// <summary>
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>An array of byte</returns>
    public static byte[] Sha256(this ReadOnlySpan<byte> value)
    {
        return SHA256.HashData(value);
    }

    /// <summary>
    ///     Hmacs the sha256.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="key">The key.</param>
    /// <returns>An array of byte</returns>
    public static byte[] HmacSha256(this ReadOnlySpan<byte> value, ReadOnlySpan<byte> key)
    {
        return HMACSHA256.HashData(key, value);
    }
}

/// <summary>
///     The span split enumerator.
/// </summary>
public ref struct SpanSplitEnumerator
{
    private ReadOnlySpan<char> _remaining;
    private readonly char _separator;
    private readonly StringSplitOptions _options;
    private bool _completed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SpanSplitEnumerator" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="options">The options.</param>
    public SpanSplitEnumerator(ReadOnlySpan<char> value, char separator, StringSplitOptions options)
    {
        this._remaining = value;
        this._separator = separator;
        this._options = options;
        this._completed = false;
        this.Current = default;
    }

    /// <summary>
    ///     Gets the current.
    /// </summary>
    public ReadOnlySpan<char> Current { get; private set; }

    /// <summary>
    ///     Get the enumerator.
    /// </summary>
    /// <returns>A SpanSplitEnumerator</returns>
    public readonly SpanSplitEnumerator GetEnumerator() => this;

    /// <summary>
    ///     Move next.
    /// </summary>
    /// <returns>A bool</returns>
    public bool MoveNext()
    {
        if (this._completed)
        {
            return false;
        }

        while (true)
        {
            int index = this._remaining.IndexOf(this._separator);
            ReadOnlySpan<char> segment;
            if (index < 0)
            {
                segment = this._remaining;
                this._remaining = default;
                this._completed = true;
            }
            else
            {
                segment = this._remaining[..index];
                this._remaining = this._remaining[(index + 1)..];
            }

            if ((this._options & StringSplitOptions.TrimEntries) != 0)
            {
                segment = segment.Trim();
            }

            if ((this._options & StringSplitOptions.RemoveEmptyEntries) != 0 && segment.IsEmpty)
            {
                if (this._completed)
                {
                    return false;
                }

                continue;
            }

            this.Current = segment;
            return true;
        }
    }
}

/// <summary>
///     The span line enumerator.
/// </summary>
public ref struct SpanLineEnumerator
{
    private ReadOnlySpan<char> _remaining;
    private readonly bool _trimLineEnd;
    private bool _completed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SpanLineEnumerator" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="trimLineEnd">If true, trim line end.</param>
    public SpanLineEnumerator(ReadOnlySpan<char> value, bool trimLineEnd)
    {
        this._remaining = value;
        this._trimLineEnd = trimLineEnd;
        this._completed = false;
        this.Current = default;
    }

    /// <summary>
    ///     Gets the current.
    /// </summary>
    public ReadOnlySpan<char> Current { get; private set; }

    /// <summary>
    ///     Get the enumerator.
    /// </summary>
    /// <returns>A SpanLineEnumerator</returns>
    public readonly SpanLineEnumerator GetEnumerator() => this;

    /// <summary>
    ///     Move next.
    /// </summary>
    /// <returns>A bool</returns>
    public bool MoveNext()
    {
        if (this._completed)
        {
            return false;
        }

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
            line = this._remaining[..index];
            this._remaining = this._remaining[(index + 1)..];
        }

        if (this._trimLineEnd && !line.IsEmpty && line[^1] == '\r')
        {
            line = line[..^1];
        }

        this.Current = line;
        return true;
    }
}