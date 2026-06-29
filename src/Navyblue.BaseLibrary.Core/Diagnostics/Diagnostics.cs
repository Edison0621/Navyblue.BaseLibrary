using System.Diagnostics;

namespace Navyblue.BaseLibrary.Diagnostics;

public static class CorrelationContext
{
    private static readonly AsyncLocal<string?> CurrentValue = new();
    public static string? Current { get => CurrentValue.Value; set => CurrentValue.Value = value; }
    public static IDisposable BeginScope(string correlationId) { ArgumentException.ThrowIfNullOrWhiteSpace(correlationId); var previous = CurrentValue.Value; CurrentValue.Value = correlationId; return new Scope(previous); }
    private sealed class Scope(string? previous) : IDisposable { private bool _disposed; public void Dispose() { if (_disposed) return; CurrentValue.Value = previous; _disposed = true; } }
}

public readonly struct OperationTimer
{
    private readonly long _startTimestamp;
    private OperationTimer(long startTimestamp) => _startTimestamp = startTimestamp;
    public static OperationTimer StartNew() => new(Stopwatch.GetTimestamp());
    public TimeSpan Elapsed => Stopwatch.GetElapsedTime(_startTimestamp);
}
