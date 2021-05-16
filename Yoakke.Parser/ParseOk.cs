using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Parser
{
    /// <summary>
    /// Represents a successful parse.
    /// </summary>
    /// <typeparam name="T">The value of the parse.</typeparam>
    public readonly struct ParseOk<T>
    {
        /// <summary>
        /// The resulted parse value.
        /// </summary>
        public readonly T Value;
        /// <summary>
        /// The offset in the number of tokens.
        /// </summary>
        public readonly int Offset;
        /// <summary>
        /// The furthest error found.
        /// </summary>
        public readonly ParseError? FurthestError;

        /// <summary>
        /// Initializes a new <see cref="ParseOk{T}"/>.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        /// <param name="offset">The offset in the number of tokens.</param>
        /// <param name="furthestError">The furthest error found.</param>
        public ParseOk(T value, int offset, ParseError? furthestError = null)
        {
            Value = value;
            Offset = offset;
            FurthestError = furthestError;
        }
    }
}
