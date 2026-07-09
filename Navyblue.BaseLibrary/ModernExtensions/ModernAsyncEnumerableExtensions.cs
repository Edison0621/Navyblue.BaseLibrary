// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernAsyncEnumerableExtensions.cs
// Created          : 2026-06-30  13:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-06-30  14:49
// ****************************************************************************************************************************************
// <copyright file="ModernAsyncEnumerableExtensions.cs" company="">
//     Copyright (c) 2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
using System.Runtime.CompilerServices;

namespace Navyblue.BaseLibrary.Extensions;

public static class ModernAsyncEnumerableExtensions
{
    public static async ValueTask<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        List<T> result = new List<T>();
        await foreach (T item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            result.Add(item);
        }

        return result;
    }

    public static async ValueTask<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
    {
        List<T> result = await source.ToListAsync(cancellationToken).ConfigureAwait(false);
        return result.ToArray();
    }

    public static async ValueTask ForEachAsync<T>(this IAsyncEnumerable<T> source, Func<T, CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (action == null) throw new ArgumentNullException(nameof(action));
        await foreach (T item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            await action(item, cancellationToken).ConfigureAwait(false);
        }
    }

    public static async IAsyncEnumerable<T> WhereAsync<T>(this IAsyncEnumerable<T> source, Func<T, bool> predicate, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        await foreach (T item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (predicate(item)) yield return item;
        }
    }

    public static async IAsyncEnumerable<TResult> SelectAsync<T, TResult>(this IAsyncEnumerable<T> source, Func<T, TResult> selector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        await foreach (T item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            yield return selector(item);
        }
    }

    public static async ValueTask<T?> FirstOrDefaultAsync<T>(this IAsyncEnumerable<T> source, Func<T, bool>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        await foreach (T item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (predicate is null || predicate(item)) return item;
        }

        return default;
    }
}