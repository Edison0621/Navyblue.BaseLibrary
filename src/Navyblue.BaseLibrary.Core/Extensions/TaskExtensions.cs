// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : TaskExtensions.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-29  13:02
// ****************************************************************************************************************************************
// <copyright file="TaskExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
///     The task extensions.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    ///     Wait and return a task of type <typeparamref name="T" /> asynchronously.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="task">The task.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="timeoutMessage">The timeout message.</param>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><![CDATA[Task<T>]]></returns>
    public static async Task<T> WaitAsync<T>(this Task<T> task, TimeSpan timeout, string? timeoutMessage = null)
    {
        ArgumentNullException.ThrowIfNull(task);
        using CancellationTokenSource cts = new CancellationTokenSource(timeout);
        try
        {
            return await task.WaitAsync(cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!task.IsCompleted)
        {
            throw new TimeoutException(timeoutMessage ?? $"The operation timed out after {timeout}.");
        }
    }

    /// <summary>
    ///     Withs the timeout.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="timeoutMessage">The timeout message.</param>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A Task</returns>
    public static async Task WithTimeout(this Task task, TimeSpan timeout, string? timeoutMessage = null)
    {
        ArgumentNullException.ThrowIfNull(task);
        using CancellationTokenSource cts = new CancellationTokenSource(timeout);
        try
        {
            await task.WaitAsync(cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!task.IsCompleted)
        {
            throw new TimeoutException(timeoutMessage ?? $"The operation timed out after {timeout}.");
        }
    }

    /// <summary>
    ///     Ignore the exception asynchronously.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <param name="onException">The on exception.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A Task</returns>
    public static async Task IgnoreExceptionAsync(this Task task, Action<Exception>? onException = null)
    {
        ArgumentNullException.ThrowIfNull(task);
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            onException?.Invoke(ex);
        }
    }

    /// <summary>
    ///     Whens the all.
    /// </summary>
    /// <typeparam name="T" />
    /// <param name="tasks">The tasks.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns><![CDATA[Task<IReadOnlyList<T>>]]></returns>
    public static async Task<IReadOnlyList<T>> WhenAll<T>(this IEnumerable<Task<T>> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        return await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}