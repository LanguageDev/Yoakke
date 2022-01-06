// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Xunit;
using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;
using Yoakke.Parser.Attributes;
using IgnoreAttribute = Yoakke.Lexer.Attributes.IgnoreAttribute;
using Token = Yoakke.Lexer.IToken<Yoakke.Parser.Tests.Issue121Tests.TokenType>;

namespace Yoakke.Parser.Tests
{
    // https://github.com/LanguageDev/Yoakke/issues/121
    public partial class Issue121Tests
    {
        internal enum TokenType
        {
            [End] End,
            [Error] Error,
            [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

            [Regex(Regexes.Identifier)] Identifier,

            [Token("1")] One,
            [Token("2")] Two,
            [Token("(")] OpenParen,
            [Token(")")] CloseParen,
            [Token("[")] OpenBracket,
            [Token("]")] CloseBracket,
        }

        [Lexer(typeof(TokenType))]
        internal partial class Lexer
        {
        }

        [Parser(typeof(TokenType))]
        internal partial class Parser
        {
            [Rule("abstract_declarator: '1'")]
            private static string One(Token t) => "1";

            [Rule("abstract_declarator: '1'? direct_abstract_declarator")]
            private static string AbstractDeclarator(Token? one, string direct) => one is null
                ? direct
                : $"1{direct}";

            [Rule("direct_abstract_declarator: '(' abstract_declarator ')'")]
            private static string Paren(Token lp, string inner, Token rp) => $"({inner})";

            [Rule("direct_abstract_declarator: direct_abstract_declarator? '[' '2'? ']'")]
            private static string Bracket(string? dir, Token lb, Token? two, Token rb) =>
                $"{dir}[{(two is null ? string.Empty : "2")}]";
        }

        private static string Parse(string source) =>
           new Parser(new Lexer(source)).ParseAbstractDeclarator().Ok.Value;

        [Theory]
        [InlineData("1", "1")]
        [InlineData("(1)", "(1)")]
        [InlineData("1(1)", "1(1)")]
        [InlineData("[]", "[]")]
        public void Tests(string expected, string input) => Assert.Equal(expected, Parse(input));
    }
}
