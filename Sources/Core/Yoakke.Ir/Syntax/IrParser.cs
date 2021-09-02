// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Ir.Model;
using Yoakke.Ir.Model.Attributes;
using Yoakke.Lexer;
using Yoakke.Lexer.Streams;
using Type = Yoakke.Ir.Model.Type;

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
        /// Parses an <see cref="Constant"/> value of a given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type of the constant value to parse.</param>
        /// <returns>The parsed <see cref="Constant"/> of type <paramref name="type"/>.</returns>
        public Constant ParseConstant(Type type) => type switch
        {
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

        /// <summary>
        /// Parses an <see cref="Type"/>.
        /// </summary>
        /// <returns>The parsed <see cref="Type"/>.</returns>
        public Type ParseType()
        {
            var peek = this.source.Peek();

            // Pointer type
            if (peek.Kind == IrTokenType.Star)
            {
                this.source.Advance();
                var elementType = this.ParseType();
                throw new NotImplementedException("ptr");
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

        private void Expect(IrTokenType tokenType)
        {
            if (!this.Matches(tokenType)) throw new InvalidOperationException("TODO: Syntax error");
        }

        private bool Matches(IrTokenType tokenType)
        {
            if (this.source.TryPeek(out var token) && token.Kind == tokenType)
            {
                this.source.Advance();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
