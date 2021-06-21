using System;
using System.Text;
using Yoakke.Collections.RegEx;

namespace Yoakke.C.Lexer.Generator
{
    enum InterpretationMode
    {
        Literal,
        Regex,
    }

    class Program
    {
        private static readonly RegExParser RegExParser = new RegExParser();
        private static readonly RegExAst LineContinuation = RegExParser.Parse(@"(\\ *(\r\n|\r|\n))*");

        static void Main(string[] args)
        {
            InterpretationMode? mode = null;

            while (true)
            {
                var line = Console.ReadLine().Trim();
                if (line.StartsWith("$lit"))
                {
                    mode = InterpretationMode.Literal;
                    continue;
                }
                if (line.StartsWith("$regex"))
                {
                    mode = InterpretationMode.Regex;
                    continue;
                }

                if (mode is null)
                {
                    Console.WriteLine("Make sure to set a mode with $lit or $regex first!");
                    continue;
                }

                var regexText = mode == InterpretationMode.Literal
                    ? RegExParser.Escape(line)
                    : line;
                var regexAst = RegExParser.Parse(regexText);
                var withLineContinuations = InsertLineContinuations(regexAst);
                var regexStr = ToRegexString(withLineContinuations);
                Console.WriteLine(regexStr);
            }
        }

        /// <summary>
        /// Converts the regex AST into a regex string literal.
        /// </summary>
        private static string ToRegexString(RegExAst node)
        {
            var result = new StringBuilder();
            result.Append("@\"");
            ToRegexString(node, result, 0);
            result.Append("\"");
            return result.ToString();
        }

        private static void ToRegexString(RegExAst node, StringBuilder result, int currentPrecedence)
        {
            var nodePrecedence = GetPrecedence(node);

            if (nodePrecedence < currentPrecedence) result.Append('(');
            switch (node)
            {
            case RegExAst.Alt alt:
                ToRegexString(alt.First, result, nodePrecedence);
                result.Append('|');
                ToRegexString(alt.Second, result, nodePrecedence);
                break;

            case RegExAst.Seq seq:
                ToRegexString(seq.First, result, nodePrecedence);
                ToRegexString(seq.Second, result, nodePrecedence);
                break;

            case RegExAst.Rep0 rep0:
                ToRegexString(rep0.Subexpr, result, nodePrecedence);
                result.Append('*');
                break;

            case RegExAst.Rep1 rep1:
                ToRegexString(rep1.Subexpr, result, nodePrecedence);
                result.Append('+');
                break;

            case RegExAst.Opt opt:
                ToRegexString(opt.Subexpr, result, nodePrecedence);
                result.Append('?');
                break;

            case RegExAst.Quantified quant:
                ToRegexString(quant.Subexpr, result, nodePrecedence);
                result.Append('{');
                result.Append(quant.AtLeast);
                result.Append(',');
                if (quant.AtMost is not null) result.Append(quant.AtMost.Value);
                result.Append('}');
                break;

            case RegExAst.Literal lit:
                result.Append(EscapeCharacter(lit.Char));
                break;

            case RegExAst.LiteralRange litRange:
                result.Append('[');
                if (litRange.Negate) result.Append('^');
                foreach (var (from, to) in litRange.Ranges)
                {
                    result.Append(EscapeCharacter(from));
                    if (from != to)
                    {
                        result.Append('-');
                        result.Append(EscapeCharacter(to));
                    }
                }
                result.Append(']');
                break;

            default:
                throw new NotSupportedException();
            }
            if (nodePrecedence < currentPrecedence) result.Append(')');
        }

        private static string EscapeCharacter(char ch)
        {
            if (ch == '\r') return @"\r";
            if (ch == '\n') return @"\n";
            if (ch == '\\') return @"\\";
            return ch.ToString();
        }

        /// <summary>
        /// Inserts line continuation sequences into the regex.
        /// </summary>
        private static RegExAst InsertLineContinuations(RegExAst node) => node switch
        {
            // The only interesting case, sequencing line continuation in between
            RegExAst.Seq seq => new RegExAst.Seq(
                InsertLineContinuations(seq.First),
                new RegExAst.Seq(
                    LineContinuation,
                    InsertLineContinuations(seq.Second))),

            // The rest are all insertions into subexpressions
            RegExAst.Alt alt => new RegExAst.Alt(InsertLineContinuations(alt.First), InsertLineContinuations(alt.Second)),
            RegExAst.Rep0 rep0 => new RegExAst.Rep0(InsertLineContinuations(rep0.Subexpr)),
            RegExAst.Rep1 rep1 => new RegExAst.Rep1(InsertLineContinuations(rep1.Subexpr)),
            RegExAst.Quantified quant => new RegExAst.Quantified(InsertLineContinuations(quant.Subexpr), quant.AtLeast, quant.AtMost),
            RegExAst.Opt opt => new RegExAst.Opt(InsertLineContinuations(opt.Subexpr)),

            // For atoms we do nothing
               RegExAst.Literal
            or RegExAst.LiteralRange => node,

            _ => throw new NotSupportedException(),
        };

        /// <summary>
        /// Returns a precedence for the given node (higher means higher precedence).
        /// Alternative has a precedence of 0, which is the lowest.
        /// </summary>
        private static int GetPrecedence(RegExAst node) => node switch
        {
            RegExAst.Alt => 0,
            RegExAst.Seq => 1,
               RegExAst.Opt
            or RegExAst.Rep0
            or RegExAst.Rep1
            or RegExAst.Quantified => 2,
               RegExAst.Literal
            or RegExAst.LiteralRange => 3,
            _ => throw new NotSupportedException(),
        };
    }
}
