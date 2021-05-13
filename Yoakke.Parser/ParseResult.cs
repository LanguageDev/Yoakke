using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Parser
{
    /// <summary>
    /// The result of a parse.
    /// </summary>
    /// <typeparam name="T">The parsed value type.</typeparam>
    public readonly struct ParseResult<T>
    {
        private readonly object value;

        /// <summary>
        /// True, if the result is a success.
        /// </summary>
        public bool IsSuccess => value is ParseSuccess<T>;
        /// <summary>
        /// True, if the result is an error.
        /// </summary>
        public bool IsError => value is ParseError;

        /// <summary>
        /// Retrieves the parse as a success.
        /// </summary>
        public ParseSuccess<T> Success => (ParseSuccess<T>)value;
        /// <summary>
        /// Retrieves the parse as an error.
        /// </summary>
        public ParseError Error => (ParseError)value;

        /// <summary>
        /// Initializes a new <see cref="ParseResult{T}"/> as a successful parse.
        /// </summary>
        /// <param name="success">The successful parse description.</param>
        public ParseResult(ParseSuccess<T> success)
        {
            value = success;
        }

        /// <summary>
        /// Initializes a new <see cref="ParseResult{T}"/> as a failing parse.
        /// </summary>
        /// <param name="error">The error description.</param>
        public ParseResult(ParseError error)
        {
            value = error;
        }

        /// <summary>
        /// Unifies two alternative parse results.
        /// </summary>
        /// <param name="first">The first result to unify.</param>
        /// <param name="second">The second result to unify.</param>
        /// <returns>The unified parse results.</returns>
        public static ParseResult<T> Unify(ParseResult<T> first, ParseResult<T> second)
        {
            if (first.IsSuccess && second.IsSuccess)
            {
                if (second.Success.Offset > first.Success.Offset) return second;
                // NOTE: Even if they are equal, we return the first
                return first;
            }
            if (first.IsSuccess) return first;
            if (second.IsSuccess) return second;
            // Both are errors
            return new ParseResult<T>(ParseError.Unify(first.Error, second.Error));
        }
    }
}
