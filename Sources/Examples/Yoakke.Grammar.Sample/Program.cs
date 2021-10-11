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
    public class GssNode
    {
        public int State { get; init; }

        public IParseTreeNode? Node { get; init; }

        public List<GssNode> Prev { get; init; } = new();
    }

    class Program
    {
        static void Main(string[] args)
        {
            var cfg = ParseGrammar(@"
S -> E
E -> E + E
E -> F
F -> 1
E -> 1
");
            cfg.AugmentStartSymbol();

            var table = LrParsingTable.Lr0(cfg);
            Console.WriteLine(table.ToHtmlTable());
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(table.ToDotDfa());

            while (true)
            {
                var input = Console.ReadLine();
                if (input is null) break;
                GlrParse(table, input);
                // Console.WriteLine(tree.ToDot());
            }
        }

        static void GlrParse(ILrParsingTable table, string input)
        {
            // Input reader
            var inputEnumerator = input.GetEnumerator();
            Terminal NextInput() => inputEnumerator!.MoveNext()
                ? new(inputEnumerator.Current.ToString())
                : Terminal.EndOfInput;

            // The root node of the GSS
            var root = new GssNode { State = 0, Node = new LeafParseTreeNode(Terminal.EndOfInput, "$") };
            // The front nodes of the stack
            var front = new HashSet<GssNode> { root };
            // Last read input
            var term = NextInput();

            void PrintGss()
            {
                Console.WriteLine("graph GSS {");
                Console.WriteLine("  rankdir=RL;");
                // Label each node
                var nodeNames = new Dictionary<GssNode, int>();
                foreach (var n in front!.SelectMany(f => BreadthFirst.Search(f, n => n.Prev)))
                {
                    if (nodeNames.ContainsKey(n)) continue;
                    nodeNames.Add(n, nodeNames.Count);
                }
                // Print nodes
                foreach (var (n, idx) in nodeNames) Console.WriteLine($"  {idx} [label=\"{n.State}: {n.Node?.Symbol}\"]");
                // Print relations
                foreach (var (n, idx) in nodeNames)
                {
                    foreach (var p in n.Prev) Console.WriteLine($"  {idx} -- {nodeNames[p]}");
                }
                Console.WriteLine("}");
            }

            PrintGss();

            while (true)
            {
                // Collect the next front
                var nextFront = new Dictionary<int, GssNode>();

                // For each front node of the GSS we collect all actions needed to be performed
                // We perform all actions on the GSS until a shift
                // This is so all heads are synchronized at the input
                // First let's collect all heads with all operations into a stack
                var remaining = new Stack<(GssNode Head, Action Action)>();
                var initialActions = front.SelectMany(n => table.Action[n.State, term].Select(a => (n, a)));
                foreach (var (n, a) in initialActions) remaining.Push((n, a));

                var anyAccepted = false;

                // Now we perform actions until this constructed stack is emptied
                while (remaining.TryPop(out var top))
                {
                    var (node, action) = top;

                    // If we are shifting, we are done with this node, we just need to push on the next node
                    if (action is Shift shift)
                    {
                        // We need to check, if the shifted state is already present
                        // If so, we can just use the existing one
                        if (nextFront.TryGetValue(shift.State, out var existing))
                        {
                            existing.Prev.Add(node);
                            continue;
                        }
                        // Otherwise we need to create a new front node
                        var newNode = new GssNode
                        {
                            State = shift.State,
                            Node = new LeafParseTreeNode(term, term.Value),
                            Prev = new() { node },
                        };
                        nextFront.Add(shift.State, newNode);
                        // Either way, we are done with this node, a shift happened
                        continue;
                    }

                    // For accept we simply note that an accept has happened and carry over the accepting node
                    if (action is Accept)
                    {
                        anyAccepted = true;
                        nextFront.Add(node.State, node);
                        continue;
                    }

                    // We are dealing with a reduction
                    var reduce = (Reduce)action;
                    // We pop off the reduction arguments
                    // We need to keep track of all of the exposed states we need to deal with, as the stack can branch
                    var nextNodes = new Dictionary<int, (GssNode, List<IParseTreeNode>)>();
                    nextNodes.Add(node.State, (node, new() { node.Node! }));
                    for (var i = 0; i < reduce.Production.Right.Count; ++i)
                    {
                        var nextNextNodes = new Dictionary<int, (GssNode, List<IParseTreeNode>)>();
                        foreach (var (nextNode, nextNodeList) in nextNodes.Values)
                        {
                            foreach (var p in nextNode.Prev)
                            {
                                Debug.Assert(p.Node is not null);
                                nextNextNodes[p.State] = (p, nextNodeList.Append(p.Node!).ToList());
                            }
                        }
                        nextNodes = nextNextNodes;
                    }
                    // Now all these exposed nodes go to some other state, based on the goto table
                    // Since they have not shifted, we are not done with them and all of them require further processing
                    foreach (var (nextNode, nextNodeList) in nextNodes.Values)
                    {
                        var nextState = table.Goto[nextNode.State, reduce.Production.Left]!.Value;
                        // Check, if the next front already has a node with such state
                        if (nextFront.TryGetValue(nextState, out var existing))
                        {
                            // If so, all we need to do is add it
                            existing.Prev.Add(nextNode);
                        }
                        else
                        {
                            // Otherwise we need a new node
                            nextNodeList.Reverse();
                            var newNode = new GssNode
                            {
                                State = nextState,
                                Node = new ProductionParseTreeNode(reduce.Production, nextNodeList),
                                Prev = new() { nextNode },
                            };
                            // This node needs further processing, add it with all possible actions it could need
                            var furtherActions = table.Action[newNode.State, term];
                            foreach (var a in furtherActions) remaining.Push((newNode, a));
                        }
                    }
                }

                // Swap the current front
                front = nextFront.Values.ToHashSet();

                Console.WriteLine();
                Console.WriteLine("========================");
                Console.WriteLine();
                PrintGss();

                // If we had accept actions, terminate
                if (anyAccepted) break;

                // Since the last operation is guaranteed to be a shift for every stack head, we can eat the next input here
                term = NextInput();
            }

            // Let's print all trees
            foreach (var tree in front.Select(n => n.Node))
            {
                Console.WriteLine(tree!.ToDot());
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
