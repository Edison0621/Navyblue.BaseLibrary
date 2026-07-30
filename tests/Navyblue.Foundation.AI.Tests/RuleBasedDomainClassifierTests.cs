// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : RuleBasedDomainClassifierTests.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="RuleBasedDomainClassifierTests.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Xunit;

namespace Navyblue.Foundation.AI.Tests;

/// <summary>
///     Framework-level tests for the configurable keyword classifier (no business domains baked in).
/// </summary>
public sealed class RuleBasedDomainClassifierTests
{
    [Fact]
    public async Task Empty_rules_always_clarify()
    {
        RuleBasedDomainClassifier classifier = new(new NavyblueAiOptions());
        DomainClassificationResult result = await classifier.ClassifyAsync(UserUtterance.FromText("anything"));
        Assert.Equal(AiWellKnownDomains.Clarify, result.Domain);
        Assert.True(result.RequiresClarification);
    }

    [Fact]
    public async Task Matches_configured_application_domain()
    {
        NavyblueAiOptions options = new() { MinDomainConfidence = 0.5 };
        options.DomainRules.Add(new DomainKeywordRule("orders", "订单", "下单"));
        options.DomainRules.Add(new DomainKeywordRule("catalog", "商品", "SKU"));

        RuleBasedDomainClassifier classifier = new(options);
        DomainClassificationResult result = await classifier.ClassifyAsync(UserUtterance.FromText("查询今日订单"));

        Assert.Equal("orders", result.Domain);
        Assert.False(result.RequiresClarification);
    }

    [Fact]
    public async Task Equal_strength_domains_clarify()
    {
        NavyblueAiOptions options = new();
        options.DomainRules.Add(new DomainKeywordRule("alpha", "关键词A"));
        options.DomainRules.Add(new DomainKeywordRule("beta", "关键词B"));

        RuleBasedDomainClassifier classifier = new(options);
        DomainClassificationResult result = await classifier.ClassifyAsync(UserUtterance.FromText("关键词A 和 关键词B"));

        Assert.Equal(AiWellKnownDomains.Clarify, result.Domain);
        Assert.Contains("alpha", result.CompetingDomains!);
        Assert.Contains("beta", result.CompetingDomains!);
    }

    [Fact]
    public async Task Empty_text_clarifies()
    {
        NavyblueAiOptions options = new();
        options.DomainRules.Add(new DomainKeywordRule("orders", "订单"));
        RuleBasedDomainClassifier classifier = new(options);

        DomainClassificationResult result = await classifier.ClassifyAsync(UserUtterance.FromText("   "));
        Assert.True(result.RequiresClarification);
    }

    [Fact]
    public async Task Unknown_utterance_clarifies_without_defaulting()
    {
        NavyblueAiOptions options = new();
        options.DomainRules.Add(new DomainKeywordRule("orders", "订单"));
        RuleBasedDomainClassifier classifier = new(options);

        DomainClassificationResult result = await classifier.ClassifyAsync(UserUtterance.FromText("今天天气怎么样"));
        Assert.Equal(AiWellKnownDomains.Clarify, result.Domain);
        Assert.NotEqual("orders", result.Domain);
    }
}