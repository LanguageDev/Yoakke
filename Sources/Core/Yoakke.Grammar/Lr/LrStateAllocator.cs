// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections;

namespace Yoakke.Grammar.Lr
{
    /// <summary>
    /// A simple allocator to allocate LR states based on item sets.
    /// </summary>
    /// <typeparam name="TItem">The LR item type.</typeparam>
    public class LrStateAllocator<TItem>
        where TItem : ILrItem
    {
        /// <summary>
        /// The number of states allocated.
        /// </summary>
        public int StateCount => this.itemSets.Count;

        /// <summary>
        /// The item sets in the allocator.
        /// </summary>
        public IEnumerable<KeyValuePair<ISet<TItem>, int>> ItemSets => this.itemSets;

        private readonly Dictionary<ISet<TItem>, int> itemSets = new(SetEqualityComparer<TItem>.Default);

        /// <summary>
        /// Allocates a state for the given item set.
        /// </summary>
        /// <param name="itemSet">The item set to allocate a state for.</param>
        /// <param name="state">The state gets written here, that corresponds to <paramref name="itemSet"/>.</param>
        /// <returns>True, if the state was new, false otherwise.</returns>
        public bool Allocate(ISet<TItem> itemSet, out int state)
        {
            if (this.itemSets.TryGetValue(itemSet, out state)) return false;
            state = this.itemSets.Count;
            this.itemSets.Add(itemSet, state);
            return true;
        }
    }
}
