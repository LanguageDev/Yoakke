// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.SynKit.Text;

namespace Yoakke.SynKit.Lexer.Tests;

public abstract class TestBase<TKind>
    where TKind : notnull
{
    protected static Token<TKind> Token(string value, TKind tt, Range r) => new(r, new Location(new SourceFile("<no-location>", "test"), r), value, tt);
    protected static Token<TKind> Token(string value, Location location, TKind tt, Range r) => new(r, location, value, tt);

    protected static Range Range((int Line, int Column) p1, (int Line, int Column) p2) =>
        new(new Position(p1.Line, p1.Column), new Position(p2.Line, p2.Column));

    protected static Range Range((int Line, int Column) p1, int len) =>
        new(new Position(p1.Line, p1.Column), len);
}
