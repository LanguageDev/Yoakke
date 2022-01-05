// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Text;

namespace Yoakke.Collections.Graphs;

/// <summary>
/// Generic BFS implementation.
/// </summary>
public static class BreadthFirst
{
    /// <summary>
    /// Does a breadth-first search.
    /// </summary>
    /// <typeparam name="TVertex">The vertex type.</typeparam>
    /// <param name="initial">The initial vertex.</param>
    /// <param name="getNeighbors">The function to retrieve the neighbors.</param>
    /// <returns>The sequence of the visited vertices.</returns>
    public static IEnumerable<TVertex> Search<TVertex>(TVertex initial, Func<TVertex, IEnumerable<TVertex>> getNeighbors) =>
        Search(initial, getNeighbors, EqualityComparer<TVertex>.Default);

    /// <summary>
    /// Does a breadth-first search.
    /// </summary>
    /// <typeparam name="TVertex">The vertex type.</typeparam>
    /// <param name="initial">The initial vertex.</param>
    /// <param name="getNeighbors">The function to retrieve the neighbors.</param>
    /// <param name="comparer">The vertex comparer to use.</param>
    /// <returns>The sequence of the visited vertices.</returns>
    public static IEnumerable<TVertex> Search<TVertex>(
        TVertex initial,
        Func<TVertex, IEnumerable<TVertex>> getNeighbors,
        IEqualityComparer<TVertex> comparer)
    {
        var explored = new HashSet<TVertex>(comparer);
        var q = new Queue<TVertex>();
        q.Enqueue(initial);
        while (q.TryDequeue(out var v))
        {
            yield return v;
            foreach (var w in getNeighbors(v))
            {
                if (explored.Add(w)) q.Enqueue(w);
            }
        }
    }
}
