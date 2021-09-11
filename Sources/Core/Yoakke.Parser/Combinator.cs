// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Lexer;
using Yoakke.Streams;

namespace Yoakke.Parser
{
    /// <summary>
    /// Parser combinator logic implementation.
    /// </summary>
    public static class Combinator
    {
        /// <summary>
        /// Represents a generic parser function. Any function can be used for parsing, that conforms this signature.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <typeparam name="TResult">The parsed value type.</typeparam>
        /// <param name="stream">The stream to parse from.</param>
        /// <param name="offset">The offset to start parsing from.</param>
        /// <returns>The result of the parse.</returns>
        public delegate ParseResult<TResult> Parser<TItem, TResult>(IPeekableStream<TItem> stream, int offset);

        /// <summary>
        /// Applies a parser to some input, consuming the parsed tokens on success.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <typeparam name="TResult">The parsed value type.</typeparam>
        /// <param name="parser">The parser to apply.</param>
        /// <param name="stream">The stream to parse from.</param>
        /// <returns>The result of parsing with <paramref name="parser"/> on <paramref name="stream"/>.</returns>
        public static ParseResult<TResult> Parse<TItem, TResult>(this Parser<TItem, TResult> parser, IPeekableStream<TItem> stream)
        {
            var result = parser(stream, 0);
            if (result.IsOk) stream.Consume(result.Ok.Offset);
            return result;
        }

        #region Core Combinators

        /// <summary>
        /// Constructs a parser, that takes a single element from the input stream.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <returns>A new parser, that takes a single element from the input stream, if there is any.</returns>
        public static Parser<TItem, TItem> Item<TItem>() =>
            (stream, offset) =>
            {
                if (stream.TryLookAhead(offset, out var item)) return ParseResult.Ok(item, offset + 1, null);
                // TODO: Better error info?
                else return ParseResult.Error("item", null, offset, string.Empty);
            };

        /// <summary>
        /// Constructs a parser, that takes a single character, if it matches a specified one.
        /// </summary>
        /// <param name="ch">The character to match.</param>
        /// <returns>A new parser, that tries to match a character with <paramref name="ch"/>.</returns>
        public static Parser<char, char> Char(char ch) =>
            (stream, offset) =>
            {
                if (stream.TryLookAhead(offset, out var c) && c == ch) return ParseResult.Ok(ch, offset + 1, null);
                // TODO: Better error info?
                else return ParseResult.Error(ch.ToString(), c, offset, string.Empty);
            };

        /// <summary>
        /// Constructs a parser, that takes a single token, if its text matches the specified one.
        /// </summary>
        /// <param name="text">The text to match.</param>
        /// <returns>A new parser, that tries to match a token with text <paramref name="text"/>.</returns>
        public static Parser<IToken, IToken> Text(string text) =>
            (stream, offset) =>
            {
                if (stream.TryLookAhead(offset, out var token) && token.Text == text)
                {
                    return ParseResult.Ok(token, offset + 1, null);
                }
                else
                {
                    // TODO: Better error info?
                    return ParseResult.Error(text, token, offset, string.Empty);
                }
            };

        /// <summary>
        /// Constructs a parser, that takes a single token, if its kind matches the specified one.
        /// </summary>
        /// <param name="kind">The token kind to match.</param>
        /// <returns>A new parser, that tries to match a token with kind <paramref name="kind"/>.</returns>
        public static Parser<IToken<TKind>, IToken<TKind>> Text<TKind>(TKind kind)
            where TKind : notnull =>
            (stream, offset) =>
            {
                if (stream.TryLookAhead(offset, out var token) && token.Kind.Equals(kind))
                {
                    return ParseResult.Ok(token, offset + 1, null);
                }
                else
                {
                    // TODO: Better error info?
                    return ParseResult.Error(kind, token, offset, string.Empty);
                }
            };

        /// <summary>
        /// Constructs an alternative of parsers.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <typeparam name="TResult">The parsed value type.</typeparam>
        /// <param name="left">The first alternative parser to combine.</param>
        /// <param name="right">The second alternative parser to combine.</param>
        /// <returns>A new parser that tries both <paramref name="left"/> and <paramref name="right"/>, and returns
        /// the succeeding result that got further.</returns>
        public static Parser<TItem, TResult> Alt<TItem, TResult>(Parser<TItem, TResult> left, Parser<TItem, TResult> right) =>
            (stream, offset) => left(stream, offset) | right(stream, offset);

