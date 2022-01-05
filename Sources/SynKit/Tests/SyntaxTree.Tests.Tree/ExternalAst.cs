// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.SynKit.SyntaxTree.Attributes;

namespace Yoakke.SynKit.SyntaxTree.Tests.Tree;

[SyntaxTree]
public abstract partial record ExternalAst
{
    public partial record Foo : ExternalAst;

    public partial record Bar : ExternalAst;

    public partial record Baz : ExternalAst;
}
