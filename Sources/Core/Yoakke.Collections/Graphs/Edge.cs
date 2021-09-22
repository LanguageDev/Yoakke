// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Graphs
{
    /// <summary>
    /// Represents a single edge in a graph.
    /// </summary>
    /// <typeparam name="TVertex">The vertex type.</typeparam>
    /// <typeparam name="TEdge">The associated edge data type.</typeparam>
    public readonly struct Edge<TVertex, TEdge>
    {
        /// <summary>
        /// The source vertex.
        /// </summary>
        public TVertex Source { get; }

        /// <summary>
        /// The edge value.
        /// </summary>
        public TEdge Value { get; }

        /// <summary>
        /// The destination vertex.
        /// </summary>
        public TVertex Destination { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge{TVertex, TEdge}"/> struct.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="value">The edge value.</param>
        /// <param name="destination">The destination vertex.</param>
        public Edge(TVertex source, TEdge value, TVertex destination)
        {
            this.Source = source;
            this.Value = value;
            this.Destination = destination;
        }
    }
}
