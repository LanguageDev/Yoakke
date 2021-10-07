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
        /// The states allocated in the allocator.
        /// </summary>
        public IReadOnlyCollection<int> States => this.stateToItemSet.Keys;

        /// <summary>
        /// The item sets allocated in the allocator.
        /// </summary>
        public IReadOnlyCollection<ISet<TItem>> ItemSets => this.itemSetToState.Keys;

        /// <summary>
        /// Retrieves the allocated state for a given <paramref name="itemSet"/>.
        /// </summary>
        /// <param name="itemSet">The item set to get the allocated state for.</param>
        /// <returns>The allocated state of <paramref name="itemSet"/>.</returns>
        public int this[ISet<TItem> itemSet] => this.itemSetToState[itemSet];

        /// <summary>
        /// Retrieves the item set for a given allocated <paramref name="state"/>.
        /// </summary>
        /// <param name="state">The state to get the item set for.</param>
        /// <returns>The item set associated to <paramref name="state"/>.</returns>
        public ISet<TItem> this[int state] => this.stateToItemSet[state];

        private readonly Dictionary<ISet<TItem>, int> itemSetToState = new(SetEqualityComparer<TItem>.Default);
        private readonly Dictionary<int, ISet<TItem>> stateToItemSet = new();

        /// <summary>
        /// Allocates a state for the given item set.
        /// </summary>
        /// <param name="itemSet">The item set to allocate a state for.</param>
        /// <param name="state">The state gets written here, that corresponds to <paramref name="itemSet"/>.</param>
        /// <returns>True, if the state was new, false otherwise.</returns>
        public bool Allocate(ISet<TItem> itemSet, out int state)
        {
            if (this.itemSetToState.TryGetValue(itemSet, out state)) return false;
            state = this.itemSetToState.Count;
            this.itemSetToState.Add(itemSet, state);
            this.stateToItemSet.Add(state, itemSet);
            return true;
        }
    }
}
