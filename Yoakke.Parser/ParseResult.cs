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
        public bool IsOk => value is ParseOk<T>;
        /// <summary>
        /// True, if the result is an error.
        /// </summary>
        public bool IsError => value is ParseError;

        /// <summary>
        /// Retrieves the parse as a success.
        /// </summary>
        public ParseOk<T> Ok => (ParseOk<T>)value;
        /// <summary>
        /// Retrieves the parse as an error.
        /// </summary>
        public ParseError Error => (ParseError)value;
        /// <summary>
        /// Retrieves the furthest error for this result.
        /// </summary>
        public ParseError? FurthestError => value is ParseOk<T> ok ? ok.FurthestError : (ParseError)value;

        /// <summary>
        /// Initializes a new <see cref="ParseResult{T}"/> as a successful parse.
        /// </summary>
        /// <param name="ok">The successful parse description.</param>
        public ParseResult(ParseOk<T> ok)
        {
            value = ok;
        }

        /// <summary>
        /// Initializes a new <see cref="ParseResult{T}"/> as a failing parse.
        /// </summary>
        /// <param name="error">The error description.</param>
        public ParseResult(ParseError error)
        {
            value = error;
        }

        public static implicit operator ParseResult<T>(ParseOk<T> ok) => new(ok);
        public static implicit operator ParseResult<T>(ParseError error) => new(error);
    }
}
