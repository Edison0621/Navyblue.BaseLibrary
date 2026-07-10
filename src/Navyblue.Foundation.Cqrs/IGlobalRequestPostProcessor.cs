// ******************************************************************************************************
// Project          : CQRS.Mediatr.Lite.Samples
// File             : IGlobalRequestPostProcessor.cs
// Created          : 2025-11-14  15:11
// 
// Last Modified By : Edison.Ma(jstsmaxx@163.com)
// Last Modified On : 2025-11-14  15:22
// ******************************************************************************************************
// <copyright file="IGlobalRequestPostProcessor.cs" company="">
//     Copyright ©  2011-2025. All rights reserved.
// </copyright>
// ******************************************************************************************************

namespace Navyblue.Foundation.Cqrs
{
    public interface IGlobalRequestPostProcessor : IRequestPostProcessor<IRequest, object>
    {
    }
}