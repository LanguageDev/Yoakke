// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Symbols
{
    /// <summary>
    /// A default implementation for symbol tables.
    /// </summary>
    public class SymbolTable : ISymbolTable
    {
        IReadOnlyScope IReadOnlySymbolTable.GlobalScope => this.GlobalScope;

        public IScope GlobalScope { get; }

        public IScope CurrentScope { get; set; }

        /// <summary>
        /// Initializes a new <see cref="SymbolTable{TKey}"/>.
        /// </summary>
        /// <param name="globalScope">The global scope to use.</param>
        public SymbolTable(IScope globalScope)
        {
            this.GlobalScope = globalScope;
            this.CurrentScope = globalScope;
        }

        public void PushScope(Func<IScope, IScope> makeScope) => this.PushScope(makeScope(this.CurrentScope));

        public void PushScope(IScope scope)
        {
            if (scope.ContainingScope != this.CurrentScope)
            {
                throw new ArgumentException(
                    "the given scope does not have the containing scope specified as the current one", nameof(scope));
            }
            this.CurrentScope = scope;
        }

        public void PopScope()
        {
            if (this.CurrentScope.ContainingScope is null)
            {
                throw new InvalidOperationException("parent of current scope was null, can't pop");
            }
            this.CurrentScope = (IScope)this.CurrentScope.ContainingScope;
        }
    }
}
