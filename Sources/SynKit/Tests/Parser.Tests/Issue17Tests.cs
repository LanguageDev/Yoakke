// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Xunit;
using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Parser.Attributes;
using Yoakke.Streams;

namespace Yoakke.SynKit.Parser.Tests;

// https://github.com/LanguageDev/Yoakke/issues/17
public partial class Issue17Tests
{
    [Parser]
    internal partial class ImplicitCtorParser
    {
    }

    [Parser]
    internal partial class ExplicitCtorParser
    {
        [TokenSource]
        private readonly IStream<IToken> tokens;

        public ExplicitCtorParser(IEnumerable<IToken> tokens)
        {
            this.tokens = new EnumerableStream<IToken>(tokens);
        }
    }

    [Fact]
    public void ImplicitCtors()
    {
        Assert.Equal(3, typeof(ImplicitCtorParser).GetConstructors().Length);
        Assert.NotNull(typeof(ImplicitCtorParser).GetConstructor(new[] { typeof(IEnumerable<IToken>) }));
        Assert.NotNull(typeof(ImplicitCtorParser).GetConstructor(new[] { typeof(ILexer<IToken>) }));
        Assert.NotNull(typeof(ImplicitCtorParser).GetConstructor(new[] { typeof(IPeekableStream<IToken>) }));
    }

    [Fact]
    public void ExplicitCtors()
    {
        Assert.Single(typeof(ExplicitCtorParser).GetConstructors());
        Assert.NotNull(typeof(ExplicitCtorParser).GetConstructor(new[] { typeof(IEnumerable<IToken>) }));
        Assert.Null(typeof(ExplicitCtorParser).GetConstructor(new[] { typeof(ILexer<IToken>) }));
    }
}
