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
    public interface IReadOnlySymbolTable
    {
        /// <summary>
        /// The global scope in the symbol hierarchy.
        /// </summary>
        public IReadOnlyScope GlobalScope { get; }
    }
}
