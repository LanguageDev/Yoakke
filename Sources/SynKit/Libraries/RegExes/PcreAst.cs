// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.RegExes;

/// <summary>
/// AST for PCRE constructs.
/// </summary>
public abstract record class PcreAst
{
    /// <summary>
    /// Desugars this PCRE into plain regex.
    /// </summary>
    /// <param name="settings">The regex settings.</param>
    /// <returns>The equivalent plain regex construct.</returns>
    public abstract RegExAst<char> ToPlainRegex(RegExSettings settings);

    /// <summary>
    /// Represents the alternation of subexpressions.
    /// </summary>
    /// <param name="Elements">The subexpression alternatives.</param>
    public sealed record class Alternation(IReadOnlyList<PcreAst> Elements) : PcreAst
    {
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings)
        {
            if (this.Elements.Count == 0) return RegExAst<char>.Epsilon.Instance;

            var result = this.Elements[0].ToPlainRegex(settings);
            for (var i = 1; i < this.Elements.Count; ++i)
            {
                var right = this.Elements[i].ToPlainRegex(settings);
                result = new RegExAst<char>.Alternation(result, right);
            }
            return result;
        }
    }

    /// <summary>
    /// Represents the sequence of subexpressions.
    /// </summary>
    /// <param name="Elements">The sequence of subexpressions.</param>
    public sealed record class Sequence(IReadOnlyList<PcreAst> Elements) : PcreAst
    {
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings)
        {
            if (this.Elements.Count == 0) return RegExAst<char>.Epsilon.Instance;

            var result = this.Elements[0].ToPlainRegex(settings);
            for (var i = 1; i < this.Elements.Count; ++i)
            {
                var right = this.Elements[i].ToPlainRegex(settings);
                result = new RegExAst<char>.Sequence(result, right);
            }
            return result;
        }
    }

    /// <summary>
    /// Represents some repeated construct.
    /// </summary>
    /// <param name="Element">The repeated element.</param>
    /// <param name="Quantifier">The quantifier.</param>
    public sealed record class Quantified(PcreAst Element, Quantifier Quantifier) : PcreAst
    {
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings) => throw new NotImplementedException();
    }
}
