// *****************************************************************************************************************
// Project          : NavyBlue
// File             : Task.cs
// Created          : 2019-01-09  20:11
//
// Last Modified By : (jstsmaxx@163.com)
// Last Modified On : 2019-01-15  10:53
// *****************************************************************************************************************
// <copyright file="Task.cs" company="Shanghai Future Mdt InfoTech Ltd.">
//     Copyright ©  2012-2019 Mdt InfoTech Ltd. All rights reserved.
// </copyright>
// *****************************************************************************************************************

using System;
using System.Threading.Tasks;

namespace Navyblue.BaseLibrary
{
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
}