using System.Buffers;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Navyblue.BaseLibrary.Extensions;

public static class SpanExtensions
{
    public static bool IsWhiteSpace(this ReadOnlySpan<char> value) => value.Trim().IsEmpty;
    public static bool IsNullOrWhiteSpace(this ReadOnlySpan<char> value) => value.Trim().IsEmpty;

    public static bool EqualsOrdinal(this ReadOnlySpan<char> value, ReadOnlySpan<char> other)
    {
        return value.Equals(other, StringComparison.Ordinal);
    }

    public static bool EqualsOrdinalIgnoreCase(this ReadOnlySpan<char> value, ReadOnlySpan<char> other)
    {
        return value.Equals(other, StringComparison.OrdinalIgnoreCase);
    }

    public static bool ContainsAny(this ReadOnlySpan<char> value, SearchValues<char> searchValues)
    {
        ArgumentNullException.ThrowIfNull(searchValues);
        return value.ContainsAny(searchValues);
    }

    public static bool TryParseInt32(this ReadOnlySpan<char> value, out int result, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
    {
        return int.TryParse(value, style, provider, out result);
    }

    public static bool TryParseInt64(this ReadOnlySpan<char> value, out long result, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
    {
        return long.TryParse(value, style, provider, out result);
    }

    public static bool TryParseDecimal(this ReadOnlySpan<char> value, out decimal result, NumberStyles style = NumberStyles.Number, IFormatProvider? provider = null)
    {
        return decimal.TryParse(value, style, provider, out result);
    }

    public static bool TryParseGuid(this ReadOnlySpan<char> value, out Guid result)
    {
        return Guid.TryParse(value, out result);
    }

    public static SpanSplitEnumerator Split(this ReadOnlySpan<char> value, char separator, StringSplitOptions options = StringSplitOptions.None)
    {
        return new SpanSplitEnumerator(value, separator, options);
    }

    public static SpanLineEnumerator EnumerateLines(this ReadOnlySpan<char> value, bool trimLineEnd = true)
    {
        return new SpanLineEnumerator(value, trimLineEnd);
    }

    public static string ToUtf8String(this ReadOnlySpan<byte> value)
    {
        return Encoding.UTF8.GetString(value);
    }

    public static string ToHexStringLower(this ReadOnlySpan<byte> value)
    {
        return Convert.ToHexString(value).ToLowerInvariant();
    }

    public static string ToHexStringUpper(this ReadOnlySpan<byte> value)
    {
        return Convert.ToHexString(value);
    }

    public static string ToBase64String(this ReadOnlySpan<byte> value)
    {
        return Convert.ToBase64String(value);
    }

    public static bool FixedTimeEquals(this ReadOnlySpan<byte> value, ReadOnlySpan<byte> other)
    {
        return value.Length == other.Length && CryptographicOperations.FixedTimeEquals(value, other);
    }

    public static byte[] Sha256(this ReadOnlySpan<byte> value)
    {
        return SHA256.HashData(value);
    }

    public static byte[] HmacSha256(this ReadOnlySpan<byte> value, ReadOnlySpan<byte> key)
    {
        return HMACSHA256.HashData(key, value);
    }
}

public ref struct SpanSplitEnumerator
{
    private ReadOnlySpan<char> _remaining;
    private readonly char _separator;
    private readonly StringSplitOptions _options;
    private bool _completed;

    public SpanSplitEnumerator(ReadOnlySpan<char> value, char separator, StringSplitOptions options)
    {
        _remaining = value;
        _separator = separator;
        _options = options;
        _completed = false;
        Current = default;
    }

    public ReadOnlySpan<char> Current { get; private set; }
    public readonly SpanSplitEnumerator GetEnumerator() => this;

    public bool MoveNext()
    {
        if (_completed)
        {
            return false;
        }

        while (true)
        {
            var index = _remaining.IndexOf(_separator);
            ReadOnlySpan<char> segment;
            if (index < 0)
            {
                segment = _remaining;
                _remaining = default;
                _completed = true;
            }
            else
            {
                segment = _remaining[..index];
                _remaining = _remaining[(index + 1)..];
            }

            if ((_options & StringSplitOptions.TrimEntries) != 0)
            {
                segment = segment.Trim();
            }

            if ((_options & StringSplitOptions.RemoveEmptyEntries) != 0 && segment.IsEmpty)
            {
                if (_completed)
                {
                    return false;
                }

                continue;
            }

            Current = segment;
            return true;
        }
    }
}

public ref struct SpanLineEnumerator
{
    private ReadOnlySpan<char> _remaining;
    private readonly bool _trimLineEnd;
    private bool _completed;

    public SpanLineEnumerator(ReadOnlySpan<char> value, bool trimLineEnd)
    {
        _remaining = value;
        _trimLineEnd = trimLineEnd;
        _completed = false;
        Current = default;
    }

    public ReadOnlySpan<char> Current { get; private set; }
    public readonly SpanLineEnumerator GetEnumerator() => this;

    public bool MoveNext()
    {
        if (_completed)
        {
            return false;
        }

        var index = _remaining.IndexOf('\n');
        ReadOnlySpan<char> line;
        if (index < 0)
        {
            line = _remaining;
            _remaining = default;
            _completed = true;
        }
        else
        {
            line = _remaining[..index];
            _remaining = _remaining[(index + 1)..];
        }

        if (_trimLineEnd && !line.IsEmpty && line[^1] == '\r')
        {
            line = line[..^1];
        }

        Current = line;
        return true;
    }
}
