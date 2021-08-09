// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;
using Yoakke.Parser.Attributes;
using IgnoreAttribute = Yoakke.Lexer.Attributes.IgnoreAttribute;
using Token = Yoakke.Lexer.IToken<Yoakke.Parser.Tests.Issue59Tests.TokenType>;

namespace Yoakke.Parser.Tests
{
    /*
     https://github.com/LanguageDev/Yoakke/issues/59
     */

    [Parser(typeof(Issue59Tests.TokenType))]
    internal partial class Issue59Parser
    {
        [Rule("expression : call")]
        [Rule("call : primary")]
        public static string Identity(string expression) => expression;

        [Rule("call : call '(' ')'")]
        public static string FunctionCall(string callee, Token _open, Token _close) => $"({callee}())";

        [Rule("call : call '.' Identifier")]
        public static string MemberAccess(string @object, Token _dot, Token identifier) => $"({@object}.{identifier.Text})";

        [Rule("primary : Identifier")]
        public static string Primary(Token atom) => atom.Text;
    }

    [TestClass]
    public class Issue59Tests
    {
        [Lexer("LeftRecursionAltsIssueLexer")]
        public enum TokenType
        {
            [End] End,
            [Error] Error,
            [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

            [Regex(Regexes.Identifier)] Identifier,

            [Token(".")] Dot,
            [Token("(")] OpenParen,
            [Token(")")] CloseParen,
        }

        private static string Parse(string source) =>
           new Issue59Parser(new LeftRecursionAltsIssueLexer(source)).ParseCall().Ok.Value;

        [TestMethod]
        public void TestPrimary() => Assert.AreEqual("x", Parse("x"));

        [TestMethod]
        public void TestCall() => Assert.AreEqual("(x())", Parse("x()"));

        [TestMethod]
        public void TestMemberAccess() => Assert.AreEqual("(x.y)", Parse("x.y"));

        [TestMethod]
        public void TestLongChain() => Assert.AreEqual("(((((((x.y)())()).z).w)()).q)", Parse("x.y()().z.w().q"));
    }
}
