// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : AiServiceCollectionExtensions.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="AiServiceCollectionExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Navyblue.Foundation.Domain;

namespace Navyblue.Foundation.AI;

/// <summary>
///     What happens when the audit sink cannot record a tool call.
/// </summary>
public enum AuditFailurePolicy
{
    /// <summary>Abort the operation. Required where every tool call must be auditable.</summary>
    FailClosed = 0,

    /// <summary>Proceed despite the audit failure. Only for non-regulated workloads.</summary>
    Continue = 1
}

/// <summary>
///     Options for <see cref="AiServiceCollectionExtensions.AddNavyblueAi" />.
/// </summary>
public sealed class NavyblueAiOptions
{
    /// <summary>
    ///     Minimum domain classification confidence before Clarify is returned.
    /// </summary>
    public double MinDomainConfidence { get; set; } = 0.6;

    /// <summary>
    ///     Minimum intent confidence before Clarify is returned.
    /// </summary>
    public double MinIntentConfidence { get; set; } = 0.5;

    /// <summary>
    ///     Application-defined domain keyword rules. Empty by default — apps must register their own domains.
    /// </summary>
    public IList<DomainKeywordRule> DomainRules { get; } = [];

    /// <summary>
    ///     Application domain codes that route to <see cref="IKnowledgeRetriever" /> instead of tools.
    /// </summary>
    public IList<string> KnowledgeDomains { get; } = [];

    /// <summary>
    ///     Tools seeded into <see cref="IToolRegistry" /> at startup.
    /// </summary>
    public IList<ToolDefinition> Tools { get; } = [];

    /// <summary>
    ///     Top-K for knowledge retrieval.
    /// </summary>
    public int KnowledgeTopK { get; set; } = 5;

    /// <summary>
    ///     When true, knowledge hits are filtered by tenant and per-document permission before reaching the model.
    /// </summary>
    public bool EnableKnowledgePermissionFiltering { get; set; } = true;

