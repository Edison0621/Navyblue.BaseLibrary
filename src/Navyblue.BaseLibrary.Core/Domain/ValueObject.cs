namespace Navyblue.BaseLibrary.Domain;

public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        return obj is ValueObject other
            && GetType() == other.GetType()
            && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(1, (current, obj) => HashCode.Combine(current, obj));
    }

    public static bool operator ==(ValueObject? left, ValueObject? right) => Equals(left, right);
    public static bool operator !=(ValueObject? left, ValueObject? right) => !Equals(left, right);
}

public sealed class DateRange : ValueObject
{
    public DateRange(DateTimeOffset start, DateTimeOffset end)
    {
        if (end < start) throw new ArgumentException("End must be greater than or equal to start.", nameof(end));
        Start = start;
        End = end;
    }

    public DateTimeOffset Start { get; }
    public DateTimeOffset End { get; }
    public TimeSpan Duration => End - Start;
    public bool Contains(DateTimeOffset value) => value >= Start && value <= End;
    protected override IEnumerable<object?> GetEqualityComponents() { yield return Start; yield return End; }
}

public sealed class Money : ValueObject
{
    public Money(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency cannot be empty.", nameof(currency));
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = currency.Trim().ToUpperInvariant();
    }

    public decimal Amount { get; }
    public string Currency { get; }
    protected override IEnumerable<object?> GetEqualityComponents() { yield return Amount; yield return Currency; }
    public override string ToString() => $"{Amount:0.00} {Currency}";
}
