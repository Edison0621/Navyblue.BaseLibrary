// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernTreeExtensions.cs
// Created          : 2026-06-30  17:06
// 
// Last Modified By : kitt-nostalgic(jstsmaxx@gmail.com)
// Last Modified On : 2026-07-09  14:00
// ****************************************************************************************************************************************
// <copyright file="ModernTreeExtensions.cs" company="">
//     Copyright ©  2011-2026. All rights reserved.
// </copyright>
// ****************************************************************************************************************************************

#nullable enable
namespace Navyblue.BaseLibrary.Extensions;

/// <summary>
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class TreeNode<T>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TreeNode{T}" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public TreeNode(T value)
    {
        this.Value = value;
    }

    /// <summary>
    ///     Gets the value.
    /// </summary>
    /// <value>
    ///     The value.
    /// </value>
    public T Value { get; }

    /// <summary>
    ///     Gets the children.
    /// </summary>
    /// <value>
    ///     The children.
    /// </value>
    public List<TreeNode<T>> Children { get; } = new();
}

/// <summary>
/// </summary>
public static class ModernTreeExtensions
{
    /// <summary>
    ///     Converts to tree.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="idSelector">The identifier selector.</param>
    /// <param name="parentIdSelector">The parent identifier selector.</param>
    /// <param name="rootParentId">The root parent identifier.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    ///     source
    ///     or
    ///     idSelector
    ///     or
    ///     parentIdSelector
    /// </exception>
    public static List<TreeNode<T>> ToTree<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> idSelector,
        Func<T, TKey?> parentIdSelector,
        TKey? rootParentId = null,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : struct
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (idSelector == null) throw new ArgumentNullException(nameof(idSelector));
        if (parentIdSelector == null) throw new ArgumentNullException(nameof(parentIdSelector));

        comparer ??= EqualityComparer<TKey>.Default;
        List<T> items = source.ToList();
        Dictionary<TKey, TreeNode<T>> nodes = items.ToDictionary(idSelector, item => new TreeNode<T>(item), comparer);
        List<TreeNode<T>> roots = new();

        foreach (T item in items)
        {
            TreeNode<T> node = nodes[idSelector(item)];
            TKey? parentId = parentIdSelector(item);
            if (!parentId.HasValue || (rootParentId.HasValue && comparer.Equals(parentId.Value, rootParentId.Value)))
            {
                roots.Add(node);
                continue;
            }

            if (nodes.TryGetValue(parentId.Value, out TreeNode<T>? parent))
            {
                parent.Children.Add(node);
            }
            else
            {
                roots.Add(node);
            }
        }

        return roots;
    }

    /// <summary>
    ///     Converts to tree.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="idSelector">The identifier selector.</param>
    /// <param name="parentIdSelector">The parent identifier selector.</param>
    /// <param name="rootParentId">The root parent identifier.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">
    ///     source
    ///     or
    ///     idSelector
    ///     or
    ///     parentIdSelector
    /// </exception>
    public static List<TreeNode<T>> ToTree<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> idSelector,
        Func<T, TKey?> parentIdSelector,
        TKey? rootParentId = default,
        IEqualityComparer<TKey>? comparer = null)
        where TKey : class
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (idSelector == null) throw new ArgumentNullException(nameof(idSelector));
        if (parentIdSelector == null) throw new ArgumentNullException(nameof(parentIdSelector));

        comparer ??= EqualityComparer<TKey>.Default;
        List<T> items = source.ToList();
        Dictionary<TKey, TreeNode<T>> nodes = items.ToDictionary(idSelector, item => new TreeNode<T>(item), comparer);
        List<TreeNode<T>> roots = new();

        foreach (T item in items)
        {
            TreeNode<T> node = nodes[idSelector(item)];
            TKey? parentId = parentIdSelector(item);
            if (parentId == null || (rootParentId != null && comparer.Equals(parentId, rootParentId)))
            {
                roots.Add(node);
                continue;
            }

            if (nodes.TryGetValue(parentId, out TreeNode<T>? parent))
            {
                parent.Children.Add(node);
            }
            else
            {
                roots.Add(node);
            }
        }

        return roots;
    }

    /// <summary>
    ///     Flattens the specified nodes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="nodes">The nodes.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">nodes</exception>
    public static IEnumerable<TreeNode<T>> Flatten<T>(this IEnumerable<TreeNode<T>> nodes)
    {
        if (nodes == null) throw new ArgumentNullException(nameof(nodes));
        foreach (TreeNode<T> node in nodes)
        {
            yield return node;
            foreach (TreeNode<T> child in node.Children.Flatten())
            {
                yield return child;
            }
        }
    }

    /// <summary>
    ///     Flattens the values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="nodes">The nodes.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">nodes</exception>
    public static IEnumerable<T> FlattenValues<T>(this IEnumerable<TreeNode<T>> nodes)
    {
        if (nodes == null) throw new ArgumentNullException(nameof(nodes));
        return nodes.Flatten().Select(node => node.Value);
    }
}