using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Symbols
{
    /// <summary>
    /// A default implementation for symbol tables.
    /// </summary>
    /// <typeparam name="TKey">The key type to associate symbols with.</typeparam>
    public class SymbolTable<TKey> : ISymbolTable<TKey> where TKey : notnull
    {
        public IReadOnlyScope GlobalScope { get; }
        public IScope CurrentScope { get; private set; }

        IReadOnlyDictionary<TKey, IReadOnlyScope> IReadOnlySymbolTable<TKey>.AssociatedScopes => associatedScopesReadonly;
        public IReadOnlyDictionary<TKey, IScope> AssociatedScopes => associatedScopes;
        public IReadOnlyDictionary<TKey, ISymbol> DefinedSymbols => definedSymbols;
        public IReadOnlyDictionary<TKey, ISymbol> ReferencedSymbols => referencedSymbols;

        private Dictionary<TKey, IReadOnlyScope> associatedScopesReadonly = new();
        private Dictionary<TKey, IScope> associatedScopes = new();
        private Dictionary<TKey, ISymbol> definedSymbols = new();
        private Dictionary<TKey, ISymbol> referencedSymbols = new();

        /// <summary>
        /// Initializes a new <see cref="SymbolTable{TKey}"/>.
        /// </summary>
        /// <param name="globalScope">The global scope to use.</param>
        public SymbolTable(IScope globalScope)
        {
            GlobalScope = globalScope;
            CurrentScope = globalScope;
        }

        public void AssociateScope(TKey key, IScope scope)
        {
            associatedScopesReadonly.Add(key, scope);
            associatedScopes.Add(key, scope);
        }

        public void AssociateCurrentScope(TKey key) => AssociateScope(key, CurrentScope);
        public void AddDefinedSymbol(TKey key, ISymbol symbol) => definedSymbols.Add(key, symbol);
        public void AddReferencedSymbol(TKey key, ISymbol symbol) => referencedSymbols.Add(key, symbol);

        public void PushScope(Func<IScope, IScope> makeScope) => PushScope(makeScope(CurrentScope));

        public void PushScope(IScope scope)
        {
            if (scope.ContainingScope != CurrentScope)
            {
                throw new ArgumentException(
                    "the given scope does not have the containing scope specified as the current one", nameof(scope));
            }
            CurrentScope = scope;
        }

        public void PopScope()
        {
            if (CurrentScope.ContainingScope is null)
            {
                throw new InvalidOperationException("parent of current scope was null, can't pop");
            }
            CurrentScope = (IScope)CurrentScope.ContainingScope;
        }
    }
}
