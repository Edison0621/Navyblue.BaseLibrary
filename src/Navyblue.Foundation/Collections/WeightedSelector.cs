// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : WeightedSelector.cs
// Created          : 2026-06-29  11:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:01
// ****************************************************************************************************************************************
// <copyright file="WeightedSelector.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

using Navyblue.Foundation.Primitives;

namespace Navyblue.Foundation.Collections;

/// <summary>
///     The weighted selector.
/// </summary>
/// <typeparam name="T" />
public sealed class WeightedSelector<T>
{
    private readonly List<(T Item, int Weight)> _items = [];

    /// <summary>
    ///     Add and return a weightedselector.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="weight">The weight.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns><![CDATA[WeightedSelector<T>]]></returns>
    public WeightedSelector<T> Add(T item, int weight)
    {
        Guard.GreaterThanZero(weight, nameof(weight));
        this._items.Add((item, weight));
        return this;
    }

    /// <summary>
    /// </summary>
    /// <param name="random">The random.</param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <returns>A T</returns>
    public T Next(Random? random = null)
    {
        if (this._items.Count == 0) throw new InvalidOperationException("No weighted items configured.");
        random ??= Random.Shared;
        int total = this._items.Sum(x => x.Weight);
        int value = random.Next(1, total + 1);
        int current = 0;
        foreach ((T item, int weight) in this._items)
        {
            current += weight;
            if (value <= current) return item;
        }

        return this._items[^1].Item;
    }
}