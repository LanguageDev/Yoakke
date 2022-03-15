// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections;

/// <summary>
/// Graph search algorithms.
/// </summary>
public static class GraphSearch
{
    /// <summary>
    /// Neighbor accessor functionality.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    public interface INeighborSelector<TNode>
    {
        /// <summary>
        /// Retrieves all neighbors of a given node.
        /// </summary>
        /// <param name="node">The node to get the neighbors of.</param>
        /// <returns>The neighbors of <paramref name="node"/>.</returns>
        public IEnumerable<TNode> GetNeighbors(TNode node);
    }

    /// <summary>
    /// Retrieves all reachable nodes in the graph.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="root">The root to start the search from.</param>
    /// <param name="comparer">The comparer for the nodes to use.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The set of reachable nodes from <paramref name="root"/>.</returns>
    public static HashSet<TNode> AllReachable<TNode, TNodeAdapter>(
        TNode root,
        IEqualityComparer<TNode>? comparer,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : INeighborSelector<TNode> =>
        AllReachable(
            roots: EnumerableExtensions.Singleton(root),
            comparer: comparer,
            nodeAdapter: nodeAdapter);

    /// <summary>
    /// Retrieves all reachable nodes in the graph.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="roots">The roots to start the search from.</param>
    /// <param name="comparer">The comparer for the nodes to use.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The set of reachable nodes from <paramref name="roots"/>.</returns>
    public static HashSet<TNode> AllReachable<TNode, TNodeAdapter>(
        IEnumerable<TNode> roots,
        IEqualityComparer<TNode>? comparer,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : INeighborSelector<TNode>
    {
        var h = new HashSet<TNode>(comparer);
        // Force iteration
        foreach (var _ in DepthFirstImpl(roots, h, nodeAdapter));
        return h;
    }

    /// <summary>
    /// Searches through a graph using BFS.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="root">The root to start the search from.</param>
    /// <param name="comparer">The comparer for the nodes to use.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The sequence of nodes in a breadth-first order.</returns>
    public static IEnumerable<TNode> BreadthFirst<TNode, TNodeAdapter>(
        TNode root,
        IEqualityComparer<TNode>? comparer,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : INeighborSelector<TNode> =>
        BreadthFirst(
            roots: EnumerableExtensions.Singleton(root),
            comparer: comparer,
            nodeAdapter: nodeAdapter);

    /// <summary>
    /// Searches through a graph using BFS.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="roots">The roots to start the search from.</param>
    /// <param name="comparer">The comparer for the nodes to use.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The sequence of nodes in a breadth-first order.</returns>
    public static IEnumerable<TNode> BreadthFirst<TNode, TNodeAdapter>(
        IEnumerable<TNode> roots,
        IEqualityComparer<TNode>? comparer,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : INeighborSelector<TNode> =>
        BreadthFirstImpl(roots, new(comparer), nodeAdapter);

    /// <summary>
    /// Searches through a graph using DFS.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="root">The root to start the search from.</param>
    /// <param name="comparer">The comparer for the nodes to use.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The sequence of nodes in a depth-first order.</returns>
    public static IEnumerable<TNode> DepthFirst<TNode, TNodeAdapter>(
        TNode root,
        IEqualityComparer<TNode>? comparer,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : INeighborSelector<TNode> =>
        DepthFirst(
            roots: EnumerableExtensions.Singleton(root),
            comparer: comparer,
            nodeAdapter: nodeAdapter);

    /// <summary>
    /// Searches through a graph using DFS.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TNodeAdapter">The node adapter type.</typeparam>
    /// <param name="roots">The roots to start the search from.</param>
    /// <param name="comparer">The comparer for the nodes to use.</param>
    /// <param name="nodeAdapter">The node adapter.</param>
    /// <returns>The sequence of nodes in a depth-first order.</returns>
    public static IEnumerable<TNode> DepthFirst<TNode, TNodeAdapter>(
        IEnumerable<TNode> roots,
        IEqualityComparer<TNode>? comparer,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : INeighborSelector<TNode> =>
        DepthFirstImpl(roots, new(comparer), nodeAdapter);

    private static IEnumerable<TNode> BreadthFirstImpl<TNode, TNodeAdapter>(
        IEnumerable<TNode> roots,
        HashSet<TNode> explored,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : INeighborSelector<TNode>
    {
        var q = new Queue<TNode>();
        foreach (var root in roots)
        {
            if (explored.Add(root)) q.Enqueue(root);
        }
        while (q.Count > 0)
        {
            var v = q.Dequeue();
            yield return v;
            foreach (var w in nodeAdapter.GetNeighbors(v))
            {
                if (explored.Add(w)) q.Enqueue(w);
            }
        }
    }

    private static IEnumerable<TNode> DepthFirstImpl<TNode, TNodeAdapter>(
        IEnumerable<TNode> roots,
        HashSet<TNode> explored,
        TNodeAdapter nodeAdapter)
        where TNodeAdapter : INeighborSelector<TNode>
    {
        var s = new Stack<TNode>();
        foreach (var root in roots)
        {
            if (explored.Add(root)) s.Push(root);
        }
        while (s.Count > 0)
        {
            var v = s.Pop();
            yield return v;
            foreach (var w in nodeAdapter.GetNeighbors(v))
            {
                if (explored.Add(w)) s.Push(w);
            }
        }
    }
}
