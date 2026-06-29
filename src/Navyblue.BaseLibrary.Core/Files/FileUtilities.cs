using System.Security.Cryptography;

namespace Navyblue.BaseLibrary.Files;

public static class FileNameSanitizer
{
    private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

    public static string Sanitize(string fileName, string replacement = "_")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        var sanitized = InvalidFileNameChars.Aggregate(fileName.Trim(), (current, invalid) => current.Replace(invalid.ToString(), replacement));
        return string.IsNullOrWhiteSpace(sanitized) ? "file" : sanitized;
    }
}

public static class FileSizeFormatter
{
    private static readonly string[] Units = ["B", "KB", "MB", "GB", "TB"];

    public static string Format(long bytes)
    {
        if (bytes < 0) throw new ArgumentOutOfRangeException(nameof(bytes));
        double value = bytes;
        var unit = 0;
        while (value >= 1024 && unit < Units.Length - 1)
        {
            value /= 1024;
            unit++;
        }

        return $"{value:0.##} {Units[unit]}";
    }
}

public static class StreamUtilities
{
    public static async Task<string> ComputeSha256HexAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken).ConfigureAwait(false);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public static async Task<byte[]> ReadAllBytesAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory, cancellationToken).ConfigureAwait(false);
        return memory.ToArray();
    }
}

public static class MimeTypeMap
{
    private static readonly IReadOnlyDictionary<string, string> Types = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        [".txt"] = "text/plain",
        [".json"] = "application/json",
        [".xml"] = "application/xml",
        [".csv"] = "text/csv",
        [".pdf"] = "application/pdf",
        [".zip"] = "application/zip",
        [".png"] = "image/png",
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".gif"] = "image/gif",
        [".webp"] = "image/webp",
        [".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    };

    public static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return !string.IsNullOrWhiteSpace(extension) && Types.TryGetValue(extension, out var mimeType)
            ? mimeType
            : "application/octet-stream";
    }
}
