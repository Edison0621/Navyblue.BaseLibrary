// ****************************************************************************************************************************************
// Project          : Navyblue.BaseLibrary
// File             : ModernTreeExtensions.cs
// Created          : 2026-06-30
// ****************************************************************************************************************************************

#nullable enable
namespace Navyblue.BaseLibrary.Extensions;

public sealed class TreeNode<T>
{
    public TreeNode(T value)
    {
        Value = value;
    }

    public T Value { get; }

    public List<TreeNode<T>> Children { get; } = new();
}

public static class ModernTreeExtensions
{
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

    public static IEnumerable<T> FlattenValues<T>(this IEnumerable<TreeNode<T>> nodes)
    {
        if (nodes == null) throw new ArgumentNullException(nameof(nodes));
        return nodes.Flatten().Select(node => node.Value);
    }
}