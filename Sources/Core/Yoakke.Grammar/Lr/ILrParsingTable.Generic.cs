// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Grammar.Cfg;

namespace Yoakke.Grammar.Lr
{
    /// <summary>
    /// Represents an LR(k) parsing table.
    /// </summary>
    /// <typeparam name="TItem">The LR item type.</typeparam>
    public interface ILrParsingTable<TItem> : ILrParsingTable
        where TItem : ILrItem
    {
        /// <summary>
        /// The state allocator.
        /// </summary>
        public LrStateAllocator<TItem> StateAllocator { get; }

        /// <summary>
        /// Calculates the closure of some LR item.
        /// </summary>
        /// <param name="item">The item to calculate the closure of.</param>
        /// <returns>The LR closure of <paramref name="item"/>.</returns>
        public ISet<TItem> Closure(TItem item);

        /// <summary>
        /// Calculates the closure of an LR item set.
        /// </summary>
        /// <param name="set">The item set to calculate the closure of.</param>
        /// <returns>The LR closure of <paramref name="set"/>.</returns>
        public ISet<TItem> Closure(IEnumerable<TItem> set);

        /// <summary>
        /// Checks, if a given item is a kernel item.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True, if <paramref name="item"/> is a kernel item.</returns>
        public bool IsKernel(TItem item);
    }
}
