using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Symbols
{
    /// <summary>
    /// A symbol table that can be used to build the dymbol tree.
    /// </summary>
    /// <typeparam name="TKey">The key type to associate symbols with.</typeparam>
    public interface ISymbolTable<TKey> : IReadOnlySymbolTable<TKey>
    {
        /// <summary>
        /// The current scope that we are adding symbols to.
        /// </summary>
        public IScope CurrentScope { get; }

        /// <summary>
        /// The associated scopes.
        /// </summary>
        new public IReadOnlyDictionary<TKey, IScope> AssociatedScopes { get; }

        /// <summary>
        /// Associates a key with a scope.
        /// </summary>
        /// <param name="key">The key to associate.</param>
        /// <param name="scope">The scope to associate the key with.</param>
        public void AssociateScope(TKey key, IScope scope);

        /// <summary>
        /// Associates a key with the current scope.
        /// </summary>
        /// <param name="key">The key to associate.</param>
        public void AssociateCurrentScope(TKey key);

        /// <summary>
        /// Associates the symbol as being defined with the given key.
        /// </summary>
        /// <param name="key">The key to associate the symbol with.</param>
        /// <param name="symbol">The symbol to add as being defined.</param>
        public void AddDefinedSymbol(TKey key, ISymbol symbol);

        /// <summary>
        /// Associates the symbol as being referred to with the given key.
        /// </summary>
        /// <param name="key">The key to associate the symbol with.</param>
        /// <param name="symbol">The symbol to add as being referred.</param>
        public void AddReferencedSymbol(TKey key, ISymbol symbol);

        /// <summary>
        /// Pushes a new scope as the current scope.
        /// </summary>
        /// <param name="makeScope">The function that receives the current scope and constructs the new one.</param>
        public void PushScope(Func<IScope, IScope> makeScope);

        /// <summary>
        /// Pushes a new scope as the current scope.
        /// </summary>
        /// <param name="scope">The scope to push.</param>
        public void PushScope(IScope scope);

        /// <summary>
        /// Pops off the current scope to be the parent of the current one.
        /// </summary>
        public void PopScope();
    }
}
