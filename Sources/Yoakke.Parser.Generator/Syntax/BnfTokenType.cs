// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Parser.Generator.Syntax
{
    /// <summary>
    /// The possible different kinds for a <see cref="BnfToken"/>.
    /// </summary>
    internal enum BnfTokenType
    {
        End,
        Identifier,
        Colon,
        Pipe,
        OpenParen,
        CloseParen,
        StringLiteral,
        Plus,
        Star,
        QuestionMark,
    }
}
