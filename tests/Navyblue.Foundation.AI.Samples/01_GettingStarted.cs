// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : 01_GettingStarted.cs
// Created          : 2026-07-30  14:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-30  16:31
// ****************************************************************************************************************************************
// <copyright file="01_GettingStarted.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Navyblue.Foundation.AI.Samples;

/// <summary>
///     Scenario 01 — Minimal DI setup.
/// </summary>
// ReSharper disable once InconsistentNaming
public sealed class GettingStarted_01
{
    [Fact]
    public void Step1_AddNavyblueAi_registers_pipeline_services()
    {
        ServiceCollection services = new();
        // Production defaults are fail-closed; RegisterFakes only for smoke wiring.
        services.AddNavyblueAi(o => o.RegisterFakes = true);

        using ServiceProvider sp = services.BuildServiceProvider();

        Assert.NotNull(sp.GetRequiredService<IDomainClassifier>());
        Assert.NotNull(sp.GetRequiredService<IAiOrchestrator>());
        Assert.NotNull(sp.GetRequiredService<IToolGateway>());
        Assert.NotNull(sp.GetRequiredService<IDraftStore>());
        Assert.NotNull(sp.GetRequiredService<IAiAuditLogger>());
        Assert.NotNull(sp.GetRequiredService<IMcpToolBridge>());
        Assert.IsType<FailClosedPermissionGuard>(sp.GetRequiredService<IPermissionGuard>());
    }

    [Fact]
    public void Step2_App_registers_own_domains_and_implementations()
    {
        using ServiceProvider sp = DemoAiHost.Create();

        Assert.IsType<DemoErpDomainClassifier>(sp.GetRequiredService<IDomainClassifier>());
        Assert.IsType<DemoIntentRecognizer>(sp.GetRequiredService<IIntentRecognizer>());
        Assert.IsType<DemoSlotResolver>(sp.GetRequiredService<ISlotResolver>());
    }
}