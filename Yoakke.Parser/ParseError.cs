using System.Collections.Generic;
using System.Linq;
using Yoakke.Lexer;

namespace Yoakke.Parser
{
    /// <summary>
    /// Describes a parse error.
    /// </summary>
    public class ParseError
    {
        /// <summary>
        /// The error cases in different parse contexts.
        /// </summary>
        public readonly IReadOnlyDictionary<string, ParseErrorElement> Elements;
        /// <summary>
        /// The input that was found, if any.
        /// </summary>
        public readonly IToken? Got;

        /// <summary>
        /// Initializes a new <see cref="ParseError"/>.
        /// </summary>
        /// <param name="expected">The expected input.</param>
        /// <param name="got">The input that was found.</param>
        /// <param name="context">The context in which the error occurred.</param>
        public ParseError(object expected, IToken? got, string context)
            : this(new Dictionary<string, ParseErrorElement> { { context, new ParseErrorElement(expected, context) } }, got)
        {
        }

        private ParseError(IReadOnlyDictionary<string, ParseErrorElement> elements, IToken? got)
        {
            this.Elements = elements;
            this.Got = got;
        }

        /// <summary>
        /// Unifies two alternative <see cref="ParseError"/>s.
        /// </summary>
        /// <param name="first">The first error to unify.</param>
        /// <param name="second">The second error to unify.</param>
        /// <returns>The error that represents both of them properly.</returns>
        public static ParseError? Unify(ParseError? first, ParseError? second)
        {
            if (first is null && second is null) return null;
            if (first is null) return second!;
            if (second is null) return first;
            if (first.Got == null || second.Got == null)
            {
                // At least one of them is out of the input
                if (first.Got == null && second.Got != null) return first;
                if (first.Got != null && second.Got == null) return second;
            }
            else
            {
                if (first.Got.Range.Start < second.Got.Range.Start) return second;
                if (second.Got.Range.Start < first.Got.Range.Start) return first;
            }
            // Both of them got stuck at the same place, merge entries
            var elements = first.Elements.Values.ToDictionary(e => e.Context, e => e.Expected.ToHashSet());
            foreach (var element in second.Elements.Values)
            {
                if (elements.TryGetValue(element.Context, out var part))
                {
                    foreach (var e in element.Expected) part.Add(e);
                }
                else
                {
                    part = element.Expected.ToHashSet();
                    elements.Add(element.Context, part);
                }
            }
            return new ParseError(
                elements.ToDictionary(kv => kv.Key, kv => new ParseErrorElement(kv.Value, kv.Key)),
                first.Got);
        }
    }
}
