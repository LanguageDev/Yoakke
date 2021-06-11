﻿namespace Yoakke.Parser.Generator.Syntax
{
    /// <summary>
    /// A single token (terminal) in the BNF grammar.
    /// </summary>
    internal class BnfToken
    {
        /// <summary>
        /// 0-based index in the BNF source.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// The textual value of this <see cref="BnfToken"/>.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// The kind of this <see cref="BnfToken"/>.
        /// </summary>
        public readonly BnfTokenType Type;

        public BnfToken(int index, string value, BnfTokenType type)
        {
            this.Index = index;
            this.Value = value;
            this.Type = type;
        }
    }
}
