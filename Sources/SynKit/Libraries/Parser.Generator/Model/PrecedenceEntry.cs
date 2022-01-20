// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Yoakke.SynKit.Parser.Generator.Model;

/// <summary>
/// A single entry in the precedence-table.
/// </summary>
internal class PrecedenceEntry
{
    /// <summary>
    /// True, if the given precedence-level contains left-associative operands.
    /// If false, the precedence-level is considered to contain right-associative operands.
    /// </summary>
    public bool Left { get; }

    /// <summary>
    /// The set of operators on this precedence-level.
    /// </summary>
    public ISet<object> Operators { get; }

    /// <summary>
    /// The method that transforms the given precedence-level, when parsed.
    /// </summary>
    public IMethodSymbol Method { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrecedenceEntry"/> class.
    /// </summary>
    /// <param name="left">True, if the level is left-associative.</param>
    /// <param name="operators">The operators on the level.</param>
    /// <param name="method">The method that transforms the parsed elements of the precedence level.</param>
    public PrecedenceEntry(bool left, ISet<object> operators, IMethodSymbol method)
    {
        this.Left = left;
        this.Operators = operators;
        this.Method = method;
    }
}
