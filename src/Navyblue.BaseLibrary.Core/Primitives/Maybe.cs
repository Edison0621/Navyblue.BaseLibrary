namespace Navyblue.BaseLibrary.Primitives;

public readonly struct Maybe<T> : IEquatable<Maybe<T>>
{
    private readonly T? _value;

    private Maybe(T value)
    {
        _value = value;
        HasValue = true;
    }

    public bool HasValue { get; }
    public bool HasNoValue => !HasValue;
    public T Value => HasValue ? _value! : throw new InvalidOperationException("Maybe has no value.");

    public static Maybe<T> None => default;
    public static Maybe<T> From(T? value) => value is null ? None : new Maybe<T>(value);
    public static implicit operator Maybe<T>(T? value) => From(value);

    public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
    {
        ArgumentNullException.ThrowIfNull(some);
        ArgumentNullException.ThrowIfNull(none);
        return HasValue ? some(Value) : none();
    }

    public T GetValueOrDefault(T defaultValue) => HasValue ? Value : defaultValue;
    public bool Equals(Maybe<T> other) => HasValue == other.HasValue && EqualityComparer<T?>.Default.Equals(_value, other._value);
    public override bool Equals(object? obj) => obj is Maybe<T> other && Equals(other);
    public override int GetHashCode() => HasValue && _value is not null ? EqualityComparer<T>.Default.GetHashCode(_value) : 0;
    public override string ToString() => HasValue ? Value?.ToString() ?? string.Empty : string.Empty;
}