        /// <summary>
        /// Constructs a sequence of parsers.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <typeparam name="TLeftResult">The parsed value type of <paramref name="left"/>.</typeparam>
        /// <typeparam name="TRightResult">The parsed value type of <paramref name="right"/>.</typeparam>
        /// <param name="left">The first parser to combine.</param>
        /// <param name="right">The second parser to combine.</param>
        /// <returns>A new parser that first tries to parse <paramref name="left"/>, and if that succeeds,
        /// parses <paramref name="right"/>, and returns their combined results in a tuple.</returns>
        public static Parser<TItem, (TLeftResult, TRightResult)> Seq<TItem, TLeftResult, TRightResult>(
            Parser<TItem, TLeftResult> left,
            Parser<TItem, TRightResult> right) =>
            (stream, offset) =>
            {
                // Invoke first
                var first = left(stream, offset);
                if (first.IsError) return first.Error;
                var firstOk = first.Ok;
                // Invoke second
                var second = right(stream, firstOk.Offset);
                if (second.IsError) return (firstOk.FurthestError | second.Error)!;
                var secondOk = second.Ok;
                // Combine
                var value = (firstOk.Value, secondOk.Value);
                var error = firstOk.FurthestError | secondOk.FurthestError;
                return ParseResult.Ok(value, secondOk.Offset, error);
            };

        /// <summary>
        /// Constructs a parser for 0 or more repetitions of an element.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <typeparam name="TResult">The parsed value type.</typeparam>
        /// <param name="element">The repeated element parser.</param>
        /// <returns>A new parser that always succeeds, and parses <paramref name="element"/> as many times, as it
        /// succeeds. The results are collected into a list.</returns>
        public static Parser<TItem, IReadOnlyList<TResult>> Rep0<TItem, TResult>(Parser<TItem, TResult> element) =>
            (stream, offset) =>
            {
                var result = new List<TResult>();
                ParseError? error = null;
                while (true)
                {
                    var item = element(stream, offset);
                    error |= item.FurthestError;
                    if (item.IsError) break;
                    result.Add(item.Ok.Value);
                    offset = item.Ok.Offset;
                }
                return ParseResult.Ok(result as IReadOnlyList<TResult>, offset, error);
            };

        /// <summary>
        /// Constructs a parser for 1 or more repetitions of an element.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <typeparam name="TResult">The parsed value type.</typeparam>
        /// <param name="element">The repeated element parser.</param>
        /// <returns>A new parser that succeeds as long as it matches at least one element, and continues as many times, as it
        /// succeeds. The results are collected into a list.</returns>
        public static Parser<TItem, IReadOnlyList<TResult>> Rep1<TItem, TResult>(Parser<TItem, TResult> element)
        {
            var rep0 = Rep0(element);
            return (stream, offset) =>
            {
                var result = rep0(stream, offset);
                return result.Ok.Value.Count == 0
                    ? result.FurthestError!
                    : result.Ok;
            };
        }

        /// <summary>
        /// Constructs a parser that optionally parses an element.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <typeparam name="TResult">The parsed value type.</typeparam>
        /// <param name="element">The optional element parser.</param>
        /// <returns>A new parser that always succeeds and includes the succeeding value, of the element parser succeeded.</returns>
        public static Parser<TItem, TResult?> Opt<TItem, TResult>(Parser<TItem, TResult> element) =>
            (stream, offset) =>
            {
                var result = element(stream, offset);
                if (result.IsOk) return result!;
                return ParseResult.Ok(default(TResult?), offset, result.FurthestError);
            };

        /// <summary>
        /// Constructs a parser that trasforms the result, in case the subelement parser succeeds.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <typeparam name="TParsed">The parsed value type.</typeparam>
        /// <typeparam name="TResult">The transformed value type.</typeparam>
        /// <param name="element">The element parser.</param>
        /// <param name="transformer">The transformer function.</param>
        /// <returns>A new parser that invokes the <paramref name="element"/> parsed, and transforms the result, if it
        /// succeeds, using <paramref name="transformer"/>.</returns>
        public static Parser<TItem, TResult> Transform<TItem, TParsed, TResult>(
            Parser<TItem, TParsed> element,
            Func<TParsed, TResult> transformer) =>
            (stream, offset) =>
            {
                var result = element(stream, offset);
                if (result.IsError) return result.Error;
                return ParseResult.Ok(transformer(result.Ok.Value), result.Ok.Offset, result.Ok.FurthestError);
            };

        #endregion
    }
}
