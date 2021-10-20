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
using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;
using Action = Yoakke.Grammar.Lr.Action;

namespace Yoakke.Grammar.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var luaGrammar = @"
    S ::= block

	block ::= [stat_list] [retstat]

    stat_list ::= [stat_list] stat

	stat ::=  ';' | 
		 varlist '=' explist | 
		 functioncall | 
		 label | 
		 break | 
		 goto Name | 
		 do block end | 
		 while exp do block end | 
		 repeat block until exp | 
		 if exp then block [elseif_list] [else block] end | 
		 for Name '=' exp ',' exp [',' exp] do block end | 
		 for namelist in explist do block end | 
		 function funcname funcbody | 
		 local function Name funcbody | 
		 local namelist ['=' explist]

    elseif_list ::= [elseif_list] elseif exp then block

	retstat ::= return [explist] [';']

	label ::= '::' Name '::'

	funcname ::= Name [funcname_list] [':' Name]
    funcname_list ::= [funcname_list] '.' Name

	varlist ::= var [varlist_list]
    varlist_list ::= [varlist_list] ',' var

	var ::=  Name | prefixexp '[' exp ']' | prefixexp '.' Name 

	namelist ::= Name | namelist , Name

	explist ::= exp | explist , exp

	exp ::=  nil | false | true | Numeral | LiteralString | '...' | functiondef | 
		 prefixexp | tableconstructor | exp binop exp | unop exp 

	prefixexp ::= var | functioncall | '(' exp ')'

	functioncall ::=  prefixexp args | prefixexp ':' Name args 

	args ::=  '(' [explist] ')' | tableconstructor | LiteralString 

	functiondef ::= function funcbody

	funcbody ::= '(' [parlist] ')' [block] end

	parlist ::= namelist [',' '...'] | '...'

	tableconstructor ::= '{' [fieldlist] '}'

	fieldlist ::= field [fieldlist_list] [fieldsep]
    fieldlist_list ::= [fieldlist_list] fieldsep field

	field ::= '[' exp ']' '=' exp | Name '=' exp | exp

	fieldsep ::= ',' | ';'

	binop ::=  '+' | '-' | '*' | '/' | '//' | '^' | '%' | 
		 '&' | '~' | '|' | '>>' | '<<' | '..' | 
		 '<' | '<=' | '>' | '>=' | '==' | '~=' | 
		 and | or

	unop ::= '-' | not | '#' | '~'
";

            var luaCode = @"
function allwords()
    local line = io.read() -- current line
    local pos = 1 -- current position in the line
    return function() -- iterator function
        while line do -- repeat while there are lines
            local s, e = string.find(line, ""%w+"", pos)
            if s then -- found a word?
                pos = e + 1 -- update next position
                return string.sub(line, s, e) -- return the word
            else
                line = io.read() -- word not found; try next line
                pos = 1 -- restart from first position
            end
        end
        return nil -- no more lines: end of traversal
    end
end

function prefix(w1, w2)
    return (w1 .. ' ') .. w2
end

local statetab

function insert(index, value)
    if not statetab[index] then
        statetab[index] = {
            n = 0
        }
    end
    table.insert(statetab[index], value)
end

local N = 2
local MAXGEN = 10000
local NOWORD = ""\n""

-- build table
statetab = {}
local w1, w2 = NOWORD, NOWORD
for w in allwords() do
    insert(prefix(w1, w2), w)
    w1 = w2;
    w2 = w;
end
insert(prefix(w1, w2), NOWORD)
-- generate text
w1 = NOWORD;
w2 = NOWORD -- reinitialize
for i = 1, MAXGEN do
    local list = statetab[prefix(w1, w2)]
    -- choose a random item from list
    local r = math.random(table.getn(list))
    local nextword = list[r]
    if nextword == NOWORD then
        return
    end
    io.write(nextword, "" "")
    w1 = w2;
    w2 = nextword
