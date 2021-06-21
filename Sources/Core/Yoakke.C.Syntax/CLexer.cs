// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Lexer;

namespace Yoakke.C.Syntax
{
    public class CLexer : LexerBase<CTokenType>
    {
        public CLexer(TextReader source)
                : base(source)
        {
        }

        public CLexer(string source)
                : base(source)
        {
        }

        public override Token<CTokenType> Next()
        {
            throw new NotImplementedException();
        }

        private new CToken TakeToken(CTokenType type, int length)
        {
            var t = base.TakeToken(type, length);
            return new CToken(t.Range, EscapeTokenText(t.Text), t.Text, type);
        }

        private static string EscapeTokenText(string str)
        {
            // TODO
            return str;
        }
    }
}
