// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Ir.Syntax
{
    /// <summary>
    /// Different token kinds for the IR.
    /// </summary>
    public enum IrTokenType
    {
        /// <summary>
        /// End of source.
        /// </summary>
        End,

        /// <summary>
        /// Unrecognized.
        /// </summary>
        Unknown,

        /// <summary>
        /// '('.
        /// </summary>
        OpenParen,

        /// <summary>
        /// ')'.
        /// </summary>
        CloseParen,

        /// <summary>
        /// '['.
        /// </summary>
        OpenBracket,

        /// <summary>
        /// ']'.
        /// </summary>
        CloseBracket,

        /// <summary>
        /// '{'.
        /// </summary>
        OpenBrace,

        /// <summary>
        /// '}'.
        /// </summary>
        CloseBrace,

        /// <summary>
        /// '.'.
        /// </summary>
        Dot,

        /// <summary>
        /// ','.
        /// </summary>
        Comma,

        /// <summary>
        /// ':'.
        /// </summary>
        Colon,

        /// <summary>
        /// ';'.
        /// </summary>
        Semicolon,

        /// <summary>
        /// '='.
        /// </summary>
        Assign,

        /// <summary>
        /// Any C integer literal, regardless of radix.
        /// </summary>
        IntLiteral,

        /// <summary>
        /// Any C floating point number.
        /// </summary>
        FloatLiteral,

        /// <summary>
        /// A C string literal.
        /// </summary>
        StringLiteral,

        /// <summary>
        /// Any C identifier that is not a keyword.
        /// </summary>
        Identifier,

        /// <summary>
        /// 'block' keyword.
        /// </summary>
        KeywordBlock,

        /// <summary>
        /// 'proc' keyword.
        /// </summary>
        KeywordProc,
    }
}