end
";

            var cfg = EbnfParser.ParseGrammar(luaGrammar);
            cfg.AugmentStartSymbol();

            var tables = new (string Name, ILrParsingTable Table)[]
            {
                ("LR0", LrParsingTable.Lr0(cfg)),
                ("SLR", LrParsingTable.Slr(cfg)),
                ("LALR", LrParsingTable.Lalr(cfg)),
                ("CLR", LrParsingTable.Clr(cfg)),
            };

            foreach (var isGss in new[] { false, true })
            {
                foreach (var isInc in new[] { false, true })
                {
                    foreach (var (tname, table) in tables)
                    {
                        Console.WriteLine($"{(isGss ? "GSS" : "Parallel")} / {(isInc ? "Yes" : "No")} / {tname}");
                        GlrParse(table, isGss, isInc, luaCode, "11 ; 0 ; stdin");
                    }
                }
            }
        }

        static void GlrParse(ILrParsingTable table, bool gss, bool incr, string text, params string[] edits) => GlrParse(
            () => gss ? new GraphStructuredStack(table) : new ParallelStacks(table),
            ts => incr ? new IncrementalTreeSource(ts) : new TerminalTreeSource(ts),
            text,
            edits);

        static void GlrParse(Func<INondetStack> makeStack, Func<List<Terminal>, ITreeSource> makeSource, string text, params string[] edits)
        {
            var terminals = LuaLexer.LexTerminals(text);
            var treeSource = makeSource(terminals);
            var editIndex = 0;

        begin:
            var stack = makeStack();

            //Console.WriteLine("=========================");
            //Console.WriteLine(stack.ToDot());
            //Console.WriteLine("=========================");
            while (!treeSource.IsEnd)
            {
                var t = treeSource.Next(stack.CurrentState);
                // Console.WriteLine($"processing {t.Symbol}");
                stack.Feed(t);
                // Console.WriteLine("=========================");
                // Console.WriteLine(stack.ToDot());
                // Console.WriteLine("=========================");
                while (stack.Step())
                {
                    // Console.WriteLine("=========================");
                    // Console.WriteLine(stack.ToDot());
                    // Console.WriteLine("=========================");
                }
            }

            //Console.WriteLine("Nodes:");
            // foreach (var ast in stack.Trees)
            //{
                //Console.WriteLine("=========================");
                //Console.WriteLine(ToDot(CloneIncrementalTree(ast)));
                //Console.WriteLine("=========================");
            //}

            Console.WriteLine($"Shifts: {stack.ShiftCount}");
            Console.WriteLine($"Reduces: {stack.ReduceCount}");
            Console.WriteLine($"Vertices: {stack.VertexCount}");
            Console.WriteLine($"Edges: {stack.EdgeCount}");

            if (editIndex < edits.Length)
            {
                Console.WriteLine($"apply edit {edits[editIndex]}");
                var parts = edits[editIndex].Split(";");
                var start = int.Parse(parts[0].Trim());
                var length = int.Parse(parts[1].Trim());
                var inserted = LuaLexer.LexTerminals(parts[2])
                    .SkipLast(1)
                    .ToList();

                treeSource.Reset(stack.Trees.Count() == 1 ? stack.Trees.First() : null);
                treeSource.MakeEdit(start, length, inserted);

                ++editIndex;
                goto begin;
            }

            // Console.WriteLine("Nodes:");
            // foreach (var ast in stack.Trees)
            //{
            //    Console.WriteLine("=========================");
            //    Console.WriteLine(ToDot(CloneIncrementalTree(ast)));
            //    Console.WriteLine("=========================");
            //}
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

        static void TableStats<T>(ILrParsingTable<T> table)
            where T : ILrItem
        {
            Console.WriteLine($"States: {table.StateAllocator.States.Count}");
            var conflicts = 0;
            var conflictingActions = 0;
            foreach (var t in table.Grammar.Terminals)
            {
                foreach (var s in table.StateAllocator.States)
                {
                    var actions = table.Action[s, t];
                    if (actions.Count > 1)
                    {
                        ++conflictingActions;
                        conflicts += actions.Count - 1;
                    }
                }
            }
            Console.WriteLine($"Conflicting actions: {conflictingActions}");
            Console.WriteLine($"Conflicts: {conflicts}");
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

        static string ReadCode()
        {
            var result = new StringBuilder();
            while (true)
            {
                var line = Console.ReadLine();
                if (line is null || line == "END") return result.ToString();
                result.AppendLine(line);
            }
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

    public enum LuaTokenType
    {
        [Error] Error,
        [End] End,

        [Ignore]
        [Regex(Regexes.Whitespace)]
        [Regex(@"--[^\r\n]*")] Ignore,

        [Token(";")]
        [Token(".")]
        [Token("=")]
        [Token(",")]
        [Token(":")]
        [Token("::")]
        [Token("[")]
        [Token("]")]
        [Token("(")]
        [Token(")")]
        [Token("{")]
        [Token("}")]
        [Token("...")]
        [Token("+")]
        [Token("-")]
        [Token("*")]
        [Token("/")]
        [Token("//")]
        [Token("^")]
        [Token("%")]
        [Token("&")]
        [Token("~")]
        [Token("|")]
        [Token(">>")]
        [Token("<<")]
        [Token("..")]
        [Token("<")]
        [Token("<=")]
        [Token(">")]
        [Token(">=")]
        [Token("==")]
        [Token("~=")]
        [Token("~")]
        [Token("break")]
        [Token("goto")]
        [Token("do")]
        [Token("end")]
        [Token("while")]
        [Token("repeat")]
        [Token("until")]
        [Token("if")]
        [Token("then")]
        [Token("elseif")]
        [Token("else")]
        [Token("for")]
        [Token("in")]
        [Token("function")]
        [Token("local")]
        [Token("return")]
        [Token("nil")]
        [Token("false")]
        [Token("true")]
        [Token("and")]
        [Token("or")]
        [Token("not")]
        Keyword,

        [Regex(Regexes.IntLiteral)] Numeral,
        [Regex(Regexes.StringLiteral)]
        [Regex(@"'((\\[^\n\r])|[^\r\n\\'])*'")]
        LiteralString,
        [Regex(Regexes.Identifier)] Name,
    }

    [Lexer(typeof(LuaTokenType))]
    public partial class LuaLexer
    {
        public static List<Terminal> LexTerminals(string src)
        {
            var l = new LuaLexer(src);
            var res = new List<Terminal>();
            while (true)
            {
                var t = l.Next();
                if (t.Kind == LuaTokenType.End)
                {
                    res.Add(Terminal.EndOfInput);
                    break;
                }
                res.Add(new(t.Kind == LuaTokenType.Keyword ? t.Text : t.Kind.ToString()));
            }
            return res;
        }
    }
}
