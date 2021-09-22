// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Graphs
{
    /// <summary>
    /// Represents a generic graph that can be observed.
    /// </summary>
    /// <typeparam name="TVertex">The vertex type.</typeparam>
    public interface IReadOnlyGraph<TVertex>
    {
        /// <summary>
        /// True, if this is a directed graph.
        /// </summary>
        public bool IsDirected { get; }

        /// <summary>
        /// True, if parallel edges are allowed.
        /// </summary>
        public bool AllowParallelEdges { get; }

        /// <summary>
        /// The vertices of this graph.
        /// </summary>
        public IReadOnlyCollection<TVertex> Vertices { get; }

        /// <summary>
        /// Retrieves all accessible direct neighbor vertex of <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">The vertex to get the neighbors of.</param>
        /// <returns>The direct neighbors of <paramref name="vertex"/>.</returns>
        public IEnumerable<TVertex> GetNeighbors(TVertex vertex);
    }
}
