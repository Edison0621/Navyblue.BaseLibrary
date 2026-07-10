// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Task.cs
// Created          : 2026-06-26  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:05
// ****************************************************************************************************************************************
// <copyright file="Task.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary;

/// <summary>
///     TaskEx.
/// </summary>
public static class TaskEx
{
    /// <summary>
    ///     Forgets the specified task.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <param name="exceptionHandler">The exception handler.</param>
    public static async Task Forget(this Task task, Action<Exception> exceptionHandler = null)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception e)
        {
            exceptionHandler?.Invoke(e);
        }
    }
}