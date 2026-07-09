// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : StronglyTypedId.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="StronglyTypedId.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Domain;

/// <summary>
///     The strongly typed id interface.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public interface IStronglyTypedId<out TValue> where TValue : notnull
{
    /// <summary>
    ///     Gets the value.
    /// </summary>
    /// <value>
    ///     The value.
    /// </value>
    TValue Value { get; }
}

/// <summary>
///     The strongly typed id.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <seealso cref="Navyblue.Foundation.Domain.IStronglyTypedId&lt;TValue&gt;" />
public abstract record StronglyTypedId<TValue>(TValue Value) : IStronglyTypedId<TValue> where TValue : notnull
{
    /// <summary>
    ///     Converts to the string.
    /// </summary>
    /// <returns>
    ///     A string
    /// </returns>
    public override string ToString() => this.Value.ToString() ?? string.Empty;
}

/// <summary>
///     The guid strongly typed id.
/// </summary>
public abstract record GuidStronglyTypedId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    /// <summary>
    ///     Gets a value indicating whether empty.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    public bool IsEmpty => this.Value == Guid.Empty;
}

/// <summary>
///     The long strongly typed id.
/// </summary>
public abstract record LongStronglyTypedId(long Value) : StronglyTypedId<long>(Value)
{
    /// <summary>
    ///     Gets a value indicating whether empty.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    public bool IsEmpty => this.Value <= 0;
}