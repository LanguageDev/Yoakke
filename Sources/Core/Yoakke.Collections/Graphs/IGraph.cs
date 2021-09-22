// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Graphs
{
    /// <summary>
    /// Represents a generic graph.
    /// </summary>
    /// <typeparam name="TVertex">The vertex type.</typeparam>
    public interface IGraph<TVertex> : IReadOnlyGraph<TVertex>
    {
        /// <summary>
        /// The vertices of this graph.
        /// </summary>
        public new ICollection<TVertex> Vertices { get; }
    }
}
