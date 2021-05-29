using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Symbols
{
    /// <summary>
    /// A simple scope implementation with a tag type.
    /// </summary>
    /// <typeparam name="TTag">The tag type associated with the scope.</typeparam>
    public class Scope<TTag> : IScope
    {
        /// <summary>
        /// The tag associated with the scope.
        /// </summary>
        public readonly TTag Tag;

        public IReadOnlyScope? ContainingScope { get; }
        public bool IsGlobal => ContainingScope is null;
        public IReadOnlyDictionary<string, ISymbol> Symbols => symbols;

        private Dictionary<string, ISymbol> symbols = new();

        /// <summary>
        /// Initializes a new <see cref="Scope{TTag}"/>.
        /// </summary>
        /// <param name="parent">The parent (containing) scope of this one.</param>
        /// <param name="tag">The tag of this scope.</param>
        public Scope(IReadOnlyScope? parent = null, TTag tag = default)
        {
            ContainingScope = parent;
            Tag = tag;
        }

        /// <summary>
        /// Initializes a new <see cref="Scope{TTag}"/> without a containing scope.
        /// </summary>
        /// <param name="tag">The tag of this scope.</param>
        public Scope(TTag tag)
            : this(null, tag)
        {
        }

        public IEnumerable<ISymbol> GetReachableSymbols() => ContainingScope is null
            ? Symbols.Values
            : Symbols.Values.Concat(ContainingScope.GetReachableSymbols());

        public ISymbol? ReferenceSymbol(string name) => ReferenceSymbol(name, _ => true);

        public ISymbol? ReferenceSymbol(string name, Predicate<IReadOnlyScope> canCross)
        {
            if (Symbols.TryGetValue(name, out var symbol)) return symbol;
            if (canCross(this) && ContainingScope is not null) return ContainingScope.ReferenceSymbol(name, canCross);
            return null;
        }

        public void DefineSymbol(ISymbol symbol)
        {
            if (symbol.Scope != this)
            {
                throw new ArgumentException("the scope of the symbol is not set to this", nameof(symbol));
            }
            symbols[symbol.Name] = symbol;
        }
    }
}
