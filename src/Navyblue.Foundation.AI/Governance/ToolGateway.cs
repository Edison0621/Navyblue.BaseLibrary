// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ToolGateway.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="ToolGateway.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.Diagnostics;
using Navyblue.Foundation.Domain;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.AI;

/// <summary>
///     Production tool gateway: tenant scope → argument validation → permission → business rules → risk gate →
///     resilient execution → audit. Write/HighRisk require <c>confirmed: true</c> obtained through draft confirmation.
///     Holds no tool state of its own, so it can safely be registered as scoped alongside per-request services.
/// </summary>
public sealed class ToolGateway(
    IToolRegistry registry,
    IPermissionGuard permissionGuard,
    IBusinessRuleValidator ruleValidator,
    IToolArgumentValidator argumentValidator,
    IAiAuditLogger auditLogger,
    ToolResilienceCoordinator resilience,
    NavyblueAiOptions options,
    IClock? clock = null) : IToolGateway
{
    private readonly IClock _clock = clock ?? new SystemClock();

    #region IToolGateway Members

    /// <inheritdoc />
    public IReadOnlyList<ToolDefinition> ListTools(string? tenantId = null) => registry.ListTools(tenantId);

    /// <inheritdoc />
    public bool TryGetTool(string name, out ToolDefinition? tool) => registry.TryGetTool(name, out tool);

    /// <inheritdoc />
    public async ValueTask<Result<object?>> InvokeAsync(ToolInvocation invocation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(invocation);
        if (!registry.TryGetTool(invocation.ToolName, out ToolDefinition? tool) || tool is null)
        {
            await this.AuditAsync(AiAuditActions.ToolMissing, invocation, false, "Tool not registered.", cancellationToken).ConfigureAwait(false);
            return Result.Failure<object?>(new Error(AiErrorCodes.ToolNotFound, $"Tool '{invocation.ToolName}' is not registered."));
        }

        ResolvedOperation synthetic = new(tool.Name, AiWellKnownDomains.Unknown, tool.RiskLevel, invocation.Arguments, tool.PermissionName, tool.Name);
        return await this.ExecuteAsync(tool, synthetic, invocation, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<Result<object?>> InvokeResolvedAsync(ResolvedOperation operation, string? userId = null, string? tenantId = null, string? correlationId = null, bool confirmed = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        string toolName = operation.ToolName ?? operation.OperationName;
        ToolInvocation invocation = new(toolName, operation.Parameters, userId, tenantId, correlationId ?? CorrelationContext.Current, confirmed);

        if (!registry.TryGetTool(toolName, out ToolDefinition? tool) || tool is null)
        {
            await this.AuditAsync(AiAuditActions.ToolMissing, invocation, false, "Tool not registered.", cancellationToken).ConfigureAwait(false);
            return Result.Failure<object?>(new Error(AiErrorCodes.ToolNotFound, $"Tool '{toolName}' is not registered."));
        }

        // Prefer risk and permission from the registered definition; the resolver may not know them.
        ResolvedOperation effective = operation with
        {
            RiskLevel = tool.RiskLevel,
            PermissionName = operation.PermissionName ?? tool.PermissionName,
            ToolName = tool.Name
        };

        return await this.ExecuteAsync(tool, effective, invocation, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public ValueTask<Result<object?>> ExecuteConfirmedAsync(ResolvedOperation operation, string? userId = null, string? tenantId = null, string? correlationId = null, CancellationToken cancellationToken = default)
        => this.InvokeResolvedAsync(operation, userId, tenantId, correlationId, confirmed: true, cancellationToken);

    #endregion

    private async ValueTask<Result<object?>> ExecuteAsync(ToolDefinition tool, ResolvedOperation operation, ToolInvocation invocation, CancellationToken cancellationToken)
    {
        using Activity? activity = AiTelemetry.StartActivity("ai.tool.invoke");
        activity?.SetTag("ai.tool.name", tool.Name);
        activity?.SetTag("ai.tool.risk", tool.RiskLevel.ToString());
        activity?.SetTag("ai.tenant.id", invocation.TenantId);

        long startedAt = Stopwatch.GetTimestamp();

        Result<object?> outcome = await this.ExecuteCoreAsync(tool, operation, invocation, activity, cancellationToken).ConfigureAwait(false);

        double elapsedMs = (Stopwatch.GetTimestamp() - startedAt) * 1000d / Stopwatch.Frequency;
        AiTelemetry.ToolDuration.Record(elapsedMs, new KeyValuePair<string, object?>("tool", tool.Name));
        AiTelemetry.ToolInvocations.Add(
            1,
            new KeyValuePair<string, object?>("tool", tool.Name),
            new KeyValuePair<string, object?>("outcome", outcome.Succeeded ? "success" : outcome.Error.Code));

        return outcome;
    }

    private async ValueTask<Result<object?>> ExecuteCoreAsync(ToolDefinition tool, ResolvedOperation operation, ToolInvocation invocation, Activity? activity, CancellationToken cancellationToken)
    {
        if (!tool.IsVisibleTo(invocation.TenantId))
        {
            return await this.DenyAsync(AiAuditActions.ToolMissing, invocation, AiErrorCodes.ToolTenantDenied, $"Tool '{tool.Name}' is not available for this tenant.", activity, cancellationToken).ConfigureAwait(false);
        }

        Result<IReadOnlyDictionary<string, object?>> validated = await argumentValidator.ValidateAsync(tool, invocation.Arguments, cancellationToken).ConfigureAwait(false);
        if (validated.Failed)
        {
            return await this.DenyAsync(AiAuditActions.ToolArgumentInvalid, invocation, validated.Error.Code, validated.Error.Message, activity, cancellationToken).ConfigureAwait(false);
        }

        IReadOnlyDictionary<string, object?> arguments = validated.Value!;
        ToolInvocation effectiveInvocation = invocation with { Arguments = arguments };
        ResolvedOperation effectiveOperation = operation with { Parameters = arguments };

        Result permission = await permissionGuard.EnsureGrantedAsync(effectiveOperation.PermissionName ?? tool.PermissionName, cancellationToken).ConfigureAwait(false);
        if (permission.Failed)
        {
            return await this.DenyAsync(AiAuditActions.ToolPermissionDenied, effectiveInvocation, permission.Error.Code, permission.Error.Message, activity, cancellationToken).ConfigureAwait(false);
        }

        Result rules = await ruleValidator.ValidateAsync(effectiveOperation, cancellationToken).ConfigureAwait(false);
        if (rules.Failed)
        {
            return await this.DenyAsync(AiAuditActions.ToolRuleFailed, effectiveInvocation, rules.Error.Code, rules.Error.Message, activity, cancellationToken).ConfigureAwait(false);
        }

        bool mutating = tool.RiskLevel is ToolRiskLevel.Write or ToolRiskLevel.HighRisk;
        if (mutating && !effectiveInvocation.Confirmed)
        {
            return await this.DenyAsync(AiAuditActions.ToolWriteBlocked, effectiveInvocation, AiErrorCodes.ToolRequiresConfirmation, "Write or high-risk operations require draft confirmation.", activity, cancellationToken).ConfigureAwait(false);
        }

        if (tool.ExecuteAsync is null)
        {
            return await this.DenyAsync(AiAuditActions.ToolNoExecutor, effectiveInvocation, AiErrorCodes.ToolNoExecutor, $"Tool '{tool.Name}' has no executor.", activity, cancellationToken).ConfigureAwait(false);
        }

        // Mutating calls are journalled before execution so a crash mid-flight still leaves a trail.
        if (mutating)
        {
            Result preAudit = await this.AuditAsync(AiAuditActions.ToolExecuting, effectiveInvocation, true, null, cancellationToken).ConfigureAwait(false);
            if (preAudit.Failed)
            {
                AiTelemetry.MarkFailed(activity, preAudit.Error.Code, preAudit.Error.Message);
                return Result.Failure<object?>(preAudit.Error);
            }
        }

        ToolExecutionPolicy policy = tool.Policy ?? options.DefaultToolExecutionPolicy;
        Result<object?> executed = await ToolExecutionPipeline.RunAsync(tool, arguments, policy, resilience, cancellationToken).ConfigureAwait(false);

        if (executed.Failed)
        {
            await this.AuditAsync(AiAuditActions.ToolFailed, effectiveInvocation, false, executed.Error.Message, cancellationToken).ConfigureAwait(false);
            AiTelemetry.MarkFailed(activity, executed.Error.Code, executed.Error.Message);
            return executed;
        }

        Result postAudit = await this.AuditAsync(AiAuditActions.ToolExecuted, effectiveInvocation, true, null, cancellationToken).ConfigureAwait(false);
        if (postAudit.Failed && options.AuditFailurePolicy == AuditFailurePolicy.FailClosed)
        {
            // The side effect already happened; surface the audit gap instead of silently losing it.
            AiTelemetry.MarkFailed(activity, postAudit.Error.Code, postAudit.Error.Message);
            return Result.Failure<object?>(postAudit.Error);
        }

        return executed;
    }

    private async ValueTask<Result<object?>> DenyAsync(string action, ToolInvocation invocation, string errorCode, string message, Activity? activity, CancellationToken cancellationToken)
    {
        await this.AuditAsync(action, invocation, false, message, cancellationToken).ConfigureAwait(false);
        AiTelemetry.MarkFailed(activity, errorCode, message);
        return Result.Failure<object?>(new Error(errorCode, message));
    }

    private async ValueTask<Result> AuditAsync(string action, ToolInvocation invocation, bool succeeded, string? detail, CancellationToken cancellationToken)
    {
        AiAuditEntry entry = new(
            action,
            invocation.ToolName,
            invocation.UserId,
            invocation.TenantId,
            invocation.CorrelationId ?? CorrelationContext.Current,
            succeeded,
            detail,
            this._clock.UtcNow);

        try
        {
            Result written = await auditLogger.WriteAsync(entry, cancellationToken).ConfigureAwait(false);
            if (written.Succeeded || options.AuditFailurePolicy == AuditFailurePolicy.Continue)
            {
                return Result.Success();
            }

            return written;
        }
        catch (Exception ex)
        {
            if (options.AuditFailurePolicy == AuditFailurePolicy.Continue)
            {
                return Result.Success();
            }

            return Result.Failure(new Error(AiErrorCodes.AuditWriteFailed, $"Audit write failed: {ex.Message}"));
        }
    }
}

/// <summary>
///     Default MCP bridge forwarding to <see cref="IToolGateway" />.
/// </summary>
public sealed class McpToolBridge(IToolGateway gateway) : IMcpToolBridge
{
    #region IMcpToolBridge Members

    /// <inheritdoc />
    public IReadOnlyList<ToolDefinition> ListTools(string? tenantId = null) => gateway.ListTools(tenantId);

    /// <inheritdoc />
    public ValueTask<Result<object?>> InvokeToolAsync(string toolName, IReadOnlyDictionary<string, object?> arguments, string? userId = null, string? tenantId = null, string? correlationId = null, CancellationToken cancellationToken = default)
        => gateway.InvokeAsync(new ToolInvocation(toolName, arguments, userId, tenantId, correlationId ?? CorrelationContext.Current), cancellationToken);

    #endregion
}

/// <summary>
///     Fail-closed permission guard for production: any non-empty permission name is denied until a real guard is registered.
/// </summary>
public sealed class FailClosedPermissionGuard : IPermissionGuard
{
    #region IPermissionGuard Members

    /// <inheritdoc />
    public ValueTask<Result> EnsureGrantedAsync(string? permissionName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(permissionName))
        {
            return ValueTask.FromResult(Result.Success());
        }

        return ValueTask.FromResult(Result.Failure(new Error(
            AiErrorCodes.PermissionGuardNotConfigured,
            $"Permission '{permissionName}' cannot be evaluated. Register IPermissionGuard (e.g. PermissionCheckerGuard).")));
    }

    #endregion
}

/// <summary>
///     Permission guard that always grants. Only for development / explicit opt-in.
/// </summary>
public sealed class AllowAllPermissionGuard : IPermissionGuard
{
    #region IPermissionGuard Members

    /// <inheritdoc />
    public ValueTask<Result> EnsureGrantedAsync(string? permissionName, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Result.Success());

    #endregion
}

/// <summary>
///     Permission guard adapting Foundation <c>IPermissionChecker</c>. Register as scoped when the checker
///     depends on the current user.
/// </summary>
public sealed class PermissionCheckerGuard(IPermissionChecker checker) : IPermissionGuard
{
    #region IPermissionGuard Members

    /// <inheritdoc />
    public async ValueTask<Result> EnsureGrantedAsync(string? permissionName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(permissionName))
        {
            return Result.Success();
        }

        bool granted = await checker.IsGrantedAsync(permissionName, cancellationToken).ConfigureAwait(false);
        return granted
            ? Result.Success()
            : Result.Failure(new Error(AiErrorCodes.PermissionDenied, $"Permission '{permissionName}' is not granted."));
    }

    #endregion
}

/// <summary>
///     No-op business rule validator.
/// </summary>
public sealed class NullBusinessRuleValidator : IBusinessRuleValidator
{
    #region IBusinessRuleValidator Members

    /// <inheritdoc />
    public ValueTask<Result> ValidateAsync(ResolvedOperation operation, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Result.Success());

    #endregion
}

/// <summary>
///     Default confirmation service over <see cref="IDraftStore" />, enforcing maker-checker when configured.
/// </summary>
public sealed class ConfirmationService(IDraftStore draftStore) : IConfirmationService
{
    #region IConfirmationService Members

    /// <inheritdoc />
    public ValueTask<Result<OperationDraft>> ConfirmAsync(DraftConfirmationRequest request, CancellationToken cancellationToken = default)
        => draftStore.ConfirmAsync(request, cancellationToken);

    #endregion
}