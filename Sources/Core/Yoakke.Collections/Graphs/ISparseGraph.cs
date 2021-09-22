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
    public interface ISparseGraph<TVertex, TEdge> : IReadOnlySparseGraph<TVertex, TEdge>, IGraph<TVertex>
    {
        /// <summary>
        /// The collection of edges.
        /// </summary>
        public new ICollection<Edge<TVertex, TEdge>> Edges { get; }
    }
}
