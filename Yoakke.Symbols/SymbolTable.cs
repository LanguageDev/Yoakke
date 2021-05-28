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
    public class SymbolTable : ISymbolTable
    {
        public IReadOnlyScope GlobalScope { get; }
        public IScope CurrentScope { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="SymbolTable{TKey}"/>.
        /// </summary>
        /// <param name="globalScope">The global scope to use.</param>
        public SymbolTable(IScope globalScope)
        {
            GlobalScope = globalScope;
            CurrentScope = globalScope;
        }

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
