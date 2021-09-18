// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Xunit;
using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;
using Yoakke.Parser.Attributes;
using IgnoreAttribute = Yoakke.Lexer.Attributes.IgnoreAttribute;

namespace Yoakke.Parser.Tests
{
    // https://github.com/LanguageDev/Yoakke/issues/62
    public partial class Issue62Tests
    {
        internal enum TokenType
        {
            [End] End,
            [Error] Error,
            [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

            [Regex(Regexes.Identifier)] Identifier,
            [Token(";")] Semicolon,
        }

        [Lexer(typeof(TokenType))]
        internal partial class Lexer
        {
        }

        [Parser(typeof(TokenType))]
        internal partial class Parser
        {
            [Rule("program : statement*")]
            public static string Program(IReadOnlyList<string> idents) => string.Join(", ", idents);

            [Rule("statement : Identifier ';'")]
            public static string Statement(IToken<TokenType> identifier, IToken<TokenType> _) => identifier.Text;
        }

        private static string Parse(string source) =>
           new Parser(new Lexer(source)).ParseProgram().Ok.Value;

        [Fact]
        public void TestSampleInput() => Assert.Equal(string.Empty, Parse("; ^"));

        [Fact]
        public void TestSingleton() => Assert.Equal("x", Parse("x ;"));

        [Fact]
        public void TestPair() => Assert.Equal("x, y", Parse("x; y;"));
    }
}
