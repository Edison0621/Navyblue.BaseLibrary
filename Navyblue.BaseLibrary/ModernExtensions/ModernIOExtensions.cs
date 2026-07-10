// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernIOExtensions.cs
// Created          : 2026-06-30  15:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:04
// ****************************************************************************************************************************************
// <copyright file="ModernIOExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
using System.Text;

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
/// </summary>
public static class ModernStreamExtensions
{
    /// <summary>
    ///     Reads all bytes asynchronous.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">stream</exception>
    public static async Task<byte[]> ReadAllBytesAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        switch (stream)
        {
            case null:
                throw new ArgumentNullException(nameof(stream));
            case MemoryStream memoryStream when memoryStream.TryGetBuffer(out ArraySegment<byte> buffer):
                return buffer.AsSpan().ToArray();
        }

        using MemoryStream destination = new MemoryStream();
        await stream.CopyToAsync(destination, 81920, cancellationToken).ConfigureAwait(false);
        return destination.ToArray();
    }

    /// <summary>
    ///     Reads all text asynchronous.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">stream</exception>
    public static async Task<string> ReadAllTextAsync(this Stream stream, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        using StreamReader reader = new StreamReader(stream, encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        cancellationToken.ThrowIfCancellationRequested();
        // ReSharper disable once MethodSupportsCancellation
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    /// <summary>
    ///     Copies to memory stream asynchronous.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="resetSourcePosition">if set to <c>true</c> [reset source position].</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">stream</exception>
    public static async Task<MemoryStream> CopyToMemoryStreamAsync(this Stream stream, bool resetSourcePosition = false, CancellationToken cancellationToken = default)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (resetSourcePosition && stream.CanSeek) stream.Position = 0;
        MemoryStream destination = new MemoryStream();
        await stream.CopyToAsync(destination, 81920, cancellationToken).ConfigureAwait(false);
        destination.Position = 0;
        return destination;
    }

    /// <summary>
    ///     Resets the position if can seek.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">stream</exception>
    public static Stream ResetPositionIfCanSeek(this Stream stream)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (stream.CanSeek) stream.Position = 0;
        return stream;
    }
}

/// <summary>
/// </summary>
public static class ModernFileInfoExtensions
{
    /// <summary>
    ///     Reads all text asynchronous.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">file</exception>
    public static Task<string> ReadAllTextAsync(this FileInfo file, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        return File.ReadAllTextAsync(file.FullName, encoding ?? Encoding.UTF8, cancellationToken);
    }

    /// <summary>
    ///     Reads all bytes asynchronous.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">file</exception>
    public static Task<byte[]> ReadAllBytesAsync(this FileInfo file, CancellationToken cancellationToken = default)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        return File.ReadAllBytesAsync(file.FullName, cancellationToken);
    }

    /// <summary>
    ///     Writes all text asynchronous.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="content">The content.</param>
    /// <param name="encoding">The encoding.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="System.ArgumentNullException">file</exception>
    public static async Task WriteAllTextAsync(this FileInfo file, string content, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        Directory.CreateDirectory(file.DirectoryName ?? ".");
        await File.WriteAllTextAsync(file.FullName, content, encoding ?? Encoding.UTF8, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Writes all bytes asynchronous.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="content">The content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="System.ArgumentNullException">
    ///     file
    ///     or
    ///     content
    /// </exception>
    public static async Task WriteAllBytesAsync(this FileInfo file, byte[] content, CancellationToken cancellationToken = default)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (content == null) throw new ArgumentNullException(nameof(content));
        Directory.CreateDirectory(file.DirectoryName ?? ".");
        await File.WriteAllBytesAsync(file.FullName, content, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Determines whether the specified extension has extension.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="extension">The extension.</param>
    /// <param name="comparison">The comparison.</param>
    /// <returns>
    ///     <c>true</c> if the specified extension has extension; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     file
    ///     or
    ///     extension
    /// </exception>
    public static bool HasExtension(this FileInfo file, string extension, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (extension == null) throw new ArgumentNullException(nameof(extension));
        string normalized = extension.StartsWith(".", StringComparison.Ordinal) ? extension : "." + extension;
        return string.Equals(file.Extension, normalized, comparison);
    }
}