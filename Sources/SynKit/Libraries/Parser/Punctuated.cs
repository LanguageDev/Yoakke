// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Yoakke.SynKit.Parser;

/// <summary>
/// Represents a punctuated sequence of parsed values.
/// </summary>
/// <typeparam name="TValue">The punctuated value type.</typeparam>
/// <typeparam name="TPunct">The punctuation element type.</typeparam>
public class Punctuated<TValue, TPunct> : IReadOnlyList<PunctuatedValue<TValue, TPunct>>
{
    private readonly IReadOnlyList<PunctuatedValue<TValue, TPunct>> underlying;

    /// <summary>
    /// The punctuated values.
    /// </summary>
    public IEnumerable<TValue> Values => this.underlying.Select(e => e.Value);

    /// <summary>
    /// The punctuations.
    /// </summary>
    public IEnumerable<TPunct> Punctuations => this.underlying
        .Select(e => e.Punctuation)
        .OfType<TPunct>();

    /// <inheritdoc/>
    public int Count => this.underlying.Count;

    /// <inheritdoc/>
    public PunctuatedValue<TValue, TPunct> this[int index] => this.underlying[index];

    /// <summary>
    /// Initializes a new instance of the <see cref="Punctuated{TValue, TPunct}"/> class.
    /// </summary>
    public Punctuated()
        : this(Enumerable.Empty<PunctuatedValue<TValue, TPunct>>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Punctuated{TValue, TPunct}"/> class.
    /// </summary>
    /// <param name="elements">The list of <see cref="PunctuatedValue{TValue, TPunct}"/>s
    /// this sequence consists of.</param>
    public Punctuated(IReadOnlyList<PunctuatedValue<TValue, TPunct>> elements)
    {
        this.underlying = elements;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Punctuated{TValue, TPunct}"/> class.
    /// </summary>
    /// <param name="elements">The list of <see cref="PunctuatedValue{TValue, TPunct}"/>s
    /// this sequence consists of.</param>
    public Punctuated(IEnumerable<PunctuatedValue<TValue, TPunct>> elements)
    {
        this.underlying = elements.ToArray();
    }

    /// <inheritdoc/>
    public IEnumerator<PunctuatedValue<TValue, TPunct>> GetEnumerator() => this.underlying.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <summary>
    /// Implicit conversion from 0 or more elements without a trailing separator.
    /// </summary>
    /// <param name="elements">The element construct to convert.</param>
    public static implicit operator Punctuated<TValue, TPunct>((TValue, IReadOnlyList<(TPunct, TValue)>)? elements)
    {
        if (elements == null) return new Punctuated<TValue, TPunct>();
        return elements.Value;
    }

    /// <summary>
    /// Implicit conversion from 1 or more elements without a trailing separator.
    /// </summary>
    /// <param name="elements">The element construct to convert.</param>
    public static implicit operator Punctuated<TValue, TPunct>((TValue, IReadOnlyList<(TPunct, TValue)>) elements)
    {
        var result = new List<PunctuatedValue<TValue, TPunct>>();
        var prevValue = elements.Item1;
        foreach (var (punct, nextValue) in elements.Item2)
        {
            result.Add(new PunctuatedValue<TValue, TPunct>(prevValue, punct));
            prevValue = nextValue;
        }
        result.Add(new PunctuatedValue<TValue, TPunct>(prevValue, default));
        return new Punctuated<TValue, TPunct>(result);
    }

    /// <summary>
    /// Implicit conversion from 0 or more elements with optional trailing separator.
    /// </summary>
    /// <param name="elements">The element construct to convert.</param>
    public static implicit operator Punctuated<TValue, TPunct>((TValue, IReadOnlyList<(TPunct, TValue)>, TPunct)? elements)
    {
        if (elements == null) return new Punctuated<TValue, TPunct>();
        return elements.Value;
    }

    /// <summary>
    /// Implicit conversion from 1 or more elements with optional trailing separator.
    /// </summary>
    /// <param name="elements">The element construct to convert.</param>
    public static implicit operator Punctuated<TValue, TPunct>((TValue, IReadOnlyList<(TPunct, TValue)>, TPunct) elements)
    {
        var result = new List<PunctuatedValue<TValue, TPunct>>();
        var prevValue = elements.Item1;
        foreach (var (punct, nextValue) in elements.Item2)
        {
            result.Add(new PunctuatedValue<TValue, TPunct>(prevValue, punct));
            prevValue = nextValue;
        }
        result.Add(new PunctuatedValue<TValue, TPunct>(prevValue, elements.Item3));
        return new Punctuated<TValue, TPunct>(result);
    }
}
