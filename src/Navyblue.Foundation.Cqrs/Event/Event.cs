// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : Event.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:21
// ******************************************************************************************************
// <copyright file="Event.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

using System;

namespace Navyblue.Foundation.Cqrs
{
    /// <summary>
    ///     Base representation for an Event
    /// </summary>
    public abstract class Event : IRequest
    {
        public string CorrelationId { get; set; }
        public string TransactionId { get; set; }
        public string GeneratedBy { get; set; }
        public DateTime GeneratedOn { get; set; }
        public string Version { get; set; }

        #region IRequest Members

        public abstract string DisplayName { get; }
        public abstract string Id { get; set; }

        /// <summary>
        ///     Validates an event.
        /// </summary>
        /// <param name="validationErrorMessage">Error Message, if any</param>
        /// <returns>True - If event is valid</returns>
        /// <remarks>Ideally therey is no need for events to have a validation</remarks>
        public virtual bool Validate(out string validationErrorMessage)
        {
            // Events in general doesn't need to be validated
            validationErrorMessage = null;
            return true;
        }

        #endregion
    }
}