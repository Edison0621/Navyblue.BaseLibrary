// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : FakeDomainServices.cs
// Created          : 2026-07-09  16:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  16:06
// ****************************************************************************************************************************************
// <copyright file="FakeDomainServices.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.Testing;

/// <summary>
///     Fake <see cref="IAuditor" /> for tests.
/// </summary>
public sealed class FakeAuditor : IAuditor
{
    /// <inheritdoc />
    public string? CurrentUserId { get; set; } = "test-user";

    /// <inheritdoc />
    public string? CurrentUserName { get; set; } = "Test User";
}

/// <summary>
///     Fake <see cref="ITenantResolver" /> for tests.
/// </summary>
public sealed class FakeTenantResolver : ITenantResolver
{
    /// <inheritdoc />
    public string? CurrentTenantId { get; set; } = "test-tenant";
}

/// <summary>
///     Fake <see cref="IAuditPropertySetter" /> that records audited entities.
/// </summary>
public sealed class FakeAuditPropertySetter(IClock? clock = null, IAuditor? auditor = null) : IAuditPropertySetter
{
    private readonly IClock _clock = clock ?? new SystemClock();
    private readonly IAuditor _auditor = auditor ?? new FakeAuditor();

    /// <summary>
    ///     Gets entities that received creation audit.
    /// </summary>
    public List<object> Created { get; } = [];

    /// <summary>
    ///     Gets entities that received modification audit.
    /// </summary>
    public List<object> Modified { get; } = [];

    /// <summary>
    ///     Gets entities that received deletion audit.
    /// </summary>
    public List<object> Deleted { get; } = [];

    /// <inheritdoc />
    public void SetCreationAudit(object entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        this.Created.Add(entity);
        if (entity is ICreationAudited creation)
        {
            creation.CreatedAt = this._clock.UtcNow;
            creation.CreatedBy = this._auditor.CurrentUserId;
        }
    }

    /// <inheritdoc />
    public void SetModificationAudit(object entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        this.Modified.Add(entity);
        if (entity is IModificationAudited modification)
        {
            modification.ModifiedAt = this._clock.UtcNow;
            modification.ModifiedBy = this._auditor.CurrentUserId;
        }
    }

    /// <inheritdoc />
    public void SetDeletionAudit(object entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        this.Deleted.Add(entity);
        if (entity is ISoftDelete softDelete)
        {
            softDelete.IsDeleted = true;
            softDelete.DeletedAt = this._clock.UtcNow;
            softDelete.DeletedBy = this._auditor.CurrentUserId;
        }
    }

    /// <summary>
    ///     Clears recorded entities.
    /// </summary>
    public void Clear()
    {
        this.Created.Clear();
        this.Modified.Clear();
        this.Deleted.Clear();
    }
}

/// <summary>
///     Sequential <see cref="IIdGenerator{T}" /> for deterministic tests.
/// </summary>
public sealed class SequentialIdGenerator(long start = 1) : IIdGenerator<long>
{
    private long _next = start;

    /// <inheritdoc />
    public long NextId() => Interlocked.Increment(ref this._next) - 1;

    /// <summary>
    ///     Resets the sequence to <paramref name="startValue" />.
    /// </summary>
    public void Reset(long startValue = 1) => Interlocked.Exchange(ref this._next, startValue);
}

/// <summary>
///     Sequential GUID generator that embeds an incrementing counter for readability in tests.
/// </summary>
public sealed class SequentialGuidIdGenerator : IIdGenerator<Guid>
{
    private long _next;

    /// <inheritdoc />
    public Guid NextId()
    {
        long value = Interlocked.Increment(ref this._next);
        Span<byte> bytes = stackalloc byte[16];
        BitConverter.TryWriteBytes(bytes, value);
        return new Guid(bytes);
    }
}
