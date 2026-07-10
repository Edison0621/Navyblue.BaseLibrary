// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : EventHandler.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="EventHandler.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

namespace Navyblue.Foundation.Cqrs
{
    /// <summary>
    ///     Base handler for Events
    /// </summary>
    /// <typeparam name="TEvent">Type of the event</typeparam>
    public abstract class EventHandler<TEvent>
        : RequestHandler<TEvent, VoidResult>
        where TEvent : Event
    {
    }
}