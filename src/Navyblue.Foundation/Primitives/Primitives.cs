// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Primitives.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-10  19:06
// ****************************************************************************************************************************************
// <copyright file="Primitives.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

namespace Navyblue.Foundation.Primitives;

/// <summary>
///     The error.
/// </summary>
public sealed record Error(string Code, string Message)
{
    /// <summary>
    ///     The none.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty);
}

/// <summary>
///     The result.
/// </summary>
public class Result
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Result" /> class.
    /// </summary>
    /// <param name="succeeded">if set to <c>true</c> [succeeded].</param>
    /// <param name="error">The error.</param>
    /// <exception cref="InvalidOperationException">
    ///     Successful result cannot contain an error.
    ///     or
    ///     Failed result must contain an error.
    /// </exception>
    protected Result(bool succeeded, Error error)
    {
        switch (succeeded)
        {
            case true when error != Error.None:
                throw new InvalidOperationException("Successful result cannot contain an error.");
            case false when error == Error.None:
                throw new InvalidOperationException("Failed result must contain an error.");
            default:
                this.Succeeded = succeeded;
                this.Error = error;
                break;
        }
    }

    /// <summary>
    ///     Gets a value indicating whether succeeded.
    /// </summary>
    /// <value>
    ///     <c>true</c> if succeeded; otherwise, <c>false</c>.
    /// </value>
    public bool Succeeded { get; }

    /// <summary>
    ///     Gets a value indicating whether failed.
    /// </summary>
    /// <value>
    ///     <c>true</c> if failed; otherwise, <c>false</c>.
    /// </value>
    public bool Failed => !this.Succeeded;

    /// <summary>
    ///     Gets the error.
    /// </summary>
    /// <value>
    ///     The error.
    /// </value>
    public Error Error { get; }

    /// <summary>
    ///     Successes this instance.
    /// </summary>
    /// <returns>
    ///     A Result
    /// </returns>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    ///     Failures the specified error.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>
    ///     A Result
    /// </returns>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    ///     Successes and return a result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     <![CDATA[Result<T>]]>
    /// </returns>
    public static Result<T> Success<T>(T value) => new(value, true, Error.None);

    /// <summary>
    ///     Failures and return a result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="error">The error.</param>
    /// <returns>
    ///     <![CDATA[Result<T>]]>
    /// </returns>
    public static Result<T> Failure<T>(Error error) => new(default, false, error);
}

/// <summary>
///     The result.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class Result<T> : Result
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Result{T}" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="succeeded">if set to <c>true</c> [succeeded].</param>
    /// <param name="error">The error.</param>
    internal Result(T? value, bool succeeded, Error error) : base(succeeded, error) => this.Value = value;

    /// <summary>
    ///     Gets the value.
    /// </summary>
    /// <value>
    ///     The value.
    /// </value>
    public T? Value { get; }
}

/// <summary>
///     The guard.
/// </summary>
public static class Guard
{
    /// <summary>
    ///     Nots the null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="paramName">The param name.</param>
    /// <returns>
    ///     A <typeparamref name="T" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static T NotNull<T>(T? value, string paramName) where T : class => value ?? throw new ArgumentNullException(paramName);

    /// <summary>
    ///     Nots null or white space.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="paramName">The param name.</param>
    /// <returns>
    ///     A string
    /// </returns>
    /// <exception cref="ArgumentException">Value cannot be empty.</exception>
    public static string NotNullOrWhiteSpace(string? value, string paramName) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Value cannot be empty.", paramName) : value;

    /// <summary>
    ///     Greaters than zero.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="paramName">The param name.</param>
    /// <returns>
    ///     An int
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static int GreaterThanZero(int value, string paramName) => value <= 0 ? throw new ArgumentOutOfRangeException(paramName) : value;
}

/// <summary>
///     The sequential guid.
/// </summary>
public static class SequentialGuid
{
    /// <summary>
    ///     Creates this instance.
    /// </summary>
    /// <returns>
    ///     A Guid
    /// </returns>
    public static Guid Create()
    {
        Span<byte> guidBytes = stackalloc byte[16];
        Guid.NewGuid().TryWriteBytes(guidBytes);
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Span<byte> timeBytes = stackalloc byte[8];
        BitConverter.TryWriteBytes(timeBytes, timestamp);
        if (BitConverter.IsLittleEndian) timeBytes.Reverse();
        timeBytes[^6..].CopyTo(guidBytes[^6..]);
        return new Guid(guidBytes);
    }
}