// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

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
        public bool IsOk => this.value is ParseOk<T>;

        /// <summary>
        /// True, if the result is an error.
        /// </summary>
        public bool IsError => this.value is ParseError;

        /// <summary>
        /// Retrieves the parse as a success.
        /// </summary>
        public ParseOk<T> Ok => (ParseOk<T>)this.value;

        /// <summary>
        /// Retrieves the parse as an error.
        /// </summary>
        public ParseError Error => (ParseError)this.value;

        /// <summary>
        /// Retrieves the furthest error for this result.
        /// </summary>
        public ParseError? FurthestError => this.value is ParseOk<T> ok ? ok.FurthestError : (ParseError)this.value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult{T}"/> struct.
        /// </summary>
        /// <param name="ok">The successful parse description.</param>
        public ParseResult(ParseOk<T> ok)
        {
            this.value = ok;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult{T}"/> struct.
        /// </summary>
        /// <param name="error">The error description.</param>
        public ParseResult(ParseError error)
        {
            this.value = error;
        }

        /// <summary>
        /// Implicitly converts a <see cref="ParseOk{T}"/> into a <see cref="ParseResult{T}"/>.
        /// </summary>
        /// <param name="ok">The <see cref="ParseOk{T}"/> to convert.</param>
        public static implicit operator ParseResult<T>(ParseOk<T> ok) => new(ok);

        /// <summary>
        /// Implicitly converts a <see cref="ParseError"/> into a <see cref="ParseResult{T}"/>.
        /// </summary>
        /// <param name="error">The <see cref="ParseError"/> to convert.</param>
        public static implicit operator ParseResult<T>(ParseError error) => new(error);
    }
}
