// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : EventHandler.cs
// Created          : 2026-07-10  17:07
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="EventHandler.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Cqrs;

/// <summary>
///     Base handler for Events
/// </summary>
/// <typeparam name="TEvent">Type of the event</typeparam>
public abstract class EventHandler<TEvent>
    : RequestHandler<TEvent, VoidResult>
    where TEvent : Event
{
}