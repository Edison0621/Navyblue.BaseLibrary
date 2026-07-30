// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : DemoAiHost.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="DemoAiHost.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Navyblue.Foundation.Application;
using Navyblue.Foundation.Primitives;
using Xunit;

namespace Navyblue.Foundation.AI.Samples;

/// <summary>
///     Builds a mini "ERP AI host" the way a real application would:
///     AddNavyblueAi + app domain rules + IntentRecognizer + SlotResolver + Tools.
/// </summary>
public static class DemoAiHost
{
    /// <summary>
    ///     Creates a fully wired service provider for usage demos.
    /// </summary>
    public static ServiceProvider Create(Action<IServiceCollection>? configure = null)
    {
        ServiceCollection services = new();

        // 1) Register Foundation AI pipeline. Domain codes/rules come from the app.
        services.AddNavyblueAi(options =>
        {
            options.RegisterFakes = false;

            // A production host would register durable draft/audit stores instead.
            options.AllowInMemoryPersistence = true;
            DemoDomains.ConfigureRules(options);
        });

        // 2) Replace placeholders with business implementations.
        services.RemoveAll<IDomainClassifier>();
        services.RemoveAll<IIntentRecognizer>();
        services.RemoveAll<ISlotResolver>();
        services.RemoveAll<IPermissionGuard>();
        services.AddSingleton<IDomainClassifier>(sp => new DemoErpDomainClassifier(sp.GetRequiredService<NavyblueAiOptions>()));
        services.AddSingleton<DemoPermissionChecker>();
        services.AddSingleton<IPermissionChecker>(sp => sp.GetRequiredService<DemoPermissionChecker>());
        services.AddSingleton<IPermissionGuard>(sp => new PermissionCheckerGuard(sp.GetRequiredService<IPermissionChecker>()));
        services.AddSingleton<IIntentRecognizer, DemoIntentRecognizer>();
        services.AddSingleton<ISlotResolver, DemoSlotResolver>();

        configure?.Invoke(services);

        ServiceProvider provider = services.BuildServiceProvider();

        // 3) Seed the process-wide tool catalog (usually done once at startup).
        DemoTools.RegisterAll(provider.GetRequiredService<IToolRegistry>());

        return provider;
    }
}

/// <summary>
///     Serializes the scenarios that assert on <see cref="DemoTools.CreatedEmployees" />, which is shared state.
/// </summary>
[CollectionDefinition(Name)]
public sealed class DemoToolsCollection
{
    public const string Name = "demo-tools";
}

/// <summary>
///     Demo tools that stand in for Application Services.
/// </summary>
public static class DemoTools
{
    public const string GetSalesRank = "sales.get_rank";
    public const string GetEmployee = "hr.get_employee";
    public const string CreateEmployee = "hr.create_employee";
    public const string GetStock = "inventory.get_stock";

    public static readonly List<object> CreatedEmployees = [];

    public static void RegisterAll(IToolRegistry registry)
    {
        // Declared parameters are enforced by the gateway before the executor runs, so an LLM cannot
        // smuggle extra or malformed arguments into an Application Service.
        registry.Register(new ToolDefinition(
            GetSalesRank,
            "Query sales ranking",
            ToolRiskLevel.Read,
            PermissionName: "sales.rank.read",
            ExecuteAsync: (args, _) =>
            {
                string period = args.TryGetValue("period", out object? p) ? p?.ToString() ?? "month" : "month";
                return ValueTask.FromResult<object?>(new { Period = period, Top = new[] { "Alice", "Bob", "Carol" } });
            },
            Parameters: [new ToolParameterSpec("period", AllowedValues: ["day", "week", "month", "year"])],
            Policy: new ToolExecutionPolicy(TimeSpan.FromSeconds(5), MaxRetries: 1, RetryDelay: TimeSpan.FromMilliseconds(20))));

        registry.Register(new ToolDefinition(
            GetEmployee,
            "Get employee by badge",
            ToolRiskLevel.Read,
            PermissionName: "hr.employee.read",
            ExecuteAsync: (args, _) =>
            {
                string badge = args.TryGetValue("badgeNo", out object? b) ? b?.ToString() ?? "" : "";
                return ValueTask.FromResult<object?>(new { BadgeNo = badge, Name = "张三", Dept = "销售部" });
            },
            Parameters: [new ToolParameterSpec("badgeNo", ToolParameterType.String, true, MaxLength: 16)]));

        registry.Register(new ToolDefinition(
            CreateEmployee,
            "Create employee",
            ToolRiskLevel.Write,
            PermissionName: "hr.employee.write",
            ExecuteAsync: (args, _) =>
            {
                CreatedEmployees.Add(args);
                return ValueTask.FromResult<object?>(new { Ok = true, BadgeNo = "E-1001" });
            },
            Parameters: [new ToolParameterSpec("name", ToolParameterType.String, true, MaxLength: 32)]));

        registry.Register(new ToolDefinition(
            GetStock,
            "Get on-hand quantity",
            ToolRiskLevel.Read,
            PermissionName: "inventory.stock.read",
            ExecuteAsync: (args, _) =>
            {
                string sku = args.TryGetValue("sku", out object? s) ? s?.ToString() ?? "" : "";
                return ValueTask.FromResult<object?>(new { Sku = sku, OnHand = 128 });
            },
            Parameters: [new ToolParameterSpec("sku", ToolParameterType.String, true, MaxLength: 32)]));
    }

