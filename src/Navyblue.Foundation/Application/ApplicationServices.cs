// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ApplicationServices.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="ApplicationServices.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Application;

/// <summary>
/// </summary>
public interface IApplicationService;

/// <summary>
/// </summary>
/// <typeparam name="TDto">The type of the dto.</typeparam>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TQuery">The type of the query.</typeparam>
/// <seealso cref="Navyblue.Foundation.Application.IApplicationService" />
public interface IReadOnlyAppService<TDto, in TKey, in TQuery> : IApplicationService
{
    /// <summary>
    ///     Gets the asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<ApiResult<TDto>> GetAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the list asynchronous.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<ApiResult<PageResult<TDto>>> GetListAsync(TQuery query, CancellationToken cancellationToken = default);
}

/// <summary>
/// </summary>
/// <typeparam name="TDto">The type of the dto.</typeparam>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TQuery">The type of the query.</typeparam>
/// <typeparam name="TCreateInput">The type of the create input.</typeparam>
/// <typeparam name="TUpdateInput">The type of the update input.</typeparam>
/// <seealso cref="Navyblue.Foundation.Application.IReadOnlyAppService&lt;TDto, TKey, TQuery&gt;" />
public interface ICrudAppService<TDto, in TKey, in TQuery, in TCreateInput, in TUpdateInput> : IReadOnlyAppService<TDto, TKey, TQuery>
{
    /// <summary>
    ///     Creates the asynchronous.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<ApiResult<TDto>> CreateAsync(TCreateInput input, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates the asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="input">The input.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<ApiResult<TDto>> UpdateAsync(TKey id, TUpdateInput input, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes the asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<ApiResult> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}

/// <summary>
/// </summary>
public interface IObjectMapper
{
    /// <summary>
    ///     Maps the specified source.
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    TDestination Map<TDestination>(object source);

    /// <summary>
    ///     Maps the specified source.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="destination">The destination.</param>
    /// <returns></returns>
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
}

/// <summary>
/// </summary>
public interface IPermissionChecker
{
    /// <summary>
    ///     Determines whether [is granted asynchronous] [the specified permission name].
    /// </summary>
    /// <param name="permissionName">Name of the permission.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    ValueTask<bool> IsGrantedAsync(string permissionName, CancellationToken cancellationToken = default);
}

/// <summary>
/// </summary>
public interface IDataPermissionContext
{
    /// <summary>
    ///     Gets the user identifier.
    /// </summary>
    /// <value>
    ///     The user identifier.
    /// </value>
    string? UserId { get; }

    /// <summary>
    ///     Gets the tenant identifier.
    /// </summary>
    /// <value>
    ///     The tenant identifier.
    /// </value>
    string? TenantId { get; }

    /// <summary>
    ///     Gets the roles.
    /// </summary>
    /// <value>
    ///     The roles.
    /// </value>
    IReadOnlyCollection<string> Roles { get; }
}