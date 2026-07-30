// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ToolGatewayTests.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="ToolGatewayTests.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Primitives;
using Xunit;

namespace Navyblue.Foundation.AI.Tests;

public sealed class ToolGatewayTests
{
    private static ToolGateway CreateGateway(
        IToolRegistry registry,
        IAiAuditLogger? audit = null,
        NavyblueAiOptions? options = null,
        IPermissionGuard? guard = null)
    {
        NavyblueAiOptions effective = options ?? new NavyblueAiOptions();
        return new ToolGateway(
            registry,
            guard ?? new AllowAllPermissionGuard(),
            new NullBusinessRuleValidator(),
            new SpecToolArgumentValidator(effective),
            audit ?? new InMemoryAiAuditLogger(),
            new ToolResilienceCoordinator(),
            effective);
    }

    [Fact]
    public async Task Confirmed_flag_is_thread_safe_without_shared_mutable_state()
    {
        ToolRegistry registry = new();
        int runs = 0;
        registry.Register(new ToolDefinition(
            "write",
            "w",
            ToolRiskLevel.Write,
            ExecuteAsync: (_, _) =>
            {
                Interlocked.Increment(ref runs);
                return ValueTask.FromResult<object?>("ok");
            }));

        ToolGateway gateway = CreateGateway(registry);
        ResolvedOperation op = new("write", "demo", ToolRiskLevel.Write, new Dictionary<string, object?>(), ToolName: "write");

        Task<Result<object?>>[] blocked = Enumerable.Range(0, 20)
            .Select(_ => gateway.InvokeResolvedAsync(op, confirmed: false).AsTask())
            .ToArray();
        Task<Result<object?>>[] allowed = Enumerable.Range(0, 20)
            .Select(_ => gateway.ExecuteConfirmedAsync(op).AsTask())
            .ToArray();

        Result<object?>[] results = await Task.WhenAll(blocked.Concat(allowed));
        Assert.Equal(20, results.Count(r => r.Failed && r.Error.Code == AiErrorCodes.ToolRequiresConfirmation));
        Assert.Equal(20, results.Count(r => r.Succeeded));
        Assert.Equal(20, runs);
    }

    [Fact]
    public void ListTools_returns_registered_catalog()
    {
        ToolRegistry registry = new();
        registry.Register(new ToolDefinition("b", "b", ToolRiskLevel.Read, ExecuteAsync: (_, _) => ValueTask.FromResult<object?>(null)));
        registry.Register(new ToolDefinition("a", "a", ToolRiskLevel.Read, ExecuteAsync: (_, _) => ValueTask.FromResult<object?>(null)));

        ToolGateway gateway = CreateGateway(registry);
        Assert.Equal(["a", "b"], gateway.ListTools().Select(t => t.Name).ToArray());
    }

    [Fact]
    public async Task Declared_parameters_are_validated_before_execution()
    {
        ToolRegistry registry = new();
        IReadOnlyDictionary<string, object?>? received = null;
        registry.Register(new ToolDefinition(
            "GetEmployee",
            "read one employee",
            ToolRiskLevel.Read,
            ExecuteAsync: (args, _) =>
            {
                received = args;
                return ValueTask.FromResult<object?>("ok");
            },
            Parameters:
            [
                new ToolParameterSpec("employeeNo", ToolParameterType.String, true, MaxLength: 8),
                new ToolParameterSpec("year", ToolParameterType.Integer, Minimum: 2000, Maximum: 2100)
            ]));

        ToolGateway gateway = CreateGateway(registry);

        Result<object?> missing = await gateway.InvokeAsync(new ToolInvocation("GetEmployee", new Dictionary<string, object?>()));
        Assert.True(missing.Failed);
        Assert.Equal(AiErrorCodes.ToolArgumentInvalid, missing.Error.Code);

        Result<object?> unknown = await gateway.InvokeAsync(new ToolInvocation(
            "GetEmployee",
            new Dictionary<string, object?> { ["employeeNo"] = "1001", ["dropTable"] = "employees" }));
        Assert.True(unknown.Failed);
        Assert.Equal(AiErrorCodes.ToolArgumentInvalid, unknown.Error.Code);

        Result<object?> outOfRange = await gateway.InvokeAsync(new ToolInvocation(
            "GetEmployee",
            new Dictionary<string, object?> { ["employeeNo"] = "1001", ["year"] = "1900" }));
        Assert.True(outOfRange.Failed);

        Result<object?> ok = await gateway.InvokeAsync(new ToolInvocation(
            "GetEmployee",
            new Dictionary<string, object?> { ["employeeNo"] = "1001", ["year"] = "2026" }));
        Assert.True(ok.Succeeded);
        Assert.Equal(2026L, received!["year"]);
    }

