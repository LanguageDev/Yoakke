using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Symbols
{
    /// <summary>
    /// A symbol table that contains scopes and symbol associations.
    /// </summary>
    /// <typeparam name="TKey">The key type to associate symbols with.</typeparam>
    public interface IReadOnlySymbolTable<TKey>
    {
        /// <summary>
        /// The global scope in the symbol hierarchy.
        /// </summary>
        public IReadOnlyScope GlobalScope { get; }

        /// <summary>
        /// The associated scopes.
        /// </summary>
        public IReadOnlyDictionary<TKey, IReadOnlyScope> AssociatedScopes { get; }

        /// <summary>
        /// The associated defined symbols.
        /// </summary>
        public IReadOnlyDictionary<TKey, ISymbol> DefinedSymbols { get; }

        /// <summary>
        /// The associated referenced symbols.
        /// </summary>
        public IReadOnlyDictionary<TKey, ISymbol> ReferencedSymbols { get; }
    }
}
