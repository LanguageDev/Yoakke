using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Yoakke.Collections;
using Yoakke.Collections.Graphs;
using Yoakke.Collections.Values;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.Lr;
using Yoakke.Grammar.Lr.Lalr;
using Yoakke.Grammar.Lr.Lr0;
using Yoakke.Grammar.ParseTree;
using Action = Yoakke.Grammar.Lr.Action;

namespace Yoakke.Grammar.Sample
{
    abstract class Vertex
    {
        public abstract IEnumerable<Vertex> Prev { get; }
    }

    class StateVertex : Vertex
    {
        public int State { get; }

        public override ISet<SymbolVertex> Prev { get; } = new HashSet<SymbolVertex>();

        public StateVertex()
        {
        }

        public StateVertex(SymbolVertex prev, int state)
        {
            this.Prev.Add(prev);
            this.State = state;
        }
    }

    class SymbolVertex : Vertex
    {
        public Symbol Symbol { get; }

        public override ISet<StateVertex> Prev { get; } = new HashSet<StateVertex>();

        public SymbolVertex(StateVertex prev, Symbol symbol)
        {
            this.Prev.Add(prev);
            this.Symbol = symbol;
        }
    }

    class GraphStructuredStack
    {
        private readonly ILrParsingTable table;

        // Head tracking
        private readonly Dictionary<int, StateVertex> heads = new() { { 0, new() } };

        // Temporary vertex heads
        private readonly Dictionary<Symbol, SymbolVertex> symbolHeads = new();
        private readonly HashSet<StateVertex> oldHeads = new();

        // Action tracking
        private readonly Stack<(StateVertex Vertex, Reduce Reduce)> remainingReduces = new();
        private readonly Stack<(StateVertex Vertex, Shift Shift)> remainingShifts = new();

        // Current terminal
        private Terminal currentTerminal = Terminal.NotInGrammar;

        public GraphStructuredStack(ILrParsingTable table)
        {
            this.table = table;
        }

        public string ToDot()
        {
            var result = new StringBuilder();
            result.AppendLine("graph GSS {");
            result.AppendLine("  rankdir=RL;");

            var allHeads = this.oldHeads.Concat(this.heads.Values).ToHashSet();

            // We number each vertex
            var vertexNames = new Dictionary<Vertex, int>();
            foreach (var v in allHeads.Cast<Vertex>().SelectMany(n => BreadthFirst.Search(n, v => v.Prev)))
            {
                if (vertexNames.ContainsKey(v)) continue;
                vertexNames.Add(v, vertexNames.Count);
            }

            // For the old and current heads we create a subgraph to align them
            result.AppendLine("  subgraph Heads {");
            result.AppendLine($"    {{rank=same {string.Join(" ", allHeads.Select(h => vertexNames[h]))}}}");
            foreach (var h in allHeads)
            {
                var reductionsOnHead = this.remainingReduces
                    .Where(r => ReferenceEquals(r.Vertex, h))
                    .Select(r => r.Reduce);
                var shiftsOnHead = this.remainingShifts
                    .Where(s => ReferenceEquals(s.Vertex, h))
                    .Select(s => s.Shift);
                var opsOnHead = reductionsOnHead.Cast<Action>().Concat(shiftsOnHead);
                result.AppendLine($"    {vertexNames[h]}[label=\"{h.State}\", shape=circle, xlabel=\"{string.Join(@"\l", opsOnHead)}\"];");
            }
            result.AppendLine("  }");

            // Define all other nodes
            foreach (var vertex in vertexNames.Keys.Except(allHeads))
            {
                result.Append($"  {vertexNames[vertex]}[");
                if (vertex is StateVertex state)
                {
                    result.Append($"label=\"{state.State}\", shape=circle");
                }
                else
                {
                    var symbol = (SymbolVertex)vertex;
                    result.Append($"label=\"{symbol.Symbol}\", shape=square");
                }
                result.AppendLine("]");
            }

            // Connections
            foreach (var v1 in vertexNames.Keys)
            {
                foreach (var v2 in v1.Prev) result.AppendLine($"  {vertexNames[v1]} -- {vertexNames[v2]}");
            }

            result.Append("}");
            return result.ToString();
        }

