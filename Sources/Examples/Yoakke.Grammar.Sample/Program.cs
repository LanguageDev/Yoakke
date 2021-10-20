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
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = ParseGrammar(@"
S -> decl_list
decl_list -> decl_list decl
decl_list -> decl
decl_list -> ε

decl -> function_decl
function_decl -> Function Name ( arg_list ) { stmt_list }
arg_list -> arg_list , Name
arg_list -> Name
arg_list -> ε

stmt_list -> stmt_list stmt
stmt_list -> stmt
stmt_list -> ε

stmt -> expr

expr -> expr + expr
expr -> expr * expr
expr -> expr ( expr_list )
expr -> Name
expr -> 1

expr_list -> expr_list , expr
expr_list -> expr
expr_list -> ε
");
            cfg.AugmentStartSymbol();

            var table = LrParsingTable.Lalr(cfg);
            // Console.WriteLine(table.ToHtmlTable());

            while (true)
            {
                var input = Console.ReadLine();
                if (input is null) break;
                GlrParse(() => new GraphStructuredStack(table), input);
            }
        }

        static void GlrParse(Func<INondetStack> makeStack, string text)
        {
            Terminal ToTerm(string word) => new(word);

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

            var treeSource = new IncrementalTreeSource(terminals);

        begin:

            var stack = makeStack();

            Console.WriteLine("=========================");
            Console.WriteLine(stack.ToDot());
            Console.WriteLine("=========================");
            while (!treeSource.IsEnd)
            {
                // Console.WriteLine($"processing {words[i]}");
                stack.Feed(treeSource.Next(stack.CurrentState));
                Console.WriteLine("=========================");
                Console.WriteLine(stack.ToDot());
                Console.WriteLine("=========================");
                while (stack.Step())
                {
                    Console.WriteLine("=========================");
                    Console.WriteLine(stack.ToDot());
                    Console.WriteLine("=========================");
                }
            }

            Console.WriteLine("Nodes:");
            foreach (var ast in stack.Trees)
            {
                Console.WriteLine("=========================");
                Console.WriteLine(ToDot(CloneIncrementalTree(ast)));
                Console.WriteLine("=========================");
            }

            var parts = Console.ReadLine()!.Split(";");
            var start = int.Parse(parts[0].Trim());
            var length = int.Parse(parts[1].Trim());
            var inserted = parts[2]
                .Trim()
                .Split(' ')
                .Select(t => t.Trim())
                .Where(t => t.Length > 0)
                .Select(ToTerm)
                .ToList();

            treeSource.Reset(stack.Trees.Count() == 1 ? stack.Trees.First() : null);
            treeSource.MakeEdit(start, length, inserted);

            Console.WriteLine("Nodes:");
            foreach (var ast in stack.Trees)
            {
                Console.WriteLine("=========================");
                Console.WriteLine(ToDot(CloneIncrementalTree(ast)));
                Console.WriteLine("=========================");
            }

            goto begin;
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

        static IIncrementalTreeNode CloneIncrementalTree(IIncrementalTreeNode node)
        {
            if (node is LeafIncrementalTreeNode leaf) return new LeafIncrementalTreeNode(leaf.Terminal);
            var prod = (ProductionIncrementalTreeNode)node;
            return new ProductionIncrementalTreeNode(prod.Production, prod.ParserState, prod.Children.Select(CloneIncrementalTree).ToList())
            {
                IsReusable = prod.IsReusable,
            };
        }

        static IParseTreeNode CloneTree(IIncrementalTreeNode node)
        {
            if (node is LeafIncrementalTreeNode leaf) return new LeafParseTreeNode(leaf.Terminal, leaf.Terminal);
            var prod = (ProductionIncrementalTreeNode)node;
            return new ProductionParseTreeNode(prod.Production, prod.Children.Select(CloneTree).ToList());
        }

        static string ToDot(IIncrementalTreeNode node)
        {
            var result = new StringBuilder();
            result.AppendLine("graph parse_tree {");

            // We assign each node an ID
            var nodeIds = new Dictionary<IIncrementalTreeNode, int>(ReferenceEqualityComparer.Instance);
            foreach (var n in BreadthFirst.Search(node, n => ((IIncrementalTreeNode)n).Children, ReferenceEqualityComparer.Instance)) nodeIds.Add((IIncrementalTreeNode)n, nodeIds.Count);

            // Define each node with the label
            foreach (var (n, id) in nodeIds)
            {
                result.Append($"  {id}[label=\"");

                if (n is LeafIncrementalTreeNode leaf)
                {
                    result.Append($"{n.Symbol}[{leaf.Terminal}]");
                }
                else
                {
                    var p = (ProductionIncrementalTreeNode)n;
                    result.Append(n.Symbol);
                    if (!p.IsReusable) result.Append(" X");
                }

                result.AppendLine("\"];");
            }

            // Connect parent-child relations
            foreach (var (n, id) in nodeIds)
            {
                foreach (var other in n.Children) result.AppendLine($"  {id} -- {nodeIds[other]}");
            }

            result.Append('}');
            return result.ToString();
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
