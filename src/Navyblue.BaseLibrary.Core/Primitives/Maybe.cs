// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Maybe.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="Maybe.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.BaseLibrary.Primitives;

/// <summary>
///     The maybe.
/// </summary>
/// <typeparam name="T" />
public readonly struct Maybe<T> : IEquatable<Maybe<T>>
{
    private readonly T? _value;

    private Maybe(T value)
    {
        this._value = value;
        this.HasValue = true;
    }

    /// <summary>
    ///     Gets a value indicating whether has value.
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    ///     Gets a value indicating whether has no value.
    /// </summary>
    public bool HasNoValue => !this.HasValue;

    /// <summary>
    ///     Gets the value.
    /// </summary>
    public T Value => this.HasValue ? this._value! : throw new InvalidOperationException("Maybe has no value.");

    /// <summary>
    ///     Gets the none.
    /// </summary>
    public static Maybe<T> None => default;

    /// <summary>
    ///     Froms and return a maybe.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><![CDATA[Maybe<T>]]></returns>
    public static Maybe<T> From(T? value) => value is null ? None : new Maybe<T>(value);

    /// <summary>
    ///     Performs an implicit conversion to Maybe.
    /// </summary>
    /// <param name="value">The value.</param>
    public static implicit operator Maybe<T>(T? value) => From(value);

    /// <summary>
    /// </summary>
    /// <typeparam name="TResult" />
    /// <param name="some">The some.</param>
    /// <param name="none">The none.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>A <typeparamref name="TResult" /></returns>
    public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
    {
        ArgumentNullException.ThrowIfNull(some);
        ArgumentNullException.ThrowIfNull(none);
        return this.HasValue ? some(this.Value) : none();
    }

    /// <summary>
    ///     Get value or default.
    /// </summary>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>A T</returns>
    public T GetValueOrDefault(T defaultValue) => this.HasValue ? this.Value : defaultValue;

    /// <summary>
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns>A bool</returns>
    public bool Equals(Maybe<T> other) => this.HasValue == other.HasValue && EqualityComparer<T?>.Default.Equals(this._value, other._value);

    /// <summary>
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns>A bool</returns>
    public override bool Equals(object? obj) => obj is Maybe<T> other && this.Equals(other);

    /// <summary>
    ///     Get hash code.
    /// </summary>
    /// <returns>An int</returns>
    public override int GetHashCode() => this.HasValue && this._value is not null ? EqualityComparer<T>.Default.GetHashCode(this._value) : 0;

    /// <summary>
    ///     Converts to the string.
    /// </summary>
    /// <returns>A string</returns>
    public override string ToString() => this.HasValue ? this.Value?.ToString() ?? string.Empty : string.Empty;
}