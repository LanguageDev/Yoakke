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
    public interface ISymbolTable : IReadOnlySymbolTable
    {
        /// <summary>
        /// The current scope that we are adding symbols to.
        /// </summary>
        public IScope CurrentScope { get; }

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
