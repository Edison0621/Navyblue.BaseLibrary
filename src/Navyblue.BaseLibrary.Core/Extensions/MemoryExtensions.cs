using System.Text;

namespace Navyblue.BaseLibrary.Extensions;

public static class ReadOnlyMemoryExtensions
{
    public static string ToUtf8String(this ReadOnlyMemory<byte> value)
    {
        return Encoding.UTF8.GetString(value.Span);
    }

    public static byte[] ToArraySafe(this ReadOnlyMemory<byte> value)
    {
        return value.IsEmpty ? [] : value.ToArray();
    }

    public static Stream ToReadOnlyStream(this ReadOnlyMemory<byte> value)
    {
        return new MemoryStream(value.ToArray(), writable: false);
    }

    public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ReadOnlyMemory<T> value)
    {
        return value.Span;
    }
}
