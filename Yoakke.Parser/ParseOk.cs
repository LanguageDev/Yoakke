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
        /// The furthest <see cref="ParseError"/> found so far.
        /// </summary>
        public readonly ParseError? FurthestError;

        /// <summary>
        /// Initializes a new <see cref="ParseOk{T}"/>.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        /// <param name="offset">The offset in the number of tokens.</param>
        /// <param name="furthestError">The furthest <see cref="ParseError"/> found so far.</param>
        public ParseOk(T value, int offset, ParseError? furthestError = null)
        {
            Value = value;
            Offset = offset;
            FurthestError = furthestError;
        }

        public static implicit operator T(ParseOk<T> ok) => ok.Value;
    }
}
