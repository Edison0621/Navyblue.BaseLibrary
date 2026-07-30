// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : 02_DomainClassification.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="02_DomainClassification.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Xunit;

namespace Navyblue.Foundation.AI.Samples;

/// <summary>
///     Scenario 02 — Application-defined domains (not framework enums).
/// </summary>
// ReSharper disable once InconsistentNaming
public sealed class DomainClassification_02
{
    private readonly DemoErpDomainClassifier _classifier = new();

    [Theory]
    [InlineData("销售部员工有哪些", DemoDomains.Hr)]
    [InlineData("财务部员工名单", DemoDomains.Hr)]
    [InlineData("仓库人员考勤", DemoDomains.Hr)]
    [InlineData("本月销售额排名", DemoDomains.Sales)]
    [InlineData("应收逾期账龄", DemoDomains.Finance)]
    [InlineData("仓库现存量", DemoDomains.Inventory)]
    [InlineData("待审批的单据", DemoDomains.Workflow)]
    [InlineData("如何操作请假制度", DemoDomains.Knowledge)]
    public async Task Classify_domain_first(string text, string expected)
    {
        DomainClassificationResult result = await this._classifier.ClassifyAsync(UserUtterance.FromText(text));

        Assert.False(result.RequiresClarification);
        Assert.Equal(expected, result.Domain);
    }

    [Fact]
    public async Task Unknown_must_clarify_never_guess_a_business_domain()
    {
        DomainClassificationResult result = await this._classifier.ClassifyAsync(UserUtterance.FromText("今天天气怎么样"));

        Assert.True(result.RequiresClarification);
        Assert.Equal(AiWellKnownDomains.Clarify, result.Domain);
        Assert.NotEqual(DemoDomains.Sales, result.Domain);
    }

    [Fact]
    public async Task Multi_domain_sentence_asks_for_clarification()
    {
        DomainClassificationResult result = await this._classifier.ClassifyAsync(UserUtterance.FromText("销售额和应收账款"));

        Assert.Equal(AiWellKnownDomains.Clarify, result.Domain);
        Assert.NotNull(result.CompetingDomains);
        Assert.Contains(DemoDomains.Sales, result.CompetingDomains!);
        Assert.Contains(DemoDomains.Finance, result.CompetingDomains!);
    }
}