// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata.RegExAst;

/// <summary>
/// Makes a regex construct repeat between a range of values.
/// </summary>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public record RegExRepBetweenNode<TSymbol> : IRegExNode<TSymbol>
{
    /// <summary>
    /// The minimum amount of repetitions.
    /// </summary>
    public int AtLeast { get; }

    /// <summary>
    /// The maximum amount of repetitions. Can be null, if there's no upper limit.
    /// </summary>
    public int? AtMost { get; }

    /// <summary>
    /// The element to be repeated.
    /// </summary>
    public IRegExNode<TSymbol> Element { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegExRepBetweenNode{TSymbol}"/> class.
    /// </summary>
    /// <param name="atLeast">The minimum amount of repetitions required.</param>
    /// <param name="atMost">The maximum amount of repetitions allowed. Can be null, if there is no upper limit.</param>
    /// <param name="element">The element to be repeated.</param>
    public RegExRepBetweenNode(int atLeast, int? atMost, IRegExNode<TSymbol> element)
    {
        if (atLeast < 0) throw new ArgumentOutOfRangeException(nameof(atLeast));
        if (atMost is not null && atMost.Value < atLeast) throw new ArgumentOutOfRangeException(nameof(atMost));

        this.AtLeast = atLeast;
        this.AtMost = atMost;
        this.Element = element;
    }

    /// <inheritdoc/>
    public IRegExNode<TSymbol> Desugar()
    {
        if (this.AtLeast == 0)
        {
            // Just Rep0
            if (this.AtMost is null) return new RegExRep0Node<TSymbol>(this.Element.Desugar());
            // No-op
            if (this.AtMost == 0) return RegExNopNode<TSymbol>.Instance;
            // Optional
            if (this.AtMost == 1) return new RegExOptNode<TSymbol>(this.Element.Desugar());
            // 0..AtMost repetition
            var optElementDesugared = new RegExOptNode<TSymbol>(this.Element.Desugar());
            IRegExNode<TSymbol> result = optElementDesugared;
            for (var i = 1; i < this.AtMost; ++i) result = new RegExSeqNode<TSymbol>(result, optElementDesugared);
            return result;
        }
        else
        {
            // Create the minimum prefix
            var elementDesugared = this.Element.Desugar();
            var result = elementDesugared;
            for (var i = 1; i < this.AtLeast; ++i) result = new RegExSeqNode<TSymbol>(result, elementDesugared);
            // If there's no upper limit, slap on a rep0
            if (this.AtMost is null) return new RegExSeqNode<TSymbol>(result, new RegExRep0Node<TSymbol>(elementDesugared));
            // If upper limit is lower limit, it's an exactly match
            if (this.AtLeast == this.AtMost) return result;
            // Add the suffix
            var optElementDesugared = new RegExOptNode<TSymbol>(elementDesugared);
            for (var i = this.AtLeast; i < this.AtMost; ++i) result = new RegExSeqNode<TSymbol>(result, optElementDesugared);
            return result;
        }
    }

    /// <inheritdoc/>
    public (TState Start, TState End) ThompsonsConstruct<TState>(INfa<TState, TSymbol> nfa, Func<TState> makeState) =>
        throw new NotSupportedException("Element must be desugared.");
}