        public void Feed(Terminal terminal)
        {
            // If there are remaining actions to perform, feeding another terminal is illegal
            if (this.remainingReduces.Count > 0 || this.remainingShifts.Count > 0) throw new InvalidOperationException("Not all actions are performed yet");

            // Symbol heads are cached per-feed
            this.symbolHeads.Clear();

            // We add the old heads
            this.oldHeads.Clear();
            foreach (var h in this.heads.Values) this.oldHeads.Add(h);

            // We store the terminal
            this.currentTerminal = terminal;

            // Then we push each action for each head
            foreach (var head in this.heads.Values) this.PushActions(head);

            // Now we clear the current heads, they will be filled up
            this.heads.Clear();
        }

        public bool Step()
        {
            // We always reduce everywhere first
            if (this.remainingReduces.TryPop(out var r))
            {
                this.Reduce(r.Vertex, r.Reduce);
                return true;
            }
            // Then shift
            if (this.remainingShifts.TryPop(out var s)) this.Shift(s.Vertex, s.Shift);
            // If there are no more shifts, we are done
            return this.remainingShifts.Count > 0;
        }

        private bool IsActive(StateVertex vertex) =>
               this.remainingReduces.Any(r => ReferenceEquals(r.Vertex, vertex))
            || this.remainingShifts.Any(s => ReferenceEquals(s.Vertex, vertex));

        private void Reduce(StateVertex vertex, Reduce reduce)
        {
            // If the vertex is not a vertex anymore, we remove it from the old heads
            if (!this.IsActive(vertex)) this.oldHeads.Remove(vertex);
            // Now we need to pop off |b| amount of symbol vertices for an X -> b reduction
            var newRoots = new HashSet<StateVertex> { vertex };
            for (var i = 0; i < reduce.Production.Right.Count; ++i) newRoots = Pop(newRoots).ToHashSet();
            // We have all the new roots, all of them get a symbol and state pushed on
            foreach (var root in newRoots)
            {
                // Check what state we result in
                var stateGoto = this.table.Goto[root.State, reduce.Production.Left];
                // If nothing, we terminate this branch
                if (stateGoto is null) continue;
                // Otherwise we push on the symbol and the state
                var nextSymbol = new SymbolVertex(root, reduce.Production.Left);
                var nextState = new StateVertex(nextSymbol, stateGoto.Value);
                // For debugging we push on this node as an old head, will be deleted by a shift anyway
                this.oldHeads.Add(nextState);
                // Now we add all actions that can be performed on this new state for the current terminal for further processing
                this.PushActions(nextState);
            }
        }

        private void Shift(StateVertex vertex, Shift shift)
        {
            // The vertex is surely out of the heads now
            this.oldHeads.Remove(vertex);
            // Now we try to push on the symbol and next state
            // First the symbol
            if (this.symbolHeads.TryGetValue(this.currentTerminal, out var nextSymbolVertex))
            {
                // The next symbol is already contained, just connect it up
                nextSymbolVertex.Prev.Add(vertex);
            }
            else
            {
                // Does not exist yet
                nextSymbolVertex = new(vertex, this.currentTerminal);
                this.symbolHeads.Add(this.currentTerminal, nextSymbolVertex);
            }
            // Now the state
            if (this.heads.TryGetValue(shift.State, out var nextStateVertex))
            {
                // The next state is already contained, just connect it up
                nextStateVertex.Prev.Add(nextSymbolVertex);
            }
            else
            {
                // Does not exist yet
                nextStateVertex = new(nextSymbolVertex, shift.State);
                this.heads.Add(shift.State, nextStateVertex);
            }
        }

        private static IEnumerable<StateVertex> Pop(IEnumerable<StateVertex> vertices) =>
            vertices.SelectMany(v => v.Prev.SelectMany(u => u.Prev));