    /// <summary>
    ///     Default draft TTL.
    /// </summary>
    public TimeSpan DraftTimeToLive { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    ///     When true, HighRisk drafts require <see cref="DraftConfirmationRequest.HighRiskAcknowledged" />.
    /// </summary>
    public bool RequireHighRiskAcknowledgement { get; set; } = true;

    /// <summary>
    ///     When true, HighRisk drafts require two successful confirmations before execution.
    /// </summary>
    public bool RequireDoubleConfirmForHighRisk { get; set; }

    /// <summary>
    ///     When true, a user may confirm a given draft only once, so double confirmation implies a second approver
    ///     (maker-checker / four-eyes).
    /// </summary>
    public bool RequireDistinctApprovers { get; set; }

    /// <summary>
    ///     Behavior when an audit write fails. Defaults to aborting the operation.
    /// </summary>
    public AuditFailurePolicy AuditFailurePolicy { get; set; } = AuditFailurePolicy.FailClosed;

    /// <summary>
    ///     When true, arguments not declared in <see cref="ToolDefinition.Parameters" /> are rejected.
    /// </summary>
    public bool RejectUnknownToolArguments { get; set; } = true;

    /// <summary>
    ///     Execution policy applied to tools that declare none.
    /// </summary>
    public ToolExecutionPolicy DefaultToolExecutionPolicy { get; set; } = new(TimeSpan.FromSeconds(30));

    /// <summary>
    ///     Max steps for <see cref="ToolCallingAgent" /> when no budget overrides it.
    /// </summary>
    public int MaxAgentSteps { get; set; } = 8;

    /// <summary>
    ///     Default budget applied to agent runs.
    /// </summary>
    public AiBudget? DefaultAgentBudget { get; set; } = new(MaxDuration: TimeSpan.FromMinutes(2), MaxToolCalls: 16);

    /// <summary>
    ///     When true, the agent aborts if the planner repeats an identical tool call.
    /// </summary>
    public bool DetectAgentLoops { get; set; } = true;

    /// <summary>
    ///     Maximum retained conversation turns per conversation.
    /// </summary>
    public int MaxConversationTurns { get; set; } = 100;

    /// <summary>
    ///     Requests allowed per <see cref="QuotaWindow" /> per user/tenant. Zero disables the built-in quota guard.
    /// </summary>
    public int QuotaPermitLimit { get; set; }

    /// <summary>
    ///     Window for <see cref="QuotaPermitLimit" />.
    /// </summary>
    public TimeSpan QuotaWindow { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    ///     Lifetime of request-scoped pipeline services (orchestrator, gateway, agent, guards).
    ///     Scoped is correct for server applications whose permission guard and resolvers depend on the current user.
    /// </summary>
    public ServiceLifetime PipelineLifetime { get; set; } = ServiceLifetime.Scoped;

    /// <summary>
    ///     When true, in-memory draft storage and auditing are registered. Production deployments must leave this
    ///     false and register durable implementations; otherwise drafts and the audit trail fail closed.
    /// </summary>
    public bool AllowInMemoryPersistence { get; set; }

    /// <summary>
    ///     When true, registers AllowAll permission guard and other permissive helpers. Default is false (fail-closed).
    /// </summary>
    public bool UsePermissiveDefaults { get; set; }

    /// <summary>
    ///     When true, registers Fake LLM and unavailable recognizer/resolver placeholders, plus in-memory
    ///     persistence so tests and demos run end to end.
    /// </summary>
    public bool RegisterFakes { get; set; }

    /// <summary>
    ///     Enables every non-production convenience at once: permissive permissions and in-memory persistence.
    ///     Never call this from a production host.
    /// </summary>
    public NavyblueAiOptions UseDevelopmentDefaults()
    {
        this.UsePermissiveDefaults = true;
        this.AllowInMemoryPersistence = true;
        return this;
    }
}

/// <summary>
///     DI registration for Navyblue Foundation AI (production defaults are fail-closed; no business domains).
/// </summary>
public static class AiServiceCollectionExtensions
{
    /// <summary>
    ///     Registers AI pipeline services. Apps must register <see cref="IIntentRecognizer" />, <see cref="ISlotResolver" />,
    ///     domain rules via <see cref="NavyblueAiOptions.DomainRules" />, and — for production — durable
    ///     <see cref="IDraftStore" /> and <see cref="IAiAuditLogger" /> implementations.
    /// </summary>
    public static IServiceCollection AddNavyblueAi(this IServiceCollection services, Action<NavyblueAiOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        NavyblueAiOptions options = new();
        configure?.Invoke(options);
        bool inMemory = options.AllowInMemoryPersistence || options.RegisterFakes;

        services.TryAddSingleton(options);
        services.TryAddSingleton<IClock, SystemClock>();

        // Process-wide state: the tool catalog and the resilience circuit/limiter state must be shared.
        services.TryAddSingleton<IToolRegistry>(_ =>
        {
            ToolRegistry registry = new();
            foreach (ToolDefinition tool in options.Tools)
            {
                registry.Register(tool);
            }

            return registry;
        });
        services.TryAddSingleton(sp => new ToolResilienceCoordinator(sp.GetRequiredService<IClock>()));
        services.TryAddSingleton<IDomainClassifier>(_ => new RuleBasedDomainClassifier(options));
        services.TryAddSingleton<IToolArgumentValidator>(_ => new SpecToolArgumentValidator(options));
        services.TryAddSingleton<IContentSafetyFilter, NullContentSafetyFilter>();
        services.TryAddSingleton<IPromptInjectionGuard, KeywordPromptInjectionGuard>();
        services.TryAddSingleton<InMemoryConversationStore>();
        services.TryAddSingleton<IConversationStore>(sp => sp.GetRequiredService<InMemoryConversationStore>());

        RegisterQuota(services, options);
        RegisterPersistence(services, options, inMemory);
        RegisterPipeline(services, options);
        RegisterPermissions(services, options);

        if (options.RegisterFakes)
        {
            services.TryAddSingleton<FakeLlmClient>();
            services.TryAddSingleton<ILlmClient>(sp => sp.GetRequiredService<FakeLlmClient>());
            services.TryAddSingleton<IStreamingLlmClient>(sp => sp.GetRequiredService<FakeLlmClient>());
            services.TryAddSingleton<IIntentRecognizer, UnavailableIntentRecognizer>();
            services.TryAddSingleton<ISlotResolver, UnavailableSlotResolver>();
        }

        return services;
    }

    /// <summary>
    ///     Registers a tool that will be seeded into <see cref="IToolRegistry" /> at startup.
    ///     Call before or after <see cref="AddNavyblueAi" />; both orders are supported.
    /// </summary>
    public static IServiceCollection AddNavyblueAiTool(this IServiceCollection services, ToolDefinition tool)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(tool);

        services.AddSingleton(new ToolRegistration(tool));
        return services;
    }

    /// <summary>
    ///     Applies tools registered through <see cref="AddNavyblueAiTool" /> to the live registry.
    ///     Call once during application start.
    /// </summary>
    public static IServiceProvider UseNavyblueAiTools(this IServiceProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        IToolRegistry registry = provider.GetRequiredService<IToolRegistry>();
        foreach (ToolRegistration registration in provider.GetServices<ToolRegistration>())
        {
            registry.Register(registration.Tool);
        }

        return provider;
    }

    /// <summary>
    ///     Registers the application's knowledge retriever. When
    ///     <see cref="NavyblueAiOptions.EnableKnowledgePermissionFiltering" /> is on, it is wrapped so RAG results
    ///     are filtered by tenant and per-document permission before they can reach the model.
    ///     Call after <see cref="AddNavyblueAi" />.
    /// </summary>
    public static IServiceCollection AddNavyblueAiKnowledgeRetriever<TRetriever>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TRetriever : class, IKnowledgeRetriever
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Add(new ServiceDescriptor(typeof(TRetriever), typeof(TRetriever), lifetime));
        services.Replace(new ServiceDescriptor(
            typeof(IKnowledgeRetriever),
            sp =>
            {
                TRetriever retriever = sp.GetRequiredService<TRetriever>();
                NavyblueAiOptions options = sp.GetRequiredService<NavyblueAiOptions>();
                return options.EnableKnowledgePermissionFiltering
                    ? new PermissionFilteringKnowledgeRetriever(retriever, sp.GetRequiredService<IPermissionGuard>())
                    : retriever;
            },
            lifetime));

        return services;
    }

