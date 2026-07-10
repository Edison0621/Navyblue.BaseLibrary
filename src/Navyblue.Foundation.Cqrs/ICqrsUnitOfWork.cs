// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ICqrsUnitOfWork.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="ICqrsUnitOfWork.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
/// </summary>
public interface ICqrsUnitOfWork
{
    /// <summary>
    ///     Begins the asynchronous.
    /// </summary>
    /// <returns></returns>
    Task BeginAsync();

    /// <summary>
    ///     Commits the asynchronous.
    /// </summary>
    /// <returns></returns>
    Task CommitAsync();

    /// <summary>
    ///     Rollbacks the asynchronous.
    /// </summary>
    /// <returns></returns>
    Task RollbackAsync();
}