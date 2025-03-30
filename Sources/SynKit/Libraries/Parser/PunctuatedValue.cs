// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.Parser;

/// <summary>
/// A single, punctuated element.
/// </summary>
/// <typeparam name="TValue">The punctuated element type.</typeparam>
/// <typeparam name="TPunct">The punctuation element type.</typeparam>
public readonly struct PunctuatedValue<TValue, TPunct>
{
    /// <summary>
    /// The punctuated element.
    /// </summary>
    public readonly TValue Value;

    /// <summary>
    /// The punctuation that follows the element.
    /// </summary>
    public readonly TPunct? Punctuation;

    /// <summary>
    /// Initializes a new instance of the <see cref="PunctuatedValue{TValue, TPunct}"/> struct.
    /// </summary>
    /// <param name="element">The element that is punctuated.</param>
    /// <param name="punctuation">The punctuation element that follows the punctuated one.</param>
    public PunctuatedValue(TValue element, TPunct? punctuation)
    {
        this.Value = element;
        this.Punctuation = punctuation;
    }

    /// <summary>
    /// Deconstructs an object into its value and optional punctuation components.
    /// </summary>
    /// <param name="value">Represents the main value extracted from the object.</param>
    /// <param name="punct">Represents an optional punctuation element associated with the value.</param>
    public void Deconstruct(out TValue value, out TPunct? punct)
    {
        value = this.Value;
        punct = this.Punctuation!;
    }
}
