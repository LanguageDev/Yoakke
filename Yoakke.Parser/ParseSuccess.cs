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
    public readonly struct ParseSuccess<T>
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
        /// Initializes a new <see cref="ParseSuccess{T}"/>.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        /// <param name="offset">The offset in the number of tokens.</param>
        public ParseSuccess(T value, int offset)
        {
            Value = value;
            Offset = offset;
        }
    }
}
