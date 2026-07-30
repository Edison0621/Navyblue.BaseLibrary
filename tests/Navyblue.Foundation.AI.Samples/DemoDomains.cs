// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : DemoDomains.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="DemoDomains.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.AI.Samples;

/// <summary>
///     Sample application domain codes. These belong to the app, not the framework.
/// </summary>
public static class DemoDomains
{
    public const string Hr = "hr";
    public const string Sales = "sales";
    public const string Finance = "finance";
    public const string Inventory = "inventory";
    public const string Workflow = "workflow";
    public const string Knowledge = "knowledge";

    /// <summary>
    ///     Registers demo ERP keyword rules into framework options.
    /// </summary>
    public static void ConfigureRules(NavyblueAiOptions options)
    {
        options.MinDomainConfidence = 0.6;
        options.DomainRules.Clear();
        options.DomainRules.Add(new DomainKeywordRule(Hr, "员工", "人员", "部门", "岗位", "工号", "员工档案", "组织架构", "人事", "入职", "离职", "考勤"));
        options.DomainRules.Add(new DomainKeywordRule(Sales, "销售额", "销售排名", "销售趋势", "客户销售", "产品销售", "毛利", "业绩", "订单金额", "成交额"));
        options.DomainRules.Add(new DomainKeywordRule(Finance, "应收", "应付", "账龄", "逾期", "付款", "收款", "发票", "对账", "费用报销"));
        options.DomainRules.Add(new DomainKeywordRule(Inventory, "库存", "仓库库存", "现存量", "可用量", "安全库存", "缺货", "积压", "出入库"));
        options.DomainRules.Add(new DomainKeywordRule(Workflow, "审批", "待审批", "提交审批", "审批通过", "驳回", "流程节点"));
        options.DomainRules.Add(new DomainKeywordRule(Knowledge, "制度", "流程手册", "操作手册", "规则说明", "如何操作", "帮助文档", "知识库"));
        if (!options.KnowledgeDomains.Contains(Knowledge, StringComparer.OrdinalIgnoreCase))
        {
            options.KnowledgeDomains.Add(Knowledge);
        }
    }
}

/// <summary>
///     Demo classifier: applies ERP-specific precedence (department + employee → HR),
///     then falls back to the framework <see cref="RuleBasedDomainClassifier" />.
/// </summary>
public sealed class DemoErpDomainClassifier : IDomainClassifier
{
    private readonly RuleBasedDomainClassifier _inner;

    public DemoErpDomainClassifier(NavyblueAiOptions? options = null)
    {
        NavyblueAiOptions opts = options ?? new NavyblueAiOptions();
        if (opts.DomainRules.Count == 0)
        {
            DemoDomains.ConfigureRules(opts);
        }

        this._inner = new RuleBasedDomainClassifier(opts);
    }

    #region IDomainClassifier Members

    public async ValueTask<DomainClassificationResult> ClassifyAsync(UserUtterance utterance, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(utterance);
        string text = utterance.Text?.Trim() ?? string.Empty;

        // Application rule: "销售部员工" is HR, not Sales.
        if (IsPersonnelQuery(text))
        {
            return new DomainClassificationResult(DemoDomains.Hr, 0.95, "Demo rule: personnel/org queries route to hr.");
        }

        return await this._inner.ClassifyAsync(utterance, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    private static bool IsPersonnelQuery(string text)
    {
        bool hasPersonnel = text.Contains("员工", StringComparison.Ordinal)
                            || text.Contains("人员", StringComparison.Ordinal)
                            || text.Contains("工号", StringComparison.Ordinal)
                            || text.Contains("人事", StringComparison.Ordinal);
        if (!hasPersonnel)
        {
            return false;
        }

        bool hasDeptCue = text.Contains("部", StringComparison.Ordinal)
                          || text.Contains("仓库", StringComparison.Ordinal)
                          || text.Contains("岗位", StringComparison.Ordinal)
                          || text.Contains("部门", StringComparison.Ordinal);

        bool looksLikeSalesMetric = text.Contains("销售额", StringComparison.Ordinal)
                                    || text.Contains("毛利", StringComparison.Ordinal)
                                    || text.Contains("业绩", StringComparison.Ordinal)
                                    || text.Contains("销售排名", StringComparison.Ordinal);

        return hasDeptCue || !looksLikeSalesMetric;
    }
}