// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.IO;
using Xunit;
using Yoakke.SynKit.Lexer.Attributes;

namespace Yoakke.SynKit.Lexer.Tests;

// https://github.com/LanguageDev/Yoakke/issues/17
public partial class Issue17Tests
{
    internal enum TokenType
    {
        [Error] Error,
        [End] End,
    }

    [Lexer(typeof(TokenType))]
    internal partial class ImplicitCtorLexer
    {
    }

    [Lexer(typeof(TokenType))]
    internal partial class ExplicitCtorLexer
    {
        [CharSource]
        private readonly ICharStream source;

        public ExplicitCtorLexer(string text)
        {
            this.source = new TextReaderCharStream(new StringReader(text));
        }
    }

    [Fact]
    public void ImplicitCtors()
    {
        Assert.Equal(5, typeof(ImplicitCtorLexer).GetConstructors().Length);
        Assert.NotNull(typeof(ImplicitCtorLexer).GetConstructor(new[] { typeof(string) }));
        Assert.NotNull(typeof(ImplicitCtorLexer).GetConstructor(new[] { typeof(string), typeof(string) }));
        Assert.NotNull(typeof(ImplicitCtorLexer).GetConstructor(new[] { typeof(TextReader) }));
        Assert.NotNull(typeof(ImplicitCtorLexer).GetConstructor(new[] { typeof(Text.SourceFile) }));
        Assert.NotNull(typeof(ImplicitCtorLexer).GetConstructor(new[] { typeof(ICharStream) }));
    }

    [Fact]
    public void ExplicitCtors()
    {
        Assert.Single(typeof(ExplicitCtorLexer).GetConstructors());
        Assert.NotNull(typeof(ExplicitCtorLexer).GetConstructor(new[] { typeof(string) }));
        Assert.Null(typeof(ExplicitCtorLexer).GetConstructor(new[] { typeof(Text.SourceFile) }));
        Assert.Null(typeof(ExplicitCtorLexer).GetConstructor(new[] { typeof(TextReader) }));
    }
}
