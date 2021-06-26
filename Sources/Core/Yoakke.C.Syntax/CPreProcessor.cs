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
        private readonly RingBuffer<CToken> outputBuffer;
        private readonly Dictionary<string, IMacro> macros;

        /// <summary>
        /// The last consumed <see cref="CToken"/> from the <see cref="inputBuffer"/>.
        /// </summary>
        private CToken? lastInputToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="CPreProcessor"/> class.
        /// </summary>
        /// <param name="lexer">The <see cref="ILexer{CToken}"/> to get the <see cref="CToken"/>s from.</param>
        public CPreProcessor(ILexer<CToken> lexer)
        {
            this.lexer = lexer;
            this.outputBuffer = new();
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
            throw new NotImplementedException();
        }

        private CToken Skip()
        {
            this.Peek();
            this.lastInputToken = this.outputBuffer.RemoveFront();
            return this.lastInputToken;
        }

        private CToken Peek(int offset = 0)
        {
            while (this.outputBuffer.Count <= offset)
            {
                var token = this.lexer.Next();
                if (token.Kind == CTokenType.End) return token;
            }
            return this.outputBuffer[offset];
        }

        private static bool IsIdentifier(CTokenType tokenType) =>
               tokenType == CTokenType.Identifier
            || ((int)CTokenType.KeywordAuto <= (int)tokenType && (int)tokenType <= (int)CTokenType.KeywordWhile);
    }
}
