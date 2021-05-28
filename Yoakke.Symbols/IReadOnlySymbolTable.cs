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
        /// Retrieves the symbol that has a definition entry for the associated key.
        /// </summary>
        /// <param name="key">The key to get the defined symbol for.</param>
        /// <returns>The defined symbol for the key, or null if there was none.</returns>
        public ISymbol? GetDeclaringSymbol(TKey key);

        /// <summary>
        /// Retrieves the symbol that has a reference entry for the associated key.
        /// </summary>
        /// <param name="key">The key to get the referred symbol for.</param>
        /// <returns>The referred symbol for the key, or null if there was none.</returns>
        public ISymbol? GetReferringSymbol(TKey key);
    }
}
