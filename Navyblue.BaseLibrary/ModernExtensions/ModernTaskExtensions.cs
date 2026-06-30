// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernTaskExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:49
// ****************************************************************************************************************************************
// <copyright file="ModernTaskExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
namespace Navyblue.BaseLibrary.Extensions;

public static class ModernTaskExtensions
{
    public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout, string? timeoutMessage = null)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
        if (timeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(timeout));
        Task delayTask = Task.Delay(timeout);
        Task completedTask = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
        if (completedTask == delayTask) throw new TimeoutException(timeoutMessage ?? "The operation timed out after " + timeout + ".");
        return await task.ConfigureAwait(false);
    }

    public static async Task WithTimeout(this Task task, TimeSpan timeout, string? timeoutMessage = null)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));
        if (timeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(timeout));
        Task delayTask = Task.Delay(timeout);
        Task completedTask = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
        if (completedTask == delayTask) throw new TimeoutException(timeoutMessage ?? "The operation timed out after " + timeout + ".");
        await task.ConfigureAwait(false);
    }

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

    public static async Task<IReadOnlyList<T>> WhenAll<T>(this IEnumerable<Task<T>> tasks)
    {
        if (tasks == null) throw new ArgumentNullException(nameof(tasks));
        return await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}