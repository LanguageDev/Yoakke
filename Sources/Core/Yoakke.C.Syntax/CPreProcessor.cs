// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Lexer;
using Yoakke.Utilities;

namespace Yoakke.C.Syntax
{
    /// <summary>
    /// A default <see cref="IPreProcessor"/> that tries to mimic what the standard pre-processors do.
    /// </summary>
    public class CPreProcessor : IPreProcessor
    {
        private readonly ILexer<CToken> lexer;
        private readonly RingBuffer<CToken> readBuffer;
        private readonly Dictionary<string, IMacro> macros;
        private CToken? lastToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="CPreProcessor"/> class.
        /// </summary>
        /// <param name="lexer">The <see cref="ILexer{CToken}"/> to get the <see cref="CToken"/>s from.</param>
        public CPreProcessor(ILexer<CToken> lexer)
        {
            this.lexer = lexer;
            this.readBuffer = new();
            this.macros = new();
        }

        public bool IsDefined(string name) => this.macros.ContainsKey(name);

        public IMacro? Define(IMacro macro) => this.macros.Remove(macro.Name, out var oldMacro)
            ? oldMacro
            : null;

        public IMacro? Undefine(string name) => this.macros.Remove(name, out var macro)
            ? macro
            : null;

        public CToken Next()
        {
            this.Peek();
            return this.Skip();
        }

        private CToken Skip() => this.readBuffer.RemoveFront();

        private CToken Peek(int offset = 0)
        {
            while (this.readBuffer.Count <= offset)
            {
                this.LexNext();
            }
            return this.readBuffer[offset];
        }

        private void LexNext()
        {
            var token = this.lexer.Next();
            if (token.Kind == CTokenType.Hash
            && (this.lastToken is null || (this.lastToken.LogicalRange.End.Line != token.LogicalRange.Start.Line)))
            {
                // Potential preprocessor directive, a '#' on a fresh line
                // TODO: Parse directive
            }
            else if (IsIdentifier(token.Kind) && this.macros.TryGetValue(token.LogicalText, out var macro))
            {
                // We got a macro identifier
                // TODO: Parse macro call
            }
            // Just a regular token, add it and set as last
            lastToken = token;
            this.readBuffer.AddBack(lastToken);
        }

        private static bool IsIdentifier(CTokenType tokenType) =>
               tokenType == CTokenType.Identifier
            || ((int)CTokenType.KeywordAuto <= (int)tokenType && (int)tokenType <= (int)CTokenType.KeywordWhile);
    }
}
