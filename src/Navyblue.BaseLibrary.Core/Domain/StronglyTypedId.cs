namespace Navyblue.BaseLibrary.Domain;

public interface IStronglyTypedId<out TValue> where TValue : notnull
{
    TValue Value { get; }
}

public abstract record StronglyTypedId<TValue>(TValue Value) : IStronglyTypedId<TValue> where TValue : notnull
{
    public override string ToString() => Value.ToString() ?? string.Empty;
}

public abstract record GuidStronglyTypedId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public bool IsEmpty => Value == Guid.Empty;
}

public abstract record LongStronglyTypedId(long Value) : StronglyTypedId<long>(Value)
{
    public bool IsEmpty => Value <= 0;
}
