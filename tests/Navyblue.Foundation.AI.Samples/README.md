# Navyblue.Foundation.AI Samples

本项目是 **可运行的用法样板**，不是底层单元测试。  
通过阅读并运行这些用例，可以快速理解如何接入 `Navyblue.Foundation.AI`。

## 运行

```bash
dotnet test tests/Navyblue.Foundation.AI.Samples -c Release
```

## 场景索引

| 文件 | 教你做什么 |
| --- | --- |
| `DemoDomains.cs` | **应用自定义** Domain 编码与关键词（框架不内置 HR/Sales） |
| `01_GettingStarted.cs` | `AddNavyblueAi` 最小 DI 注册 |
| `02_DomainClassification.cs` | Domain 分类与 Clarify |
| `03_ReadQueryFlow.cs` | 读操作：分类 → 意图 → 解析 → 直接执行 |
| `04_WriteDraftConfirmFlow.cs` | 写操作：草稿 → 用户确认 → 执行 / 拒绝 |
| `05_PermissionAndAudit.cs` | 权限拦截与审计日志 |
| `06_McpBridge.cs` | MCP 桥接必须走 ToolGateway |
| `07_MultiTurnAndCancellation.cs` | 多轮补槽、草稿取消、并发确认只执行一次 |
| `08_ResilienceObservabilityAndMcp.cs` | 参数强校验、链路追踪、MCP schema 生成、负样本回归 |

## 推荐阅读顺序

1. `DemoDomains.cs` / `DemoAiHost.cs` — 业务侧如何定义 Domain、组装 Recognizer / Resolver / Tools  
2. 按编号打开各场景文件，注释即文档  
3. 对照正式文档：`docs/Navyblue.BaseLibrary.zh-CN.md` →「AI 能力」

## 核心约束（务必遵守）

1. 框架只保留 `unknown` / `clarify`；业务 Domain 由应用用字符串自定义  
2. LLM / Recognizer **只产出候选** Intent + Slots  
3. 最终业务参数由 `ISlotResolver`（Schema / 主数据 / 规则）决定  
4. 写操作与高风险操作必须 **草稿 → 确认 → 执行**（归属校验 + TTL）  
5. Agent / MCP **禁止绕过** `IToolGateway`；Agent 不自动执行 Write/HighRisk  
6. 生产默认权限 **fail-closed**；演示可用 `UseDevelopmentDefaults()` 或 `PermissionCheckerGuard`  
7. 生产必须注册持久化 `IDraftStore` 与 `IAiAuditLogger`；未注册时草稿与工具调用都会失败  
8. 工具声明 `Parameters` 后由网关强制校验，未声明的参数一律拒绝

## 生产能力速览

| 能力 | 入口 |
| --- | --- |
| 编排管线 | `IAiOrchestrator`（`ProcessAsync` / `ConfirmDraftAsync` / `CancelDraftAsync`） |
| 工具目录（单例） | `IToolRegistry` / `AddNavyblueAiTool` + `UseNavyblueAiTools` |
| 工具治理（请求级） | `IToolGateway` / `ListTools(tenantId)` |
| 参数校验 | `ToolDefinition.Parameters` + `IToolArgumentValidator` |
| 弹性 | `ToolExecutionPolicy`（超时 / 重试 / 熔断 / 并发） |
| 草稿治理 | `IDraftStore.TryBeginExecutionAsync`、`RequireDistinctApprovers` |
| 多步 Agent | `IAiAgent` / `IAgentPlanner` / `AiBudget` |
| 知识域 | `KnowledgeDomains` + `AddNavyblueAiKnowledgeRetriever<T>()` |
| 会话与多轮 | `UserUtterance.ConversationId` + `IConversationStore` |
| 安全 | `IContentSafetyFilter` / `IPromptInjectionGuard` |
| 配额 | `IAiQuotaGuard` / `QuotaPermitLimit` |
| 可观测性 | `AiTelemetry.ActivitySourceName` / `MeterName` |
| 回归防护 | `IntentEvaluationHarness` |
| MCP | `IMcpToolBridge` + `McpSchemaGenerator` |
| 管线扩展 | `IAiPipelineBehavior` |
