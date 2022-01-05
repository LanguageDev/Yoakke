// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.Parser;

/// <summary>
/// Represents a successful parse.
/// </summary>
/// <typeparam name="T">The value of the parse.</typeparam>
public readonly struct ParseOk<T>
{
    /// <summary>
    /// The resulted parse value.
    /// </summary>
    public readonly T Value;

    /// <summary>
    /// The offset in the number of elements.
    /// </summary>
    public readonly int Offset;

    /// <summary>
    /// The furthest <see cref="ParseError"/> found so far.
    /// </summary>
    public readonly ParseError? FurthestError;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParseOk{T}"/> struct.
    /// </summary>
    /// <param name="value">The parsed value.</param>
    /// <param name="offset">The offset in the number of elements.</param>
    /// <param name="furthestError">The furthest <see cref="ParseError"/> found so far.</param>
    public ParseOk(T value, int offset, ParseError? furthestError = null)
    {
        this.Value = value;
        this.Offset = offset;
        this.FurthestError = furthestError;
    }

    /// <summary>
    /// Implicit conversion to extract the resulting value.
    /// </summary>
    /// <param name="ok">The <see cref="ParseOk{T}"/> to cast.</param>
    public static implicit operator T(ParseOk<T> ok) => ok.Value;

    /// <summary>
    /// Unifies two alternative <see cref="ParseOk{T}"/>s.
    /// </summary>
    /// <param name="first">The first ok to unify.</param>
    /// <param name="second">The second ok to unify.</param>
    /// <returns>The ok that got further.</returns>
    public static ParseOk<T> operator |(ParseOk<T> first, ParseOk<T> second)
    {
        var error = first.FurthestError | second.FurthestError;
        if (second.Offset > first.Offset) return new(second.Value, second.Offset, error);
        // NOTE: Even if they are equal, we return the first
        // It would be neat to return both
        return new(first.Value, first.Offset, error);
    }

    /// <summary>
    /// Unifies a <see cref="ParseOk{T}"/> with a <see cref="ParseError"/>.
    /// </summary>
    /// <param name="first">The ok to unify.</param>
    /// <param name="second">The error to unify.</param>
    /// <returns>The ok with extended error info.</returns>
    public static ParseOk<T> operator |(ParseOk<T> first, ParseError? second) =>
        new(first.Value, first.Offset, first.FurthestError | second);

    /// <summary>
    /// Unifies a <see cref="ParseOk{T}"/> with a <see cref="ParseError"/>.
    /// </summary>
    /// <param name="first">The error to unify.</param>
    /// <param name="second">The ok to unify.</param>
    /// <returns>The ok with extended error info.</returns>
    public static ParseOk<T> operator |(ParseError? first, ParseOk<T> second) => second | first;
}
