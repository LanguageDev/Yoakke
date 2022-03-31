// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Collections.Intervals;

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
    /// A wrapper if a plain regex is embedded inside PCRE.
    /// </summary>
    /// <param name="Ast">The plain regex construct.</param>
    public sealed record class Desugared(RegExAst<char> Ast) : PcreAst
    {
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings) => this.Ast;
    }

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
        public override RegExAst<char> ToPlainRegex(RegExSettings settings) =>
            RegExAst.Sequence(this.Text.Select(RegExAst.Literal));
    }

    /// <summary>
    /// Represents some meta-sequence.
    /// </summary>
    /// <param name="Id">The identifying object that identifies the meta-sequence.</param>
    public sealed record class MetaSequence(object Id) : PcreAst
    {
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings) =>
            settings.MetaSequences[this.Id];
    }

    /// <summary>
    /// Represents a named character class.
    /// </summary>
    /// <param name="Invert">True, if the class should be inverted.</param>
    /// <param name="Name">The name of the character class.</param>
    public sealed record class NamedCharacterClass(bool Invert, string Name) : PcreAst
    {
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings) =>
            RegExAst.LiteralRange(this.Invert, settings.NamedCharacterClasses[this.Name]);
    }

    /// <summary>
    /// Represents a custom character class.
    /// </summary>
    /// <param name="Invert">True, if the class should be inverted.</param>
    /// <param name="Elements">The elements of the character class.</param>
    public sealed record class CharacterClass(bool Invert, IReadOnlyList<PcreAst> Elements) : PcreAst
    {
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings)
        {
            var set = new IntervalSet<char>(IntervalComparer<char>.Default);
            foreach (var element in this.Elements)
            {
                switch (element)
                {
                case Literal l:
                {
                    set.Add(Interval.Singleton(l.Char));
                    break;
                }
                case Quoted q:
                {
                    foreach (var ch in q.Text) set.Add(Interval.Singleton(ch));
                    break;
                }
                case NamedCharacterClass c:
                {
                    var subSet = new IntervalSet<char>(
                        intervals: settings.NamedCharacterClasses[c.Name],
                        comparer: IntervalComparer<char>.Default);
                    if (c.Invert) subSet.Complement();
                    foreach (var iv in subSet) set.Add(iv);
                    break;
                }
                case CharacterClassRange r:
                {
                    set.Add(Interval.Inclusive(r.From, r.To));
                    break;
                }
                default:
                    throw new InvalidOperationException();
                }
            }
            return RegExAst.LiteralRange(this.Invert, set);
        }
    }

    /// <summary>
    /// A range of elements in a character class.
    /// This type can only live inside a character class, it is invalid on its own.
    /// </summary>
    /// <param name="From">The lower end of the character range.</param>
    /// <param name="To">The upper end of the character range.</param>
    public sealed record class CharacterClassRange(char From, char To) : PcreAst
    {
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings) =>
            throw new InvalidOperationException("this type can only be inside a character class");
    }

    /// <summary>
    /// Matches a character with the given property. The \p{...} construct in PCRE.
    /// </summary>
    /// <param name="Invert">True, if the construct should be inverted.</param>
    /// <param name="PropertyName">The name of the property to match.</param>
    public sealed record class CharProperty(bool Invert, string PropertyName) : PcreAst
    {
        // TODO
        /// <inheritdoc/>
        public override RegExAst<char> ToPlainRegex(RegExSettings settings) =>
            throw new NotImplementedException();
    }
}