    public static void Reset() => CreatedEmployees.Clear();
}

/// <summary>
///     Demo recognizer: maps Domain + utterance keywords to IntentCandidate only.
/// </summary>
public sealed class DemoIntentRecognizer : IIntentRecognizer
{
    #region IIntentRecognizer Members

    public ValueTask<Result<IntentCandidate>> RecognizeAsync(UserUtterance utterance, string domain, CancellationToken cancellationToken = default)
    {
        string text = utterance.Text;

        IntentCandidate? candidate = domain switch
        {
            DemoDomains.Sales when text.Contains("排名", StringComparison.Ordinal) || text.Contains("销售额", StringComparison.Ordinal)
                => new("GetSalesRank", DemoDomains.Sales, 0.92, [new SlotValue("period", "month")]),

            DemoDomains.Hr when text.Contains("新增", StringComparison.Ordinal) || text.Contains("创建", StringComparison.Ordinal)
                => new("CreateEmployee", DemoDomains.Hr, 0.9, [new SlotValue("name", ExtractName(text))]),

            DemoDomains.Hr when text.Contains("工号", StringComparison.Ordinal) || text.Contains("员工", StringComparison.Ordinal)
                => new("GetEmployee", DemoDomains.Hr, 0.9, [new SlotValue("badgeNo", ExtractBadge(text))]),

            DemoDomains.Inventory when text.Contains("现存", StringComparison.Ordinal) || text.Contains("库存", StringComparison.Ordinal)
                => new("GetStock", DemoDomains.Inventory, 0.9, [new SlotValue("sku", "SKU-001")]),

            _ => null
        };

        return candidate is null
            ? ValueTask.FromResult(Result.Failure<IntentCandidate>(new Error("demo.intent.unknown", $"No intent for domain '{domain}'.")))
            : ValueTask.FromResult(Result.Success(candidate));
    }

    #endregion

    private static string ExtractName(string text)
    {
        int idx = text.IndexOf("员工", StringComparison.Ordinal);
        return idx >= 0 && idx + 2 < text.Length ? text[(idx + 2)..].Trim() : "新员工";
    }

    private static string ExtractBadge(string text)
    {
        Match m = Regex.Match(text, @"\d{3,}");
        return m.Success ? m.Value : "1001";
    }
}

/// <summary>
///     Demo resolver: turns candidate slots into final ResolvedOperation parameters.
/// </summary>
public sealed class DemoSlotResolver : ISlotResolver
{
    #region ISlotResolver Members

    public ValueTask<Result<ResolvedOperation>> ResolveAsync(IntentCandidate candidate, UserUtterance utterance, CancellationToken cancellationToken = default)
    {
        Dictionary<string, object?> parameters = candidate.Slots.ToDictionary(s => s.Name, s => (object?)s.RawValue, StringComparer.OrdinalIgnoreCase);

        if (candidate.IntentName == "CreateEmployee")
        {
            string? name = parameters.TryGetValue("name", out object? n) ? n?.ToString() : null;
            if (string.IsNullOrWhiteSpace(name))
            {
                return ValueTask.FromResult(Result.Failure<ResolvedOperation>(new Error("demo.resolve.name_required", "员工姓名不能为空。")));
            }

            parameters["name"] = name.Trim();
        }

        (string tool, ToolRiskLevel risk, string permission) = candidate.IntentName switch
        {
            "GetSalesRank" => (DemoTools.GetSalesRank, ToolRiskLevel.Read, "sales.rank.read"),
            "GetEmployee" => (DemoTools.GetEmployee, ToolRiskLevel.Read, "hr.employee.read"),
            "CreateEmployee" => (DemoTools.CreateEmployee, ToolRiskLevel.Write, "hr.employee.write"),
            "GetStock" => (DemoTools.GetStock, ToolRiskLevel.Read, "inventory.stock.read"),
            _ => (candidate.IntentName, ToolRiskLevel.Read, "unknown")
        };

        ResolvedOperation operation = new(
            candidate.IntentName,
            candidate.Domain,
            risk,
            parameters,
            permission,
            tool);

        return ValueTask.FromResult(Result.Success(operation));
    }

    #endregion
}

/// <summary>
///     Mutable permission checker for permission demos.
/// </summary>
public sealed class DemoPermissionChecker : IPermissionChecker
{
    private readonly HashSet<string> _granted = new(StringComparer.OrdinalIgnoreCase)
    {
        "sales.rank.read",
        "hr.employee.read",
        "hr.employee.write",
        "inventory.stock.read"
    };

    #region IPermissionChecker Members

    public ValueTask<bool> IsGrantedAsync(string permissionName, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(this._granted.Contains(permissionName));

    #endregion

    public void Grant(string permission) => this._granted.Add(permission);

    public void Revoke(string permission) => this._granted.Remove(permission);
}