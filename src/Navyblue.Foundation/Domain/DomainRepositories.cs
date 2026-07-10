// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : DomainRepositories.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="DomainRepositories.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Domain;

/// <summary>
///     The aggregate repository interface.
/// </summary>
/// <typeparam name="TAggregate" />
/// <typeparam name="TKey" />
// ReSharper disable once TypeParameterCanBeVariant
public interface IAggregateRepository<TAggregate, TKey>
    where TAggregate : class, IAggregateRoot<TKey>
{
    /// <summary>
    ///     Finds and return a valuetask of type taggregate asynchronously.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><![CDATA[ValueTask<TAggregate?>]]></returns>
    ValueTask<TAggregate?> FindAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// </summary>
    /// <param name="aggregate">The aggregate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task</returns>
    Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// </summary>
    /// <param name="aggregate">The aggregate.</param>
    void Update(TAggregate aggregate);

    /// <summary>
    /// </summary>
    /// <param name="aggregate">The aggregate.</param>
    void Remove(TAggregate aggregate);
}

/// <summary>
///     The domain event unit of work interface.
/// </summary>
public interface IDomainEventUnitOfWork
{
    /// <summary>
    ///     Dispatches domain events asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ValueTask</returns>
    ValueTask DispatchDomainEventsAsync(CancellationToken cancellationToken = default);
}