    [Fact]
    public async Task Tools_are_invisible_to_other_tenants()
    {
        ToolRegistry registry = new();
        registry.Register(new ToolDefinition(
            "TenantOnly",
            "scoped tool",
            ToolRiskLevel.Read,
            ExecuteAsync: (_, _) => ValueTask.FromResult<object?>("ok"),
            TenantIds: ["t1"]));

        ToolGateway gateway = CreateGateway(registry);

        Assert.Empty(gateway.ListTools("t2"));
        Assert.Single(gateway.ListTools("t1"));

        Result<object?> denied = await gateway.InvokeAsync(new ToolInvocation("TenantOnly", new Dictionary<string, object?>(), TenantId: "t2"));
        Assert.True(denied.Failed);
        Assert.Equal(AiErrorCodes.ToolTenantDenied, denied.Error.Code);
    }

    [Fact]
    public async Task Hanging_tool_is_bounded_by_timeout()
    {
        ToolRegistry registry = new();
        registry.Register(new ToolDefinition(
            "Slow",
            "never returns quickly",
            ToolRiskLevel.Read,
            ExecuteAsync: async (_, ct) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(30), ct);
                return "too late";
            },
            Policy: new ToolExecutionPolicy(TimeSpan.FromMilliseconds(50))));

        ToolGateway gateway = CreateGateway(registry);
        Result<object?> result = await gateway.InvokeAsync(new ToolInvocation("Slow", new Dictionary<string, object?>()));

        Assert.True(result.Failed);
        Assert.Equal(AiErrorCodes.ToolTimeout, result.Error.Code);
    }

    [Fact]
    public async Task Transient_failure_is_retried_then_circuit_opens()
    {
        ToolRegistry registry = new();
        int attempts = 0;
        registry.Register(new ToolDefinition(
            "Flaky",
            "fails twice then succeeds",
            ToolRiskLevel.Read,
            ExecuteAsync: (_, _) =>
            {
                attempts++;
                if (attempts < 3)
                {
                    throw new InvalidOperationException("downstream unavailable");
                }

                return ValueTask.FromResult<object?>("recovered");
            },
            Policy: new ToolExecutionPolicy(TimeSpan.FromSeconds(5), MaxRetries: 3)));

        ToolGateway gateway = CreateGateway(registry);
        Result<object?> result = await gateway.InvokeAsync(new ToolInvocation("Flaky", new Dictionary<string, object?>()));

        Assert.True(result.Succeeded);
        Assert.Equal("recovered", result.Value);
        Assert.Equal(3, attempts);
    }

    [Fact]
    public async Task Non_retryable_failures_are_not_retried()
    {
        ToolRegistry registry = new();
        int attempts = 0;
        registry.Register(new ToolDefinition(
            "Rejecting",
            "business rejection",
            ToolRiskLevel.Read,
            ExecuteAsync: (_, _) =>
            {
                attempts++;
                throw new NonRetryableToolException("employee not found");
            },
            Policy: new ToolExecutionPolicy(MaxRetries: 5)));

        ToolGateway gateway = CreateGateway(registry);
        Result<object?> result = await gateway.InvokeAsync(new ToolInvocation("Rejecting", new Dictionary<string, object?>()));

        Assert.True(result.Failed);
        Assert.Equal(AiErrorCodes.ToolNonRetryable, result.Error.Code);
        Assert.Equal(1, attempts);
    }

    [Fact]
    public async Task Tool_call_fails_closed_when_auditing_is_unavailable()
    {
        ToolRegistry registry = new();
        int runs = 0;
        registry.Register(new ToolDefinition(
            "Read",
            "read",
            ToolRiskLevel.Read,
            ExecuteAsync: (_, _) =>
            {
                runs++;
                return ValueTask.FromResult<object?>("ok");
            }));

        ToolGateway gateway = CreateGateway(registry, new UnconfiguredAiAuditLogger());
        Result<object?> result = await gateway.InvokeAsync(new ToolInvocation("Read", new Dictionary<string, object?>()));

        Assert.True(result.Failed);
        Assert.Equal(AiErrorCodes.AuditNotConfigured, result.Error.Code);
        Assert.Equal(1, runs);
    }

    [Fact]
    public async Task Mutating_call_is_journalled_before_execution()
    {
        ToolRegistry registry = new();
        registry.Register(new ToolDefinition(
            "Write",
            "write",
            ToolRiskLevel.Write,
            ExecuteAsync: (_, _) => ValueTask.FromResult<object?>("ok")));

        InMemoryAiAuditLogger audit = new();
        ToolGateway gateway = CreateGateway(registry, audit);

        ResolvedOperation op = new("Write", "demo", ToolRiskLevel.Write, new Dictionary<string, object?>(), ToolName: "Write");
        Result<object?> result = await gateway.ExecuteConfirmedAsync(op);

        Assert.True(result.Succeeded);
        Assert.Equal(
            [AiAuditActions.ToolExecuting, AiAuditActions.ToolExecuted],
            audit.Entries.Select(e => e.Action).ToArray());
    }
}