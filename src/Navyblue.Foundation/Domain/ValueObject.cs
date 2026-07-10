// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ValueObject.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="ValueObject.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Domain;

/// <summary>
///     The value object.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    ///     Gets the equality components.
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    ///     Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns>
    ///     A bool
    /// </returns>
    public override bool Equals(object? obj)
    {
        return obj is ValueObject other
               && this.GetType() == other.GetType()
               && this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    ///     Get hash code.
    /// </summary>
    /// <returns>
    ///     An int
    /// </returns>
    public override int GetHashCode()
    {
        return this.GetEqualityComponents()
            .Aggregate(1, (current, obj) => HashCode.Combine(current, obj));
    }

    /// <summary>
    ///     Implements the == operator.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    ///     A bool
    /// </returns>
    public static bool operator ==(ValueObject? left, ValueObject? right) => Equals(left, right);

    /// <summary>
    ///     Implements the != operator.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>
    ///     A bool
    /// </returns>
    public static bool operator !=(ValueObject? left, ValueObject? right) => !Equals(left, right);
}

/// <summary>
///     The date range.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Domain.ValueObject" />
public sealed class DateRange : ValueObject
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DateRange" /> class.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <exception cref="ArgumentException">End must be greater than or equal to start. - end</exception>
    public DateRange(DateTimeOffset start, DateTimeOffset end)
    {
        if (end < start) throw new ArgumentException("End must be greater than or equal to start.", nameof(end));
        this.Start = start;
        this.End = end;
    }

    /// <summary>
    ///     Gets the start.
    /// </summary>
    /// <value>
    ///     The start.
    /// </value>
    public DateTimeOffset Start { get; }

    /// <summary>
    ///     Gets the end.
    /// </summary>
    /// <value>
    ///     The end.
    /// </value>
    public DateTimeOffset End { get; }

    /// <summary>
    ///     Gets the duration.
    /// </summary>
    /// <value>
    ///     The duration.
    /// </value>
    public TimeSpan Duration => this.End - this.Start;

    /// <summary>
    ///     Determines whether this instance contains the object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     A bool
    /// </returns>
    public bool Contains(DateTimeOffset value) => value >= this.Start && value <= this.End;

    /// <summary>
    ///     Gets the equality components.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return this.Start;
        yield return this.End;
    }
}

/// <summary>
///     The money.
/// </summary>
/// <seealso cref="Navyblue.Foundation.Domain.ValueObject" />
public sealed class Money : ValueObject
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Money" /> class.
    /// </summary>
    /// <param name="amount">The amount.</param>
    /// <param name="currency">The currency.</param>
    /// <exception cref="ArgumentException">Currency cannot be empty. - currency</exception>
    public Money(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency cannot be empty.", nameof(currency));
        this.Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        this.Currency = currency.Trim().ToUpperInvariant();
    }

    /// <summary>
    ///     Gets the amount.
    /// </summary>
    /// <value>
    ///     The amount.
    /// </value>
    public decimal Amount { get; }

    /// <summary>
    ///     Gets the currency.
    /// </summary>
    /// <value>
    ///     The currency.
    /// </value>
    public string Currency { get; }

    /// <summary>
    ///     Gets the equality components.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return this.Amount;
        yield return this.Currency;
    }

    /// <summary>
    ///     Converts to the string.
    /// </summary>
    /// <returns>
    ///     A string
    /// </returns>
    public override string ToString() => $"{this.Amount:0.00} {this.Currency}";
}