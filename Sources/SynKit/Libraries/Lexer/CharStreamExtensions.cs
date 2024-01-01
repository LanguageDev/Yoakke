// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Text;
using Yoakke.Streams;

namespace Yoakke.SynKit.Lexer;

/// <summary>
/// Extensions for <see cref="ICharStream"/>.
/// </summary>
public static class CharStreamExtensions
{
    /// <summary>
    /// Peeks ahead a character into the input.
    /// </summary>
    /// <param name="stream">The stream to peek.</param>
    /// <param name="default">The default character to return if the end has been reached.</param>
    /// <returns>The peeked character, or default if the end has been reached.</returns>
    public static char Peek(this ICharStream stream, char @default) =>
        stream.TryPeek(out var ch) ? ch : @default;

    /// <summary>
    /// Peeks ahead some characters into the input.
    /// </summary>
    /// <param name="stream">The stream to peek.</param>
    /// <param name="offset">The amount to peek forward. 0 means next character.</param>
    /// <param name="default">The default character to return if the end has been reached.</param>
    /// <returns>The peeked character, or default if the end has been reached.</returns>
    public static char LookAhead(this ICharStream stream, int offset, char @default) =>
        stream.TryLookAhead(offset, out var ch) ? ch : @default;

    /// <summary>
    /// Checks, if some upcoming text matches a given string.
    /// </summary>
    /// <param name="stream">The stream to check the match for.</param>
    /// <param name="text">The string to compare with the upcoming text.</param>
    /// <param name="offset">The offset to start the match at in the input.</param>
    /// <returns>True, if there is a full match.</returns>
    public static bool Matches(this ICharStream stream, string text, int offset = 0) =>
        stream.Matches(text.AsSpan(), offset);

    /// <summary>
    /// Checks, if some upcoming text matches a given string.
    /// </summary>
    /// <param name="stream">The stream to check the match for.</param>
    /// <param name="text">The string to compare with the upcoming text.</param>
    /// <param name="offset">The offset to start the match at in the input.</param>
    /// <returns>True, if there is a full match.</returns>
    public static bool Matches(this ICharStream stream, ReadOnlySpan<char> text, int offset = 0)
    {
        // To avoid peeking -1, we pre-check empty string
        if (text.Length == 0) return true;
        // Check if we even have enough characters
        if (!stream.TryLookAhead(offset + text.Length - 1, out _)) return false;
        // If so, we can do a linear match without the overhead of the peek call
        for (var i = 0; i < text.Length; ++i)
        {
            if (text[i] != stream.LookAhead(offset + i)) return false;
        }
        return true;
    }

    /// <summary>
    /// Checks, if some upcoming character matches a given one.
    /// </summary>
    /// <param name="stream">The stream to check the match for.</param>
    /// <param name="ch">The character to compare with the upcoming one.</param>
    /// <param name="offset">The offset to start look for match at in the input.</param>
    /// <returns>True, if they match.</returns>
    public static bool Matches(this ICharStream stream, char ch, int offset = 0) =>
        stream.TryLookAhead(offset, out var inInput) && ch == inInput;

    /// <summary>
    /// Consumes characters in the input and builds a string from the consumed characters.
    /// </summary>
    /// <param name="stream">The stream to consume characters in.</param>
    /// <param name="length">The amount of characters to skip.</param>
    /// <returns>The concatenated characters as a string.</returns>
    public static string ConsumeText(this ICharStream stream, int length) => stream.ConsumeText(length, out _);

    /// <summary>
    /// Consumes characters in the input and builds a string from the consumed characters.
    /// </summary>
    /// <param name="stream">The stream to consume characters in.</param>
    /// <param name="length">The amount of characters to skip.</param>
    /// <param name="range">The range of the text.</param>
    /// <returns>The concatenated characters as a string.</returns>
    public static string ConsumeText(this ICharStream stream, int length, out Text.Range range)
    {
        var result = new StringBuilder();
        var start = stream.Position;
        for (var i = 0; i < length; ++i)
        {
            if (!stream.TryConsume(out var ch)) break;
            result.Append(ch);
        }
        range = new Text.Range(start, stream.Position);
        return result.ToString();
    }

    /// <summary>
    /// Consumes characters in the input and builds a <see cref="IToken{T}"/> with a given factory function.
    /// </summary>
    /// <typeparam name="TToken">The exact type of the constructed <see cref="IToken"/>.</typeparam>
    /// <param name="stream">The stream to consume characters in.</param>
    /// <param name="length">The amount of characters to skip.</param>
    /// <param name="makeToken">The factory function that receives the source <see cref="Text.Range"/> of the skipped characters
    /// and the skipped characters themselves concatenated as a string, and produces an <see cref="IToken"/> from them.</param>
    /// <returns>The constructed <see cref="IToken{TKind}"/> returned from <paramref name="makeToken"/>.</returns>
    public static TToken ConsumeToken<TToken>(this ICharStream stream, int length, Func<Text.Location, string, TToken> makeToken)
    {
        var start = stream.Position;
        var text = length == 0 ? string.Empty : stream.ConsumeText(length);
        var range = new Text.Range(start, stream.Position);
        return makeToken(new Text.Location(stream.SourceFile, range), text);
    }

    /// <summary>
    /// Consumes characters in the input and builds a <see cref="Token{TKind}"/> from the skipped characters.
    /// </summary>
    /// <typeparam name="TKind">The kind-type for the produced <see cref="Token{TKind}"/>.</typeparam>
    /// <param name="stream">The stream to consume characters in.</param>
    /// <param name="kind">The token-kind to build.</param>
    /// <param name="length">The amount of characters to skip.</param>
    /// <returns>The constructed <see cref="Token{TKind}"/>.</returns>
    public static Token<TKind> ConsumeToken<TKind>(this ICharStream stream, TKind kind, int length)
        where TKind : notnull =>
        stream.ConsumeToken(length, (location, text) => new Token<TKind>(location.Range, location, text, kind));
}
