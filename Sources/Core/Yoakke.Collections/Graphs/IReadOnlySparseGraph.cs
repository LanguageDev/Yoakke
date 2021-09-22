// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Graphs
{
    /// <summary>
    /// Represents a sparse graph where each edge is stored separately.
    /// </summary>
    /// <typeparam name="TVertex">The vertex type.</typeparam>
    /// <typeparam name="TEdge">The associated edge data type.</typeparam>
    public interface IReadOnlySparseGraph<TVertex, TEdge> : IReadOnlyGraph<TVertex>
    {
        /// <summary>
        /// The collection of edges.
        /// </summary>
        public IReadOnlyCollection<Edge<TVertex, TEdge>> Edges { get; }

        /// <summary>
        /// Retrieves a sequence of edges that go out from <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">The vertex to get the edges from.</param>
        /// <returns>The sequence of edges going from <paramref name="vertex"/>.</returns>
        public IEnumerable<Edge<TVertex, TEdge>> GetEdgesFrom(TVertex vertex);
    }
}
