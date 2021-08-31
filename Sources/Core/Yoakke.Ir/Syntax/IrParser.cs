// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Ir.Model.Attributes;
using Yoakke.Ir.Model.Types;
using Yoakke.Ir.Model.Values;
using Yoakke.Lexer;
using Yoakke.Lexer.Streams;

namespace Yoakke.Ir.Syntax
{
    /// <summary>
    /// Parser for IR model elements.
    /// </summary>
    public class IrParser
    {
        private readonly ITokenStream<IToken<IrTokenType>> source;

        /// <summary>
        /// Initializes a new instance of the <see cref="IrParser"/> class.
        /// </summary>
        /// <param name="source">The token source to parse from.</param>
        public IrParser(ITokenStream<IToken<IrTokenType>> source)
        {
            this.source = source;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IrParser"/> class.
        /// </summary>
        /// <param name="source">The token source to parse from.</param>
        public IrParser(ILexer<IToken<IrTokenType>> source)
            : this(source.AsTokenStream())
        {
        }

        /// <summary>
        /// Parses an <see cref="IConstant"/> value of a given <see cref="IType"/>.
        /// </summary>
        /// <param name="type">The type of the constant value to parse.</param>
        /// <returns>The parsed <see cref="IConstant"/> of type <paramref name="type"/>.</returns>
        public IConstant ParseConstant(IType type) => type switch
        {
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

        /// <summary>
        /// Parses an <see cref="IType"/>.
        /// </summary>
        /// <returns>The parsed <see cref="IType"/>.</returns>
        public IType ParseType()
        {
            var peek = this.source.Peek();

            // Pointer type
            if (peek.Kind == IrTokenType.Star)
            {
                this.source.Advance();
                var elementType = this.ParseType();
                return new Ptr(elementType);
            }

            // Procedure type
            if (peek.Kind == IrTokenType.KeywordProc)
            {
                this.source.Advance();
                throw new NotImplementedException("proc");
            }

            // Array type
            if (peek.Kind == IrTokenType.OpenBracket)
            {
                this.source.Advance();
                throw new NotImplementedException("array");
            }

            if (peek.Kind == IrTokenType.Identifier)
            {
                throw new NotImplementedException("bool, int, offset, simd, type, uint, void");
            }

            throw new NotImplementedException("unknown");
        }
    }
}
