// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Knowledge.cs
// Created          : 2026-07-30  15:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="Knowledge.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     Knowledge retrieval request. Carries the caller identity so retrievers can enforce tenant and permission isolation.
/// </summary>
public sealed record KnowledgeQuery(
    string Query,
    int TopK = 5,
    string? UserId = null,
    string? TenantId = null,
    IReadOnlyDictionary<string, string>? Filters = null);

/// <summary>
///     Knowledge retrieval hit for unstructured RAG only.
/// </summary>
/// <param name="TenantId">Owning tenant; null means the document is global.</param>
/// <param name="RequiredPermission">Permission required to read the document; null means no additional permission.</param>
/// <param name="Source">Origin of the document, surfaced to users as a citation.</param>
public sealed record KnowledgeHit(
    string Id,
    string Title,
    string Content,
    double Score,
    string? TenantId = null,
    string? RequiredPermission = null,
    string? Source = null);

/// <summary>
///     Retrieves unstructured knowledge only. Must not be used for live business data queries.
///     Implementations are responsible for tenant scoping; wrap with
///     <see cref="PermissionFilteringKnowledgeRetriever" /> to additionally enforce per-document permissions.
/// </summary>
public interface IKnowledgeRetriever
{
    ValueTask<Result<IReadOnlyList<KnowledgeHit>>> SearchAsync(KnowledgeQuery query, CancellationToken cancellationToken = default);
}

/// <summary>
///     Decorator that drops hits belonging to another tenant or requiring a permission the caller lacks.
///     Prevents RAG from becoming a cross-tenant data leak.
/// </summary>
public sealed class PermissionFilteringKnowledgeRetriever(IKnowledgeRetriever inner, IPermissionGuard permissionGuard) : IKnowledgeRetriever
{
    #region IKnowledgeRetriever Members

    /// <inheritdoc />
    public async ValueTask<Result<IReadOnlyList<KnowledgeHit>>> SearchAsync(KnowledgeQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        Result<IReadOnlyList<KnowledgeHit>> retrieved = await inner.SearchAsync(query, cancellationToken).ConfigureAwait(false);
        if (retrieved.Failed)
        {
            return retrieved;
        }

        List<KnowledgeHit> allowed = [];
        foreach (KnowledgeHit hit in retrieved.Value!)
        {
            if (!TenantMatches(hit.TenantId, query.TenantId))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(hit.RequiredPermission))
            {
                Result granted = await permissionGuard.EnsureGrantedAsync(hit.RequiredPermission, cancellationToken).ConfigureAwait(false);
                if (granted.Failed)
                {
                    continue;
                }
            }

            allowed.Add(hit);
        }

        return Result.Success<IReadOnlyList<KnowledgeHit>>(allowed);
    }

    #endregion

    private static bool TenantMatches(string? documentTenant, string? callerTenant)
        => string.IsNullOrWhiteSpace(documentTenant) || string.Equals(documentTenant, callerTenant, StringComparison.Ordinal);
}