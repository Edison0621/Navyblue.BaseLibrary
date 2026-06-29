namespace Navyblue.BaseLibrary.Collections;

public sealed class WeightedSelector<T>
{
    private readonly List<(T Item, int Weight)> _items = [];
    public WeightedSelector<T> Add(T item, int weight) { ArgumentOutOfRangeException.ThrowIfNegativeOrZero(weight); _items.Add((item, weight)); return this; }
    public T Next(Random? random = null)
    {
        if (_items.Count == 0) throw new InvalidOperationException("No weighted items configured.");
        random ??= Random.Shared; var total = _items.Sum(x => x.Weight); var value = random.Next(1, total + 1); var current = 0;
        foreach (var (item, weight) in _items) { current += weight; if (value <= current) return item; }
        return _items[^1].Item;
    }
}
