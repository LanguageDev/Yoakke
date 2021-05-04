using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections.FiniteAutomata
{
    /// <summary>
    /// Represents a deterministic finite automata that has only one way to step on a symbol.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type the automata steps on.</typeparam>
    public interface IDeterministicFiniteAutomata<TSymbol> : IFiniteAutomata<TSymbol>
    {
        /// <summary>
        /// Gets the state to continue on for a symbol.
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="on">The symbol to transition on.</param>
        /// <returns>The state to transition to, or null if there's no transition.</returns>
        public State GetTransition(State from, TSymbol on);

        /// <summary>
        /// Minifies this deterministic finite automata.
        /// </summary>
        /// <returns>The minified version of this automata.</returns>
        public IDeterministicFiniteAutomata<TSymbol> Minify();
    }
}
