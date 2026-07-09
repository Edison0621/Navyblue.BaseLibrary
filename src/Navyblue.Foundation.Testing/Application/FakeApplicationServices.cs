// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : FakeApplicationServices.cs
// Created          : 2026-07-09  16:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  16:06
// ****************************************************************************************************************************************
// <copyright file="FakeApplicationServices.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Collections.Concurrent;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.Testing;

/// <summary>
///     Fake <see cref="IPermissionChecker" /> with an allow-list of permission names.
/// </summary>
public sealed class FakePermissionChecker : IPermissionChecker
{
    private readonly ConcurrentDictionary<string, byte> _granted = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Gets or sets a value indicating whether all permissions are granted when the allow-list is empty.
    /// </summary>
    public bool GrantAllWhenEmpty { get; set; } = true;

    /// <summary>
    ///     Grants the specified permission.
    /// </summary>
    public FakePermissionChecker Grant(string permissionName)
    {
        Guard.NotNullOrWhiteSpace(permissionName, nameof(permissionName));
        this._granted[permissionName] = 0;
        return this;
    }

    /// <summary>
    ///     Revokes the specified permission.
    /// </summary>
    public FakePermissionChecker Revoke(string permissionName)
    {
        Guard.NotNullOrWhiteSpace(permissionName, nameof(permissionName));
        this._granted.TryRemove(permissionName, out _);
        return this;
    }

    /// <inheritdoc />
    public ValueTask<bool> IsGrantedAsync(string permissionName, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(permissionName, nameof(permissionName));
        if (this._granted.IsEmpty)
        {
            return ValueTask.FromResult(this.GrantAllWhenEmpty);
        }

        return ValueTask.FromResult(this._granted.ContainsKey(permissionName));
    }

    /// <summary>
    ///     Clears granted permissions.
    /// </summary>
    public void Clear() => this._granted.Clear();
}

/// <summary>
///     Fake <see cref="IDataPermissionContext" /> for tests.
/// </summary>
public sealed class FakeDataPermissionContext : IDataPermissionContext
{
    /// <inheritdoc />
    public string? UserId { get; set; } = "test-user";

    /// <inheritdoc />
    public string? TenantId { get; set; } = "test-tenant";

    /// <summary>
    ///     Gets the mutable role list.
    /// </summary>
    public List<string> RoleList { get; } = [];

    /// <inheritdoc />
    public IReadOnlyCollection<string> Roles => this.RoleList;
}

/// <summary>
///     Fake <see cref="IObjectMapper" /> that uses a registered map function or returns the destination as-is.
/// </summary>
public sealed class FakeObjectMapper : IObjectMapper
{
    private readonly Dictionary<(Type Source, Type Destination), Func<object, object?, object>> _maps = [];

    /// <summary>
    ///     Registers a mapping function from <typeparamref name="TSource" /> to <typeparamref name="TDestination" />.
    /// </summary>
    public FakeObjectMapper Register<TSource, TDestination>(Func<TSource, TDestination?, TDestination> map)
    {
        ArgumentNullException.ThrowIfNull(map);
        this._maps[(typeof(TSource), typeof(TDestination))] = (source, destination) =>
            map((TSource)source, destination is null ? default : (TDestination)destination)!;
        return this;
    }

    /// <inheritdoc />
    public TDestination Map<TDestination>(object source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (this._maps.TryGetValue((source.GetType(), typeof(TDestination)), out Func<object, object?, object>? map))
        {
            return (TDestination)map(source, null);
        }

        if (source is TDestination typed)
        {
            return typed;
        }

        throw new InvalidOperationException($"No map registered from {source.GetType().Name} to {typeof(TDestination).Name}.");
    }

    /// <inheritdoc />
    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (this._maps.TryGetValue((typeof(TSource), typeof(TDestination)), out Func<object, object?, object>? map))
        {
            return (TDestination)map(source, destination);
        }

        return destination;
    }
}
