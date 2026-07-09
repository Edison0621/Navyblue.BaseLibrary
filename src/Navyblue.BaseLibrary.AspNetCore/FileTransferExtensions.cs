// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : FileTransferExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:50
// ****************************************************************************************************************************************
// <copyright file="FileTransferExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Navyblue.BaseLibrary.Files;

namespace Navyblue.BaseLibrary.AspNetCore;

/// <summary>
/// </summary>
public sealed record UploadedFileInfo(string OriginalFileName, string SafeFileName, string ContentType, long Length, string Extension);

/// <summary>
/// </summary>
public static class FileTransferExtensions
{
    /// <summary>
    ///     Converts to uploadedfileinfo.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static UploadedFileInfo ToUploadedFileInfo(this IFormFile file)
    {
        ArgumentNullException.ThrowIfNull(file);
        string safeName = FileNameSanitizer.Sanitize(Path.GetFileName(file.FileName));
        return new UploadedFileInfo(file.FileName, safeName, file.ContentType, file.Length, Path.GetExtension(safeName));
    }

    /// <summary>
    ///     Saves to asynchronous.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="directory">The directory.</param>
    /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static async Task<UploadedFileInfo> SaveToAsync(this IFormFile file, string directory, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        Directory.CreateDirectory(directory);

        UploadedFileInfo info = file.ToUploadedFileInfo();
        string path = Path.Combine(directory, info.SafeFileName);
        if (!overwrite && File.Exists(path))
        {
            string name = Path.GetFileNameWithoutExtension(info.SafeFileName);
            string extension = Path.GetExtension(info.SafeFileName);
            path = Path.Combine(directory, $"{name}_{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}{extension}");
            info = info with { SafeFileName = Path.GetFileName(path), Extension = extension };
        }

        await using FileStream stream = File.Create(path);
        await file.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
        return info;
    }

    /// <summary>
    ///     Converts to filecontentresult.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="contentType">Type of the content.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException"></exception>
    public static FileContentResult ToFileContentResult(this byte[] bytes, string fileName, string? contentType = null)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        return new FileContentResult(bytes, contentType ?? MimeTypeMap.GetMimeType(fileName)) { FileDownloadName = fileName };
    }
}