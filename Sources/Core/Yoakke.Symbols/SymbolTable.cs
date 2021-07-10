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
        /// <inheritdoc/>
        IReadOnlyScope IReadOnlySymbolTable.GlobalScope => this.GlobalScope;

        /// <inheritdoc/>
        public IScope GlobalScope { get; }

        /// <inheritdoc/>
        public IScope CurrentScope { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolTable"/> class.
        /// </summary>
        /// <param name="globalScope">The global scope to use.</param>
        public SymbolTable(IScope globalScope)
        {
            this.GlobalScope = globalScope;
            this.CurrentScope = globalScope;
        }

        /// <inheritdoc/>
        public void PushScope(Func<IScope, IScope> makeScope) => this.PushScope(makeScope(this.CurrentScope));

        /// <inheritdoc/>
        public void PushScope(IScope scope)
        {
            if (scope.ContainingScope != this.CurrentScope)
            {
                throw new ArgumentException(
                    "the given scope does not have the containing scope specified as the current one", nameof(scope));
            }
            this.CurrentScope = scope;
        }

        /// <inheritdoc/>
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