    private static void RegisterQuota(IServiceCollection services, NavyblueAiOptions options)
    {
        if (options.QuotaPermitLimit > 0)
        {
            services.TryAddSingleton<IAiQuotaGuard>(sp => new InMemoryFixedWindowQuotaGuard(
                options.QuotaPermitLimit,
                options.QuotaWindow,
                sp.GetRequiredService<IClock>()));
        }
        else
        {
            services.TryAddSingleton<IAiQuotaGuard, NullAiQuotaGuard>();
        }
    }

    private static void RegisterPersistence(IServiceCollection services, NavyblueAiOptions options, bool inMemory)
    {
        if (inMemory)
        {
            services.TryAddSingleton<InMemoryAiAuditLogger>();
            services.TryAddSingleton<IAiAuditLogger>(sp => sp.GetRequiredService<InMemoryAiAuditLogger>());
            services.TryAddSingleton<InMemoryDraftStore>(sp => new InMemoryDraftStore(sp.GetRequiredService<IClock>(), options));
            services.TryAddSingleton<IDraftStore>(sp => sp.GetRequiredService<InMemoryDraftStore>());
            return;
        }

        // Fail closed: a deployment that forgot durable persistence must not silently lose drafts or audit records.
        services.TryAddSingleton<IAiAuditLogger, UnconfiguredAiAuditLogger>();
        services.TryAddSingleton<IDraftStore, UnconfiguredDraftStore>();
    }

    private static void RegisterPipeline(IServiceCollection services, NavyblueAiOptions options)
    {
        ServiceLifetime lifetime = options.PipelineLifetime;

        services.TryAdd(new ServiceDescriptor(typeof(IBusinessRuleValidator), typeof(NullBusinessRuleValidator), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IConfirmationService), typeof(ConfirmationService), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(ToolGateway), typeof(ToolGateway), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IToolGateway), sp => sp.GetRequiredService<ToolGateway>(), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IMcpToolBridge), typeof(McpToolBridge), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IAgentPlanner), typeof(UnavailableAgentPlanner), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IAiAgent), typeof(ToolCallingAgent), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IAiOrchestrator), typeof(AiOrchestrator), lifetime));

        services.TryAdd(new ServiceDescriptor(typeof(IKnowledgeRetriever), typeof(NullKnowledgeRetriever), lifetime));
    }

    private static void RegisterPermissions(IServiceCollection services, NavyblueAiOptions options)
    {
        Type guardType = options.UsePermissiveDefaults ? typeof(AllowAllPermissionGuard) : typeof(FailClosedPermissionGuard);
        services.TryAdd(new ServiceDescriptor(typeof(IPermissionGuard), guardType, options.PipelineLifetime));
    }

    #region Nested type: ToolRegistration

    /// <summary>
    ///     Marker used to collect tools registered through <see cref="AddNavyblueAiTool" />.
    /// </summary>
    public sealed record ToolRegistration(ToolDefinition Tool);

    #endregion
}