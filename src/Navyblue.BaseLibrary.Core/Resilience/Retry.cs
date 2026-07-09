// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Retry.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="Retry.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary.Resilience;

/// <summary>
///     The retry.
/// </summary>
public static class Retry
{
    /// <summary>
    ///     Execute and return a task of type <typeparamref name="T" /> asynchronously.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="operation">The operation.</param>
    /// <param name="attempts">The attempts.</param>
    /// <param name="delay">The delay.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[Task<T>]]></returns>
    public static async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, int attempts = 3, TimeSpan? delay = null, CancellationToken cancellationToken = default)
    {
        Exception? last = null;
        for (int i = 1; i <= attempts; i++)
        {
            try
            {
                return await operation(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (i < attempts)
            {
                last = ex;
                await Task.Delay(delay ?? TimeSpan.FromMilliseconds(100 * i), cancellationToken).ConfigureAwait(false);
            }
        }

        throw last ?? new InvalidOperationException("Retry failed.");
    }
}