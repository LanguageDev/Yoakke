// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Yoakke.Parser.Generator
{
    /// <summary>
    /// A single entry in the precedence-table.
    /// </summary>
    internal class PrecedenceEntry
    {
        /// <summary>
        /// True, if the given precedence-level contains left-associative operands.
        /// If false, the precedence-level is considered to contain right-associative operands.
        /// </summary>
        public readonly bool Left;

        /// <summary>
        /// The set of operators on this precedence-level.
        /// </summary>
        public readonly ISet<object> Operators;

        /// <summary>
        /// The method that transforms the given precedence-level, when parsed.
        /// </summary>
        public readonly IMethodSymbol Method;

        public PrecedenceEntry(bool left, ISet<object> operators, IMethodSymbol method)
        {
            this.Left = left;
            this.Operators = operators;
            this.Method = method;
        }
    }
}
