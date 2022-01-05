// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SynKit.Parser;

/// <summary>
/// Factory methods for <see cref="ParseResult{T}"/>.
/// </summary>
public static class ParseResult
{
    /// <summary>
    /// Constructs a <see cref="ParseOk{T}"/>.
    /// </summary>
    /// <typeparam name="T">The parsed value type.</typeparam>
    /// <param name="value">The parsed value.</param>
    /// <param name="offset">The offset in the number of tokens.</param>
    /// <param name="furthestError">The furthest advanced <see cref="ParseError"/>, if any.</param>
    /// <returns>The created <see cref="ParseOk{T}"/>.</returns>
    public static ParseOk<T> Ok<T>(T value, int offset, ParseError? furthestError = null) => new(value, offset, furthestError);

    /// <summary>
    /// Constructs a <see cref="ParseError"/>.
    /// </summary>
    /// <param name="expected">The expected element.</param>
    /// <param name="got">The token encountered instead.</param>
    /// <param name="position">The position where the error occured.</param>
    /// <param name="context">The rule context the error occurred in.</param>
    /// <returns>The created <see cref="ParseError"/>.</returns>
    public static ParseError Error(object expected, object? got, IComparable position, string context) =>
        new(expected, got, position, context);
}
