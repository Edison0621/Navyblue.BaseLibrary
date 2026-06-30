// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernSpanExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:49
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

public static class ModernSpanExtensions
{
    public static bool IsWhiteSpace(this ReadOnlySpan<char> value) => value.Trim().IsEmpty;
    public static bool IsNullOrWhiteSpace(this ReadOnlySpan<char> value) => value.Trim().IsEmpty;
    public static bool EqualsOrdinal(this ReadOnlySpan<char> value, ReadOnlySpan<char> other) => value.Equals(other, StringComparison.Ordinal);
    public static bool EqualsOrdinalIgnoreCase(this ReadOnlySpan<char> value, ReadOnlySpan<char> other) => value.Equals(other, StringComparison.OrdinalIgnoreCase);
    public static bool ContainsAny(this ReadOnlySpan<char> value, ReadOnlySpan<char> anyOf) => value.IndexOfAny(anyOf) >= 0;
    public static bool TryParseInt32(this ReadOnlySpan<char> value, out int result, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null) => int.TryParse(value, style, provider, out result);
    public static bool TryParseInt64(this ReadOnlySpan<char> value, out long result, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null) => long.TryParse(value, style, provider, out result);
    public static bool TryParseDecimal(this ReadOnlySpan<char> value, out decimal result, NumberStyles style = NumberStyles.Number, IFormatProvider? provider = null) => decimal.TryParse(value, style, provider, out result);
    public static bool TryParseGuid(this ReadOnlySpan<char> value, out Guid result) => Guid.TryParse(value, out result);
    public static ModernSpanSplitEnumerator Split(this ReadOnlySpan<char> value, char separator, StringSplitOptions options = StringSplitOptions.None) => new ModernSpanSplitEnumerator(value, separator, options);
    public static ModernSpanLineEnumerator EnumerateLines(this ReadOnlySpan<char> value, bool trimLineEnd = true) => new ModernSpanLineEnumerator(value, trimLineEnd);
    public static string ToUtf8String(this ReadOnlySpan<byte> value) => Encoding.UTF8.GetString(value);
    public static string ToHexStringLower(this ReadOnlySpan<byte> value) => Convert.ToHexString(value).ToLowerInvariant();
    public static string ToHexStringUpper(this ReadOnlySpan<byte> value) => Convert.ToHexString(value);
    public static string ToBase64String(this ReadOnlySpan<byte> value) => Convert.ToBase64String(value);
    public static bool FixedTimeEquals(this ReadOnlySpan<byte> value, ReadOnlySpan<byte> other) => value.Length == other.Length && CryptographicOperations.FixedTimeEquals(value, other);
    public static byte[] Sha256(this ReadOnlySpan<byte> value) => SHA256.HashData(value);
    public static byte[] HmacSha256(this ReadOnlySpan<byte> value, ReadOnlySpan<byte> key) => HMACSHA256.HashData(key, value);
}

public ref struct ModernSpanSplitEnumerator(ReadOnlySpan<char> value, char separator, StringSplitOptions options)
{
    private ReadOnlySpan<char> _remaining = value;
    private bool _completed = false;

    public ReadOnlySpan<char> Current { get; private set; } = default;
    public readonly ModernSpanSplitEnumerator GetEnumerator() => this;

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

public ref struct ModernSpanLineEnumerator(ReadOnlySpan<char> value, bool trimLineEnd)
{
    private ReadOnlySpan<char> _remaining = value;
    private bool _completed = false;

    public ReadOnlySpan<char> Current { get; private set; } = default;
    public readonly ModernSpanLineEnumerator GetEnumerator() => this;

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