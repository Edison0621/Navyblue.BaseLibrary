// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernIOExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:49
// ****************************************************************************************************************************************
// <copyright file="ModernIOExtensions.cs" company="">
//     Copyright (c) 2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
using System.Text;

namespace Navyblue.BaseLibrary.Extensions;

public static class ModernStreamExtensions
{
    public static async Task<byte[]> ReadAllBytesAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (stream is MemoryStream memoryStream && memoryStream.TryGetBuffer(out ArraySegment<byte> buffer))
        {
            return buffer.AsSpan().ToArray();
        }

        using MemoryStream destination = new MemoryStream();
        await stream.CopyToAsync(destination, 81920, cancellationToken).ConfigureAwait(false);
        return destination.ToArray();
    }

    public static async Task<string> ReadAllTextAsync(this Stream stream, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        using StreamReader reader = new StreamReader(stream, encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        cancellationToken.ThrowIfCancellationRequested();
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    public static async Task<MemoryStream> CopyToMemoryStreamAsync(this Stream stream, bool resetSourcePosition = false, CancellationToken cancellationToken = default)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (resetSourcePosition && stream.CanSeek) stream.Position = 0;
        MemoryStream destination = new MemoryStream();
        await stream.CopyToAsync(destination, 81920, cancellationToken).ConfigureAwait(false);
        destination.Position = 0;
        return destination;
    }

    public static Stream ResetPositionIfCanSeek(this Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (stream.CanSeek) stream.Position = 0;
        return stream;
    }
}

public static class ModernFileInfoExtensions
{
    public static Task<string> ReadAllTextAsync(this FileInfo file, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        return File.ReadAllTextAsync(file.FullName, encoding ?? Encoding.UTF8, cancellationToken);
    }

    public static Task<byte[]> ReadAllBytesAsync(this FileInfo file, CancellationToken cancellationToken = default)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        return File.ReadAllBytesAsync(file.FullName, cancellationToken);
    }

    public static async Task WriteAllTextAsync(this FileInfo file, string content, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        Directory.CreateDirectory(file.DirectoryName ?? ".");
        await File.WriteAllTextAsync(file.FullName, content, encoding ?? Encoding.UTF8, cancellationToken).ConfigureAwait(false);
    }

    public static async Task WriteAllBytesAsync(this FileInfo file, byte[] content, CancellationToken cancellationToken = default)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (content == null) throw new ArgumentNullException(nameof(content));
        Directory.CreateDirectory(file.DirectoryName ?? ".");
        await File.WriteAllBytesAsync(file.FullName, content, cancellationToken).ConfigureAwait(false);
    }

    public static bool HasExtension(this FileInfo file, string extension, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (extension == null) throw new ArgumentNullException(nameof(extension));
        string normalized = extension.StartsWith(".", StringComparison.Ordinal) ? extension : "." + extension;
        return string.Equals(file.Extension, normalized, comparison);
    }
}