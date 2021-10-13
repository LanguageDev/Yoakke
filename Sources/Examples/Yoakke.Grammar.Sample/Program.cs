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

        public override IEnumerable<Vertex> Prev => this.PrevMap.Values;

        public Dictionary<Symbol, SymbolVertex> PrevMap { get; } = new Dictionary<Symbol, SymbolVertex>();

        public StateVertex()
        {
        }

        public StateVertex(SymbolVertex prev, int state)
        {
            this.PrevMap.Add(prev.Symbol, prev);
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

    class VertexLayer
    {
        public IDictionary<int, StateVertex> StateVertices { get; } = new Dictionary<int, StateVertex>();

        public void Clear()
        {
            this.StateVertices.Clear();
        }

        public StateVertex? Push(StateVertex prevState, Symbol symbol, int state)
        {
            // We can only share a symbol vertex, if we share the state vertex
            // So first we check if we share a state vertex
            if (this.StateVertices.TryGetValue(state, out var nextState))
            {
                // There is a state vertex we can share, there's a possibility we can share the symbol vertex too
                if (nextState.PrevMap.TryGetValue(symbol, out var nextSymbol))
                {
                    // Everything is cached, nothing to do, just connect them up
                    nextSymbol.Prev.Add(prevState);
                }
                else
                {
                    // The next symbol does not exist yet
                    nextSymbol = new(prevState, symbol);
                    nextState.PrevMap.Add(symbol, nextSymbol);
                }
                // Anyway, the state exists
                return null;
            }
            else
            {
                // The state does not exist yet, we need to add it
                // In this case, we don't share the symbols either
                var nextSymbol = new SymbolVertex(prevState, symbol);
                nextSymbol.Prev.Add(prevState);
                nextState = new(nextSymbol, state);
                this.StateVertices.Add(state, nextState);
                return nextState;
            }
        }
    }

    class GraphStructuredStack
    {
        private readonly ILrParsingTable table;

        // Layer caches
        private readonly VertexLayer reduceLayer = new();
        private readonly VertexLayer shiftLayer = new();

        // Temporary vertex heads
        private readonly HashSet<StateVertex> oldHeads = new();

        // Action tracking
        private readonly Stack<(StateVertex Vertex, Reduce Reduce)> remainingReduces = new();
        private readonly Stack<(StateVertex Vertex, Shift Shift)> remainingShifts = new();

        // Current terminal
        private Terminal currentTerminal = Terminal.NotInGrammar;

        public GraphStructuredStack(ILrParsingTable table)
        {
            this.table = table;
            // Initial state
            this.shiftLayer.StateVertices.Add(0, new());
        }

        public string ToDot()
        {
            var result = new StringBuilder();
            result.AppendLine("graph GSS {");
            result.AppendLine("  rankdir=RL;");

            var allHeads = this.oldHeads.Concat(this.shiftLayer.StateVertices.Values).ToHashSet();

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

            result.Replace("->", "→");

            return result.ToString();
        }

        public void Feed(Terminal terminal)
        {
            // If there are remaining actions to perform, feeding another terminal is illegal
            if (this.remainingReduces.Count > 0 || this.remainingShifts.Count > 0) throw new InvalidOperationException("Not all actions are performed yet");

            // We add the old heads
            this.oldHeads.Clear();
            foreach (var h in this.shiftLayer.StateVertices.Values) this.oldHeads.Add(h);

            // Clear cache
            this.shiftLayer.Clear();
            this.reduceLayer.Clear();

            // We store the terminal
            this.currentTerminal = terminal;

            // Then we push each action for each head
            foreach (var head in this.oldHeads) this.PushActions(head);
        }

        public bool Step()
        {
            // We always reduce everywhere first
            if (this.remainingReduces.TryPop(out var r))
            {
                this.Reduce(r.Vertex, r.Reduce);
                return true;
            }
            // We do the shifts all at once
            var result = this.remainingShifts.Count > 0;
            while (this.remainingShifts.TryPop(out var s)) this.Shift(s.Vertex, s.Shift);
            // If there are no more shifts, we are done
            return result;
        }

        private bool IsActive(StateVertex vertex) =>
               this.remainingReduces.Any(r => ReferenceEquals(r.Vertex, vertex))
            || this.remainingShifts.Any(s => ReferenceEquals(s.Vertex, vertex));

        private void Reduce(StateVertex vertex, Reduce reduce)
        {
            // If the vertex is not a vertex anymore, we remove it from the old heads
            if (!this.IsActive(vertex))
            {
                this.oldHeads.Remove(vertex);
                this.reduceLayer.StateVertices.Remove(vertex.State);
            }
            // Now we need to pop off |b| amount of symbol vertices for an X -> b reduction
            var newRoots = new HashSet<StateVertex> { vertex };
            for (var i = 0; i < reduce.Production.Right.Count; ++i)
            {

                newRoots = Pop(newRoots).ToHashSet();
            }
            // We have all the new roots, all of them get a symbol and state pushed on
            foreach (var root in newRoots)
            {
                // Check what state we result in
                var stateGoto = this.table.Goto[root.State, reduce.Production.Left];
                // If nothing, we terminate this branch
                if (stateGoto is null) continue;
                // Otherwise we push on the symbol and the state
                var pushedVertex = this.reduceLayer.Push(root, reduce.Production.Left, stateGoto.Value);
                // If the vertex existed, we don't do anything
                if (pushedVertex is null) continue;
                // For debugging we push on this node as an old head, will be deleted by a shift anyway
                this.oldHeads.Add(pushedVertex);
                // Now we add all actions that can be performed on this new state for the current terminal for further processing
                this.PushActions(pushedVertex);
            }
        }

        private void Shift(StateVertex vertex, Shift shift)
        {
            // The vertex is surely out of the heads now
            this.oldHeads.Remove(vertex);
            // Now we try to push on the symbol and next state
            this.shiftLayer.Push(vertex, this.currentTerminal, shift.State);
        }

        private static IEnumerable<StateVertex> Pop(IEnumerable<StateVertex> vertices) =>
            vertices.SelectMany(v => v.PrevMap.Values.SelectMany(u => u.Prev));

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
