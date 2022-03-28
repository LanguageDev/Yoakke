// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
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
        public override RegExAst<char> ToPlainRegex(RegExSettings settings) =>
            RegExAst.Alternation(this.Elements.Select(e => e.ToPlainRegex(settings)));
    }

    /// <summary>
    /// Represents the sequence of subexpressions.
    /// </summary>
    /// <param name="Elements">The sequence of subexpressions.</param>
    public sealed record class Sequence(IReadOnlyList<PcreAst> Elements) : PcreAst
    {
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings) =>
            RegExAst.Sequence(this.Elements.Select(e => e.ToPlainRegex(settings)));
    }

    /// <summary>
    /// Represents some repeated construct.
    /// </summary>
    /// <param name="Element">The repeated element.</param>
    /// <param name="Quantifier">The quantifier.</param>
    public sealed record class Quantified(PcreAst Element, Quantifier Quantifier) : PcreAst
    {
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings)
        {
            var element = this.Element.ToPlainRegex(settings);
            return this.Quantifier switch
            {
                Quantifier.Optional => RegExAst.Option(element),
                Quantifier.OneOrMore => RegExAst.Repeat1(element),
                Quantifier.ZeroOrMore => RegExAst.Repeat0(element),
                Quantifier.Exactly e => RegExAst.Exactly(element, e.Amount),
                Quantifier.AtLeast l => RegExAst.AtLeast(element, l.Amount),
                Quantifier.AtMost m => RegExAst.AtMost(element, m.Amount),
                Quantifier.Between b => RegExAst.Between(element, b.Min, b.Max),
                _ => throw new InvalidOperationException(),
            };
        }
    }

    /// <summary>
    /// Represents a character literal.
    /// </summary>
    /// <param name="Char">The represented character.</param>
    public sealed record class Literal(char Char) : PcreAst
    {
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings) => RegExAst.Literal(this.Char);
    }

    /// <summary>
    /// A quoted sequence of characters between \Q and \E
    /// </summary>
    /// <param name="Text">The quoted text.</param>
    public sealed record class Quoted(string Text) : PcreAst
    {
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings) => throw new NotImplementedException();
    }

    /// <summary>
    /// Represents some meta-sequence.
    /// </summary>
    /// <param name="Id">The identifying object that identifies the meta-sequence.</param>
    public sealed record class MetaSequence(object Id) : PcreAst
    {
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings) => throw new NotImplementedException();
    }

    /// <summary>
    /// Represents a named character class.
    /// </summary>
    /// <param name="Invert">True, if the class should be inverted.</param>
    /// <param name="Name">The name of the character class.</param>
    public sealed record class NamedCharacterClass(bool Invert, string Name) : PcreAst
    {
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings) => throw new NotImplementedException();
    }

    /// <summary>
    /// Represents a custom character class.
    /// </summary>
    /// <param name="Invert">True, if the class should be inverted.</param>
    /// <param name="Elements">The elements of the character class.</param>
    public sealed record class CharacterClass(bool Invert, IReadOnlyList<CharacterClassElement> Elements) : PcreAst
    {
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings) => throw new NotImplementedException();
    }

    /// <summary>
    /// An element in a custom character class.
    /// </summary>
    public abstract record class CharacterClassElement;

    /// <summary>
    /// A single element in a character class.
    /// </summary>
    /// <param name="Ast">The PCRE expressing the single literal.</param>
    public sealed record class CharacterClassLiteral(PcreAst Ast) : CharacterClassElement;

    /// <summary>
    /// A range of elements in a character class. It can also be coincidental, syntactical match, then
    /// it is simply From or '-' or To.
    /// </summary>
    /// <param name="From">The lower end of the character range.</param>
    /// <param name="To">The upper end of the character range.</param>
    public sealed record class CharacterClassRange(PcreAst From, PcreAst To) : CharacterClassElement;
}
