using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Collections;
using Yoakke.Grammar.Cfg;

namespace Yoakke.Grammar.Lr
{
    /// <summary>
    /// A table that contains the actions and state transitions for an LR parser.
    /// </summary>
    public sealed class LrParsingTable
    {
        private Dictionary<ISet<Lr0Item>, int> itemSets = new(SetEqualityComparer<Lr0Item>.Default);
        private Dictionary<int, Dictionary<Terminal, HashSet<Action>>> action = new();
        private Dictionary<int, Dictionary<Symbol, int>> goTo = new();

        /// <inheritdoc/>
        public override string ToString()
        {
            var allTerminalsInAction = this.action.Values.SelectMany(d => d.Keys).Distinct().ToList();
            var allSymbolsInGoto = this.goTo.Values.SelectMany(d => d.Keys).Distinct().ToList();

            var sb = new StringBuilder();

            // Start of table
            sb.Append(@"\begin{tabular}{|c|");
            for (var i = 0; i < allTerminalsInAction.Count; ++i) sb.Append("|c");
            sb.Append('|');
            for (var i = 0; i < allSymbolsInGoto.Count; ++i) sb.Append("|c");
            sb.Append('|');
            sb.AppendLine("}");

            // Header with Action and Goto
            sb
                .Append("  ")
                .Append(@$"\multicolumn{{1}}{{c}}{{}} & ")
                .Append($@"\multicolumn{{{allTerminalsInAction.Count}}}{{c}}{{Action}} &")
                .AppendLine($@"\multicolumn{{{allSymbolsInGoto.Count}}}{{c}}{{Goto}} \\");
            sb.AppendLine(@"  \hline");

            // Header with state, terminals and symbols
            sb.Append("  State");
            foreach (var t in allTerminalsInAction) sb.Append($" & {t}");
            foreach (var t in allSymbolsInGoto) sb.Append($" & {t}");
            sb
                .AppendLine(@" \\")
                .AppendLine(@"  \hline");

            for (var i = 0; i < this.itemSets.Count; ++i)
            {
                sb.AppendLine(@"  \hline");
                sb.Append($"  {i}");
                foreach (var t in allTerminalsInAction)
                {
                    sb.Append(" & ");
                    if (this.action.TryGetValue(i, out var onMap) && onMap.TryGetValue(t, out var actions))
                    {
                        if (actions.Count == 1)
                        {
                            sb.Append(actions.First());
                        }
                        else
                        {
                            sb.Append(@"\begin{tabular}{c} ");
                            sb.Append(string.Join(@" \\ ", actions));
                            sb.Append(@" \end{tabular}");
                        }
                    }
                }
                foreach (var t in allSymbolsInGoto)
                {
                    sb.Append(" & ");
                    if (this.goTo.TryGetValue(i, out var onMap) && onMap.TryGetValue(t, out var to)) sb.Append(to);
                }
                sb.AppendLine(@" \\");
            }

            // End of table
            sb.AppendLine(@"  \hline");
            sb.Append(@"\end{tabular}");

            // Some escapes
            sb
                .Replace("$", @"\$")
                .Replace("->", @"\rightarrow");

            return sb.ToString();
        }

        /// <summary>
        /// Allocates a state for a given item set.
        /// </summary>
        /// <param name="itemSet">The item set to allocate the state for.</param>
        /// <param name="idx">The allocated state index gets written here.</param>
        /// <returns>True, if the item set is unique and had no allocated state before.</returns>
        public bool AllocateState(ISet<Lr0Item> itemSet, out int idx)
        {
            if (this.itemSets.TryGetValue(itemSet, out idx)) return false;
            idx = this.itemSets.Count;
            this.itemSets.Add(itemSet, idx);
            return true;
        }

        /// <summary>
        /// Adds an action to the parser table.
        /// </summary>
        /// <param name="state">The state to add the action to.</param>
        /// <param name="term">The terminal to add the action to.</param>
        /// <param name="action">The action to perform.</param>
        public void AddAction(int state, Terminal term, Action action)
        {
            if (!this.action.TryGetValue(state, out var termDict))
            {
                termDict = new();
                this.action.Add(state, termDict);
            }
            if (!termDict.TryGetValue(term, out var actionList))
            {
                actionList = new();
                termDict.Add(term, actionList);
            }
            actionList.Add(action);
        }

        /// <summary>
        /// Adds a goto entry to the parser table.
        /// </summary>
        /// <param name="fromState">The source state to transition from.</param>
        /// <param name="symbol">The symbol to perform the transition on.</param>
        /// <param name="toState">The state to transition to.</param>
        public void AddGoto(int fromState, Symbol symbol, int toState)
        {
            if (!this.goTo.TryGetValue(fromState, out var on))
            {
                on = new();
                this.goTo.Add(fromState, on);
            }
            on.Add(symbol, toState);
        }
    }
}
