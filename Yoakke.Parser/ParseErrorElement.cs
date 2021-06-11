﻿using System.Collections.Generic;

namespace Yoakke.Parser
{
    /// <summary>
    /// A single error case of parsing.
    /// </summary>
    public class ParseErrorElement
    {
        /// <summary>
        /// The expected possible inputs.
        /// </summary>
        public readonly IReadOnlySet<object> Expected;

        /// <summary>
        /// The context in which the error occurred.
        /// </summary>
        public readonly string Context;

        /// <summary>
        /// Initializes a new <see cref="ParseErrorElement"/>.
        /// </summary>
        /// <param name="expected">The expected input.</param>
        /// <param name="context">The context in which the error occurred.</param>
        public ParseErrorElement(object expected, string context)
            : this(new HashSet<object> { expected }, context)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ParseErrorElement"/>.
        /// </summary>
        /// <param name="expected">The expected possible inputs.</param>
        /// <param name="context">The context in which the error occurred.</param>
        public ParseErrorElement(IReadOnlySet<object> expected, string context)
        {
            this.Expected = expected;
            this.Context = context;
        }
    }
}
