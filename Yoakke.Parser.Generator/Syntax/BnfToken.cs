using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator.Syntax
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
            Index = index;
            Value = value;
            Type = type;
        }
    }
}