        private void PushActions(StateVertex vertex)
        {
            var actions = this.table.Action[vertex.State, this.currentTerminal];
            foreach (var action in actions)
            {
                if (action is Shift s) this.remainingShifts.Push((vertex, s));
                else if (action is Reduce r) this.remainingReduces.Push((vertex, r));
                // NOTE: else it's an accept
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var cfg = ParseGrammar(@"
S -> NP VP
S -> S PP
NP -> noun
NP -> determiner noun
NP -> NP PP
PP -> preposition NP
VP -> verb NP
");
            cfg.AugmentStartSymbol();

            var table = LrParsingTable.Lalr(cfg);
            Console.WriteLine("Parsing table:");
            Console.WriteLine(table.ToHtmlTable());
            Console.WriteLine("=========================");

            while (true)
            {
                var input = Console.ReadLine();
                if (input is null) break;
                GlrParse(table, input);
            }
        }

        static void GlrParse(ILrParsingTable table, string text)
        {
            var nouns = new[] { "I", "man", "telescope", "bed", "apartment", "park" };
            var determiners = new[] { "a", "the" };
            var prepositions = new[] { "on", "in", "with" };
            var verbs = new[] { "saw" };

            Terminal ToTerm(string word)
            {
                if (nouns!.Contains(word)) return new("noun");
                if (determiners!.Contains(word)) return new("determiner");
                if (prepositions!.Contains(word)) return new("preposition");
                if (verbs!.Contains(word)) return new("verb");
                throw new ArgumentOutOfRangeException();
            }

            var words = text
                .Split(' ')
                .Select(t => t.Trim())
                .Where(t => t.Length > 0)
                .ToList();
            var terminals = words
                .Select(ToTerm)
                .Append(Terminal.EndOfInput)
                .ToList();
            words.Add("$");

            var gss = new GraphStructuredStack(table);
            Console.WriteLine("=========================");
            Console.WriteLine(gss.ToDot());
            Console.WriteLine("=========================");
            for (var i = 0; i < words.Count; ++i)
            {
                Console.WriteLine($"processing {words[i]}");
                gss.Feed(terminals[i]);
                Console.WriteLine("=========================");
                Console.WriteLine(gss.ToDot());
                Console.WriteLine("=========================");
                while (gss.Step())
                {
                    Console.WriteLine("=========================");
                    Console.WriteLine(gss.ToDot());
                    Console.WriteLine("=========================");
                }
                Console.WriteLine("=========================");
                Console.WriteLine(gss.ToDot());
                Console.WriteLine("=========================");
            }
        }

        static IParseTreeNode Parse(ILrParsingTable table, string input)
        {
            var parser = new LrParser(table);
            foreach (var action in parser.Parse(input, c => new(c.ToString()!)))
            {
                // First we do some state printing
                Console.WriteLine($"State stack: {string.Join(" ", parser.StateStack.Reverse())}");
                Console.WriteLine($"Result stack: {string.Join(" ", parser.ResultStack.Reverse().Select(r => r.Symbol))}");
                Console.WriteLine(action);
            }

            return parser.ResultStack.First();
        }

        static ContextFreeGrammar ParseGrammar(string text)
        {
            var result = new ContextFreeGrammar();
            var tokens = text
                .Split(' ', '\r', '\n')
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();
            var arrowPositions = tokens.IndicesOf("->").ToList();
            var ruleNames = arrowPositions
                .Select(pos => tokens[pos - 1])
                .ToHashSet();
            for (var i = 0; i < arrowPositions.Count; ++i)
            {
                var arrowPosition = arrowPositions[i];
                var productionName = tokens[arrowPosition - 1];
                var productionsUntil = tokens.Count;
                if (i < arrowPositions.Count - 1) productionsUntil = arrowPositions[i + 1] - 1;
                var productions = tokens.GetRange(arrowPosition + 1, productionsUntil - (arrowPosition + 1));
                while (productions.Count > 0)
                {
                    var end = productions.IndexOf("|");
                    if (end == -1) end = productions.Count;
                    else productions.RemoveAt(end);
                    List<Symbol> prodSymbols = new();
                    if (productions[0] != "ε")
                    {
                        prodSymbols = productions
                            .Take(end)
                            .Select(t => ruleNames.Contains(t)
                                ? (Symbol)new Nonterminal(t)
                                : new Terminal(t))
                            .ToList();
                    }
                    result.Productions.Add(new(new(productionName), prodSymbols.ToValue()));
                    productions.RemoveRange(0, end);
                }
            }
            return result;
        }
    }
}
