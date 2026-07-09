// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : FileUtilities.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="FileUtilities.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Security.Cryptography;

namespace Navyblue.BaseLibrary.Files;

/// <summary>
///     The file name sanitizer.
/// </summary>
public static class FileNameSanitizer
{
    /// <summary>
    ///     The invalid file name chars
    /// </summary>
    private static readonly char[] _invalidFileNameChars = Path.GetInvalidFileNameChars();

    /// <summary>
    ///     Sanitizes the specified file name.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="replacement">The replacement.</param>
    /// <returns>
    ///     A string
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public static string Sanitize(string fileName, string replacement = "_")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        string sanitized = _invalidFileNameChars.Aggregate(fileName.Trim(), (current, invalid) => current.Replace(invalid.ToString(), replacement));
        return string.IsNullOrWhiteSpace(sanitized) ? "file" : sanitized;
    }
}

/// <summary>
///     The file size formatter.
/// </summary>
public static class FileSizeFormatter
{
    /// <summary>
    ///     The units
    /// </summary>
    private static readonly string[] _units = ["B", "KB", "MB", "GB", "TB"];

    /// <summary>
    ///     Formats the specified bytes.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <returns>
    ///     A string
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">bytes</exception>
    public static string Format(long bytes)
    {
        if (bytes < 0) throw new ArgumentOutOfRangeException(nameof(bytes));
        double value = bytes;
        int unit = 0;
        while (value >= 1024 && unit < _units.Length - 1)
        {
            value /= 1024;
            unit++;
        }

        return $"{value:0.##} {_units[unit]}";
    }
}

/// <summary>
///     The stream utilities.
/// </summary>
public static class StreamUtilities
{
    /// <summary>
    ///     Compute sha256 hex asynchronously.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     <![CDATA[Task<string>]]>
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task<string> ComputeSha256HexAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        byte[] hash = await SHA256.HashDataAsync(stream, cancellationToken).ConfigureAwait(false);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    ///     Reads all bytes asynchronously.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     <![CDATA[Task<byte[]>]]>
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task<byte[]> ReadAllBytesAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        using MemoryStream memory = new MemoryStream();
        await stream.CopyToAsync(memory, cancellationToken).ConfigureAwait(false);
        return memory.ToArray();
    }
}

/// <summary>
///     The mime type map.
/// </summary>
public static class MimeTypeMap
{
    /// <summary>
    ///     The types
    /// </summary>
    private static readonly IReadOnlyDictionary<string, string> _types = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
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

    /// <summary>
    ///     Get mime type.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <returns>
    ///     A string
    /// </returns>
    public static string GetMimeType(string fileName)
    {
        string extension = Path.GetExtension(fileName);
        return !string.IsNullOrWhiteSpace(extension) && _types.TryGetValue(extension, out string? mimeType)
            ? mimeType
            : "application/octet-stream";
    }
}