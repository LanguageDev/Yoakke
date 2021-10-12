using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public int Level { get; }

        public HashSet<Vertex> Prev { get; }

        protected Vertex(int level, HashSet<Vertex> prev)
        {
            this.Level = level;
            this.Prev = prev;
        }
    }

    class StateVertex : Vertex
    {
        public int State { get; }

        public StateVertex()
            : base(0, new())
        {
            this.State = 0;
        }

        public StateVertex(int level, int state, Vertex prev)
            : base(level, new() { prev })
        {
            this.State = state;
        }
    }

    class SymbolVertex : Vertex
    {
        public Symbol Symbol { get; }

        public SymbolVertex(int level, Symbol symbol, Vertex prev)
            : base(level, new() { prev })
        {
            this.Symbol = symbol;
        }
    }

    class GraphStructuredStack
    {
        private readonly List<Dictionary<object, Vertex>> levels;

        public GraphStructuredStack()
        {
            this.levels = new() { new() { { 0, new StateVertex() } } };
        }

        public void DebugPrint()
        {
            Console.WriteLine("graph GSS {");
            Console.WriteLine("  rankdir=RL;");

            // First we name the nodes
            var nodeNames = new Dictionary<Vertex, int>();
            foreach (var node in this.levels[^1].Values.SelectMany(v => BreadthFirst.Search(v, n => n.Prev)))
            {
                if (nodeNames.ContainsKey(node)) continue;
                nodeNames.Add(node, nodeNames.Count);
            }

            // We print all levels in subgraphs
            for (var l = 0; l < this.levels.Count; ++l)
            {
                Console.WriteLine($"  subgraph level_{l} {{");
                Console.WriteLine($"    {{rank=same {string.Join(" ", this.levels[l].Values.Where(v => nodeNames.ContainsKey(v)).Select(v => nodeNames[v]))}}}");
                foreach (var node in this.levels[l].Values)
                {
                    if (!nodeNames.ContainsKey(node)) continue;
                    var idx = nodeNames[node];
                    if (node is StateVertex stateVertex)
                    {
                        Console.WriteLine($"    {idx}[label=\"{stateVertex.State}\", shape=circle];");
                    }
                    else
                    {
                        var symbolVertex = (SymbolVertex)node;
                        Console.WriteLine($"    {idx}[label=\"{symbolVertex.Symbol}\", shape=square];");
                    }
                }
                Console.WriteLine("  }");
            }

            // Finally print connections
            foreach (var (node, idx) in nodeNames)
            {
                foreach (var prev in node.Prev) Console.WriteLine($"  {idx} -- {nodeNames[prev]}");
            }

            Console.WriteLine("}");
        }

        public bool Step(ILrParsingTable table, Terminal input)
        {
            // We keep track of the unhandled heads in a stack
            // Unhandled heads are all heads that didn't have a shift or accept action performed as the last action
            // Initially the stack will contain the current heads with all the possible actions they can perform
            var stk = new Stack<(StateVertex Head, Action Action)>();
            var initialActions = this.levels[^1].Values
                .Cast<StateVertex>()
                .SelectMany(h => table.Action[h.State, input].Select(a => (Head: h, Action: a)));
            foreach (var item in initialActions) stk.Push(item);
            var lastLevel = this.levels[^1].Values.First().Level;

            // As long as there is an unhandled head, handle it
            while (stk.TryPop(out var top))
            {
                var (head, action) = top;

                // Fow now we immediately terminate on an accept
                if (action is Accept) return true;

                // If this is a shift, we just do the shifting and we are done with this node
                // Nothing derives onto the stack, as this head can be considered in-sync with the input
                if (action is Shift shift)
                {
                    this.Push(lastLevel, head, input, shift.State);
                    continue;
                }

                // This is a reduce, which is a bit more complicated
                var reduce = (Reduce)action;
                // We first need to pop off enough symbol stack elements for the reduction
                // The pop might happen on a joint vertex, so the pop will result in multiple possible nodes
                // We need to consider all of them at once and collect all elements for the reduction
                var poppedElements = new HashSet<StateVertex> { head };
                for (var i = 0; i < reduce.Production.Right.Count; ++i) poppedElements = Pop(poppedElements);
                // Now that we popped off the proper amount of elements, we also need to push onto all these roots
                // all the different actions that can be performed on them, not unlike for the initialization of this stack
                foreach (var root in poppedElements)
                {
                    // We push on a new state
                    var nextState = table.Goto[root.State, reduce.Production.Left];
                    if (nextState is null) continue;
                    var nextRoot = this.Push(root.Level, root, reduce.Production.Left, nextState.Value);
                    // We add all possible actions on this state to be processed, until they all terminate in a shift too
                    foreach (var nextAction in table.Action[nextState.Value, input]) stk.Push((nextRoot, nextAction));
                }
            }

            return false;
        }

        private StateVertex Push(int levelOffset, StateVertex stateVertex, Symbol symbol, int state)
        {
            // Make sure we have enough levels
            if (this.levels.Count == levelOffset + 1)
            {
                this.levels.Add(new());
                this.levels.Add(new());
            }
            // First push the symbol vertex
            SymbolVertex symbolVertex;
            var symbolLevel = levelOffset + 1;
            if (this.levels[symbolLevel].TryGetValue(symbol, out var existingSymbol))
            {
                symbolVertex = (SymbolVertex)existingSymbol;
                symbolVertex.Prev.Add(stateVertex);
            }
            else
            {
                symbolVertex = new(symbolLevel, symbol, stateVertex);
                this.levels[symbolLevel].Add(symbol, symbolVertex);
            }
            // Then push the state vertex
            StateVertex newStateVertex;
            var newStateLevel = levelOffset + 2;
            if (this.levels[newStateLevel].TryGetValue(state, out var existingState))
            {
                newStateVertex = (StateVertex)existingState;
                existingState.Prev.Add(symbolVertex);
            }
            else
            {
                newStateVertex = new(newStateLevel, state, symbolVertex);
                this.levels[newStateLevel].Add(state, newStateVertex);
            }
            return newStateVertex;
        }

        private static HashSet<StateVertex> Pop(HashSet<StateVertex> vertices)
        {
            var result = new HashSet<StateVertex>();
            // We keep unhandled nodes on a stack
            var stk = new Stack<Vertex>();
            // Initially we push every previous node of the current node
            foreach (var item in vertices.SelectMany(v => v.Prev)) stk.Push(item);
            // While there's an unprocessed node, take it out
            while (stk.TryPop(out var node))
            {
                // If this node is a symbol vertex, the previous items all belong in the result and we are done
                if (node is SymbolVertex symbolVertex)
                {
                    // this.levels[node.Level].Remove(symbolVertex.Symbol);
                    foreach (var p in symbolVertex.Prev) result.Add((StateVertex)p);
                    continue;
                }
                // Otherwise this is a state vertex and we need to go further back
                var stateVertex = (StateVertex)node;
                // this.levels[node.Level].Remove(stateVertex.State);
                foreach (var p in stateVertex.Prev) stk.Push(p);
            }
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var cfg = ParseGrammar(@"
S -> NP VP
S -> S PP
NP -> NOUN
NP -> DETERMINER NOUN
NP -> NP PP
PP -> PREPOSITION NP
VP -> VERB NP
NOUN -> I | man | telescope | bed | apartment | park
DETERMINER -> a | the
PREPOSITION -> on | in | with
VERB -> saw
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
            // Console.WriteLine("=========================");
            // Console.WriteLine($"Parsing {text}");
            var words = text
                .Split(' ')
                .Select(t => t.Trim())
                .Where(t => t.Length > 0)
                .Select(t => new Terminal(t))
                .Append(Terminal.EndOfInput)
                .ToList();
            var gss = new GraphStructuredStack();
            // Console.WriteLine("=========================");
            // Console.WriteLine("Initial GSS:");
            // gss.DebugPrint();
            for (var i = 0; i < words.Count; ++i)
            {
                Console.WriteLine("=========================");
                Console.WriteLine($"Processing {words[i]}");
                Console.WriteLine("=========================");
                Console.WriteLine("Current GSS:");
                gss.Step(table, words[i]);
                gss.DebugPrint();
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
