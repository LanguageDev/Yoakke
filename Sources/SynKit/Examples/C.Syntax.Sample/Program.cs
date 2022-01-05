// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SynKit.C.Syntax.Sample;

class Program
{
    static void Main(string[] args)
    {
        var sourceCode = @"
#define FOO(x, y) x ## y
FOO(L, ""asd"")
";
        var lexer = new CLexer(sourceCode);
        var pp = new CPreProcessor(lexer);

        while (true)
        {
            var token = pp.Next();
            if (token.Kind == CTokenType.End) break;
            Console.WriteLine(token.LogicalText);
        }
    }
}
