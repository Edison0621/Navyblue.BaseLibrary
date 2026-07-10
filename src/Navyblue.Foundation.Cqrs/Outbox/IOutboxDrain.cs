// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : IOutboxDrain.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="IOutboxDrain.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
/// 
/// </summary>
public interface IOutboxDrain
{
    /// <summary>
    /// Drains this instance.
    /// </summary>
    /// <returns></returns>
    IEnumerable<Event> Drain();
}