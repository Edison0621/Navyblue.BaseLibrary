namespace Navyblue.BaseLibrary.Primitives;

public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}

public class Result
{
    protected Result(bool succeeded, Error error)
    {
        if (succeeded && error != Error.None) throw new InvalidOperationException("Successful result cannot contain an error.");
        if (!succeeded && error == Error.None) throw new InvalidOperationException("Failed result must contain an error.");
        Succeeded = succeeded;
        Error = error;
    }

    public bool Succeeded { get; }
    public bool Failed => !Succeeded;
    public Error Error { get; }
    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(value, true, Error.None);
    public static Result<T> Failure<T>(Error error) => new(default, false, error);
}

public sealed class Result<T> : Result
{
    internal Result(T? value, bool succeeded, Error error) : base(succeeded, error) => Value = value;
    public T? Value { get; }
}

public sealed record PagedResult<T>(IReadOnlyList<T> Items, long Total, int PageIndex, int PageSize)
{
    public long TotalPages => PageSize <= 0 ? 0 : (long)Math.Ceiling(Total / (double)PageSize);
}

public static class Guard
{
    public static T NotNull<T>(T? value, string paramName) where T : class => value ?? throw new ArgumentNullException(paramName);
    public static string NotNullOrWhiteSpace(string? value, string paramName) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Value cannot be empty.", paramName) : value;
    public static int GreaterThanZero(int value, string paramName) => value <= 0 ? throw new ArgumentOutOfRangeException(paramName) : value;
}

public static class SequentialGuid
{
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
