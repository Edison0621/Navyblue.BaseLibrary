// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernTaskExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:04
// ****************************************************************************************************************************************
// <copyright file="ModernTaskExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
/// </summary>
public static class ModernTaskExtensions
{
    /// <summary>
    ///     Withes the timeout.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task">The task.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="timeoutMessage">The timeout message.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">task</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">timeout</exception>
    /// <exception cref="System.TimeoutException"></exception>
    public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout, string? timeoutMessage = null)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
        if (timeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(timeout));
        Task delayTask = Task.Delay(timeout);
        Task completedTask = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
        if (completedTask == delayTask) throw new TimeoutException(timeoutMessage ?? "The operation timed out after " + timeout + ".");
        return await task.ConfigureAwait(false);
    }

    /// <summary>
    ///     Withes the timeout.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="timeoutMessage">The timeout message.</param>
    /// <exception cref="System.ArgumentNullException">task</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">timeout</exception>
    /// <exception cref="System.TimeoutException"></exception>
    public static async Task WithTimeout(this Task task, TimeSpan timeout, string? timeoutMessage = null)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
        if (timeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(timeout));
        Task delayTask = Task.Delay(timeout);
        Task completedTask = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
        if (completedTask == delayTask) throw new TimeoutException(timeoutMessage ?? "The operation timed out after " + timeout + ".");
        await task.ConfigureAwait(false);
    }

    /// <summary>
    ///     Ignores the exception asynchronous.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <param name="onException">The on exception.</param>
    /// <exception cref="System.ArgumentNullException">task</exception>
    public static async Task IgnoreExceptionAsync(this Task task, Action<Exception>? onException = null)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
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
    ///     Whens all.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tasks">The tasks.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">tasks</exception>
    public static async Task<IReadOnlyList<T>> WhenAll<T>(this IEnumerable<Task<T>> tasks)
    {
        if (tasks == null) throw new ArgumentNullException(nameof(tasks));
        return await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}