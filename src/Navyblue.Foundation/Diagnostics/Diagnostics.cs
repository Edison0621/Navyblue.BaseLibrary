// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : Diagnostics.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="Diagnostics.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using System.Diagnostics;
using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.Diagnostics;

/// <summary>
///     The correlation context.
/// </summary>
public static class CorrelationContext
{
    private static readonly AsyncLocal<string?> _currentValue = new();

    /// <summary>
    ///     Gets or sets the current.
    /// </summary>
    public static string? Current
    {
        get => _currentValue.Value;
        set => _currentValue.Value = value;
    }

    /// <summary>
    ///     Begins the scope.
    /// </summary>
    /// <param name="correlationId">The correlation id.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <returns>An IDisposable</returns>
    public static IDisposable BeginScope(string correlationId)
    {
        Guard.NotNullOrWhiteSpace(correlationId, nameof(correlationId));
        string? previous = _currentValue.Value;
        _currentValue.Value = correlationId;
        return new Scope(previous);
    }

    #region Nested type: Scope

    private sealed class Scope(string? previous) : IDisposable
    {
        private bool _disposed;

        #region IDisposable Members

        public void Dispose()
        {
            if (this._disposed) return;
            _currentValue.Value = previous;
            this._disposed = true;
        }

        #endregion
    }

    #endregion
}

/// <summary>
///     The operation timer.
/// </summary>
public readonly struct OperationTimer
{
    private readonly long _startTimestamp;
    private OperationTimer(long startTimestamp) => this._startTimestamp = startTimestamp;

    /// <summary>
    ///     Start the new.
    /// </summary>
    /// <returns>An OperationTimer</returns>
    public static OperationTimer StartNew() => new(Stopwatch.GetTimestamp());

    /// <summary>
    ///     Gets the elapsed.
    /// </summary>
    public TimeSpan Elapsed
    {
        get
        {
#if NET7_0_OR_GREATER
            return Stopwatch.GetElapsedTime(this._startTimestamp);
#else
            long elapsedTicks = (long)((Stopwatch.GetTimestamp() - this._startTimestamp) * ((double)TimeSpan.TicksPerSecond / Stopwatch.Frequency));
            return TimeSpan.FromTicks(elapsedTicks);
#endif
        }
    }
}