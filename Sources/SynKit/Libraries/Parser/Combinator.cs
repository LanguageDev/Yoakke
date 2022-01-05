// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Yoakke.SynKit.Lexer;
using Yoakke.Streams;

namespace Yoakke.SynKit.Parser;

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
    /// Constructs a parser, that takes a single element from the input stream, if it matches the given one.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <param name="item">The item to match.</param>
    /// <returns>A new parser, that takes a single element from the input stream, if it equals <paramref name="item"/>.</returns>
    public static Parser<TItem, TItem> Item<TItem>(TItem item) =>
        (stream, offset) =>
        {
            if (stream.TryLookAhead(offset, out var got) && item!.Equals(got))
            {
                return ParseResult.Ok(item, offset + 1, null);
            }
            else
            {
              // TODO: Better error info?
              return ParseResult.Error(item!, got, offset, string.Empty);
            }
        };

    /// <summary>
    /// Constructs a parser, that takes a single character, if it matches a specified one.
    /// </summary>
    /// <param name="ch">The character to match.</param>
    /// <returns>A new parser, that tries to match a character with <paramref name="ch"/>.</returns>
    public static Parser<char, char> Char(char ch) => Item(ch);

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
    /// <typeparam name="TKind">The token kind type.</typeparam>
    /// <param name="kind">The token kind to match.</param>
    /// <returns>A new parser, that tries to match a token with kind <paramref name="kind"/>.</returns>
    public static Parser<IToken<TKind>, IToken<TKind>> Kind<TKind>(TKind kind)
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

    #region Alternatives

    /// <summary>
    /// Utility for constructing arbitrary amounts of alternatives. See
    /// <see cref="Alt{TItem, TResult}(Parser{TItem, TResult}, Parser{TItem, TResult})"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <typeparam name="TResult">The result type of the parsers.</typeparam>
    /// <param name="p1">The first alternative parser.</param>
    /// <returns>The combined alternative parsers.</returns>
    public static Parser<TItem, TResult> Alt<TItem, TResult>(Parser<TItem, TResult> p1) => p1;

    /// <summary>
    /// Utility for constructing arbitrary amounts of alternatives. See
    /// <see cref="Alt{TItem, TResult}(Parser{TItem, TResult}, Parser{TItem, TResult})"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <typeparam name="TResult">The result type of the parsers.</typeparam>
    /// <param name="p1">The first alternative parser.</param>
    /// <param name="p2">The second alternative parser.</param>
    /// <param name="p3">The third alternative parser.</param>
    /// <returns>The combined alternative parsers.</returns>
    public static Parser<TItem, TResult> Alt<TItem, TResult>(
        Parser<TItem, TResult> p1,
        Parser<TItem, TResult> p2,
        Parser<TItem, TResult> p3) => Alt(Alt(p1, p2), p3);

    /// <summary>
    /// Utility for constructing arbitrary amounts of alternatives. See
    /// <see cref="Alt{TItem, TResult}(Parser{TItem, TResult}, Parser{TItem, TResult})"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <typeparam name="TResult">The result type of the parsers.</typeparam>
    /// <param name="p1">The first alternative parser.</param>
    /// <param name="p2">The second alternative parser.</param>
    /// <param name="p3">The third alternative parser.</param>
    /// <param name="p4">The fourth alternative parser.</param>
    /// <returns>The combined alternative parsers.</returns>
    public static Parser<TItem, TResult> Alt<TItem, TResult>(
        Parser<TItem, TResult> p1,
        Parser<TItem, TResult> p2,
        Parser<TItem, TResult> p3,
        Parser<TItem, TResult> p4) => Alt(Alt(p1, p2, p3), p4);

    /// <summary>
    /// Utility for constructing arbitrary amounts of alternatives. See
    /// <see cref="Alt{TItem, TResult}(Parser{TItem, TResult}, Parser{TItem, TResult})"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <typeparam name="TResult">The result type of the parsers.</typeparam>
    /// <param name="p1">The first alternative parser.</param>
    /// <param name="p2">The second alternative parser.</param>
    /// <param name="p3">The third alternative parser.</param>
    /// <param name="p4">The fourth alternative parser.</param>
    /// <param name="p5">The fifth alternative parser.</param>
    /// <returns>The combined alternative parsers.</returns>
    public static Parser<TItem, TResult> Alt<TItem, TResult>(
        Parser<TItem, TResult> p1,
        Parser<TItem, TResult> p2,
        Parser<TItem, TResult> p3,
        Parser<TItem, TResult> p4,
        Parser<TItem, TResult> p5) => Alt(Alt(p1, p2, p3, p4), p5);

    /// <summary>
    /// Utility for constructing arbitrary amounts of alternatives. See
    /// <see cref="Alt{TItem, TResult}(Parser{TItem, TResult}, Parser{TItem, TResult})"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <typeparam name="TResult">The result type of the parsers.</typeparam>
    /// <param name="p1">The first alternative parser.</param>
    /// <param name="p2">The second alternative parser.</param>
    /// <param name="p3">The third alternative parser.</param>
    /// <param name="p4">The fourth alternative parser.</param>
    /// <param name="p5">The fifth alternative parser.</param>
    /// <param name="p6">The sixth alternative parser.</param>
    /// <returns>The combined alternative parsers.</returns>
    public static Parser<TItem, TResult> Alt<TItem, TResult>(
        Parser<TItem, TResult> p1,
        Parser<TItem, TResult> p2,
        Parser<TItem, TResult> p3,
        Parser<TItem, TResult> p4,
        Parser<TItem, TResult> p5,
        Parser<TItem, TResult> p6) => Alt(Alt(p1, p2, p3, p4, p5), p6);

    /// <summary>
    /// Utility for constructing arbitrary amounts of alternatives. See
    /// <see cref="Alt{TItem, TResult}(Parser{TItem, TResult}, Parser{TItem, TResult})"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <typeparam name="TResult">The result type of the parsers.</typeparam>
    /// <param name="p1">The first alternative parser.</param>
    /// <param name="p2">The second alternative parser.</param>
    /// <param name="p3">The third alternative parser.</param>
    /// <param name="p4">The fourth alternative parser.</param>
    /// <param name="p5">The fifth alternative parser.</param>
    /// <param name="p6">The sixth alternative parser.</param>
    /// <param name="p7">The seventh alternative parser.</param>
    /// <returns>The combined alternative parsers.</returns>
    public static Parser<TItem, TResult> Alt<TItem, TResult>(
        Parser<TItem, TResult> p1,
        Parser<TItem, TResult> p2,
        Parser<TItem, TResult> p3,
        Parser<TItem, TResult> p4,
        Parser<TItem, TResult> p5,
        Parser<TItem, TResult> p6,
        Parser<TItem, TResult> p7) => Alt(Alt(p1, p2, p3, p4, p5, p6), p7);

    /// <summary>
    /// Utility for constructing arbitrary amounts of alternatives. See
    /// <see cref="Alt{TItem, TResult}(Parser{TItem, TResult}, Parser{TItem, TResult})"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <typeparam name="TResult">The result type of the parsers.</typeparam>
    /// <param name="p1">The first alternative parser.</param>
    /// <param name="p2">The second alternative parser.</param>
    /// <param name="p3">The third alternative parser.</param>
    /// <param name="p4">The fourth alternative parser.</param>
    /// <param name="p5">The fifth alternative parser.</param>
    /// <param name="p6">The sixth alternative parser.</param>
    /// <param name="p7">The seventh alternative parser.</param>
    /// <param name="p8">The eight alternative parser.</param>
    /// <returns>The combined alternative parsers.</returns>
    public static Parser<TItem, TResult> Alt<TItem, TResult>(
        Parser<TItem, TResult> p1,
        Parser<TItem, TResult> p2,
        Parser<TItem, TResult> p3,
        Parser<TItem, TResult> p4,
        Parser<TItem, TResult> p5,
        Parser<TItem, TResult> p6,
        Parser<TItem, TResult> p7,
        Parser<TItem, TResult> p8) => Alt(Alt(p1, p2, p3, p4, p5, p6, p7), p8);

    #endregion

    #region Sequences

    /// <summary>
    /// Utility for constructing arbitrary length sequences. See
    /// <see cref="Seq{TItem, TLeftResult, TRightResult}(Parser{TItem, TLeftResult}, Parser{TItem, TRightResult})"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <param name="p1">The first sequenced parser.</param>
    /// <returns>The combined sequential parsers.</returns>
    public static Parser<TItem, T1> Seq<TItem, T1>(Parser<TItem, T1> p1) => p1;

    /// <summary>
    /// Utility for constructing arbitrary length sequences. See
    /// <see cref="Seq{TItem, TLeftResult, TRightResult}(Parser{TItem, TLeftResult}, Parser{TItem, TRightResult})"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <typeparam name="T3">The result type of the third parser.</typeparam>
    /// <param name="p1">The first sequenced parser.</param>
    /// <param name="p2">The second sequenced parser.</param>
    /// <param name="p3">The third sequenced parser.</param>
    /// <returns>The combined sequential parsers.</returns>
    public static Parser<TItem, (T1, T2, T3)> Seq<TItem, T1, T2, T3>(
        Parser<TItem, T1> p1,
        Parser<TItem, T2> p2,
        Parser<TItem, T3> p3) => Transform(
            Seq(Seq(p1, p2), p3),
            v => (v.Item1.Item1, v.Item1.Item2, v.Item2));

    /// <summary>
    /// Utility for constructing arbitrary length sequences. See
    /// <see cref="Seq{TItem, TLeftResult, TRightResult}(Parser{TItem, TLeftResult}, Parser{TItem, TRightResult})"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <typeparam name="T3">The result type of the third parser.</typeparam>
    /// <typeparam name="T4">The result type of the fourth parser.</typeparam>
    /// <param name="p1">The first sequenced parser.</param>
    /// <param name="p2">The second sequenced parser.</param>
    /// <param name="p3">The third sequenced parser.</param>
    /// <param name="p4">The fourth sequenced parser.</param>
    /// <returns>The combined sequential parsers.</returns>
    public static Parser<TItem, (T1, T2, T3, T4)> Seq<TItem, T1, T2, T3, T4>(
        Parser<TItem, T1> p1,
        Parser<TItem, T2> p2,
        Parser<TItem, T3> p3,
        Parser<TItem, T4> p4) => Transform(
            Seq(Seq(p1, p2, p3), p4),
            v => (v.Item1.Item1, v.Item1.Item2, v.Item1.Item3, v.Item2));

    /// <summary>
    /// Utility for constructing arbitrary length sequences. See
    /// <see cref="Seq{TItem, TLeftResult, TRightResult}(Parser{TItem, TLeftResult}, Parser{TItem, TRightResult})"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <typeparam name="T3">The result type of the third parser.</typeparam>
    /// <typeparam name="T4">The result type of the fourth parser.</typeparam>
    /// <typeparam name="T5">The result type of the fifth parser.</typeparam>
    /// <param name="p1">The first sequenced parser.</param>
    /// <param name="p2">The second sequenced parser.</param>
    /// <param name="p3">The third sequenced parser.</param>
    /// <param name="p4">The fourth sequenced parser.</param>
    /// <param name="p5">The fifth sequenced parser.</param>
    /// <returns>The combined sequential parsers.</returns>
    public static Parser<TItem, (T1, T2, T3, T4, T5)> Seq<TItem, T1, T2, T3, T4, T5>(
        Parser<TItem, T1> p1,
        Parser<TItem, T2> p2,
        Parser<TItem, T3> p3,
        Parser<TItem, T4> p4,
        Parser<TItem, T5> p5) => Transform(
            Seq(Seq(p1, p2, p3, p4), p5),
            v => (v.Item1.Item1, v.Item1.Item2, v.Item1.Item3, v.Item1.Item4, v.Item2));

    /// <summary>
    /// Utility for constructing arbitrary length sequences. See
    /// <see cref="Seq{TItem, TLeftResult, TRightResult}(Parser{TItem, TLeftResult}, Parser{TItem, TRightResult})"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <typeparam name="T3">The result type of the third parser.</typeparam>
    /// <typeparam name="T4">The result type of the fourth parser.</typeparam>
    /// <typeparam name="T5">The result type of the fifth parser.</typeparam>
    /// <typeparam name="T6">The result type of the sixth parser.</typeparam>
    /// <param name="p1">The first sequenced parser.</param>
    /// <param name="p2">The second sequenced parser.</param>
    /// <param name="p3">The third sequenced parser.</param>
    /// <param name="p4">The fourth sequenced parser.</param>
    /// <param name="p5">The fifth sequenced parser.</param>
    /// <param name="p6">The sixth sequenced parser.</param>
    /// <returns>The combined sequential parsers.</returns>
    public static Parser<TItem, (T1, T2, T3, T4, T5, T6)> Seq<TItem, T1, T2, T3, T4, T5, T6>(
        Parser<TItem, T1> p1,
        Parser<TItem, T2> p2,
        Parser<TItem, T3> p3,
        Parser<TItem, T4> p4,
        Parser<TItem, T5> p5,
        Parser<TItem, T6> p6) => Transform(
            Seq(Seq(p1, p2, p3, p4, p5), p6),
            v => (v.Item1.Item1, v.Item1.Item2, v.Item1.Item3, v.Item1.Item4, v.Item1.Item5, v.Item2));

    /// <summary>
    /// Utility for constructing arbitrary length sequences. See
    /// <see cref="Seq{TItem, TLeftResult, TRightResult}(Parser{TItem, TLeftResult}, Parser{TItem, TRightResult})"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <typeparam name="T3">The result type of the third parser.</typeparam>
    /// <typeparam name="T4">The result type of the fourth parser.</typeparam>
    /// <typeparam name="T5">The result type of the fifth parser.</typeparam>
    /// <typeparam name="T6">The result type of the sixth parser.</typeparam>
    /// <typeparam name="T7">The result type of the seventh parser.</typeparam>
    /// <param name="p1">The first sequenced parser.</param>
    /// <param name="p2">The second sequenced parser.</param>
    /// <param name="p3">The third sequenced parser.</param>
    /// <param name="p4">The fourth sequenced parser.</param>
    /// <param name="p5">The fifth sequenced parser.</param>
    /// <param name="p6">The sixth sequenced parser.</param>
    /// <param name="p7">The seventh sequenced parser.</param>
    /// <returns>The combined sequential parsers.</returns>
    public static Parser<TItem, (T1, T2, T3, T4, T5, T6, T7)> Seq<TItem, T1, T2, T3, T4, T5, T6, T7>(
        Parser<TItem, T1> p1,
        Parser<TItem, T2> p2,
        Parser<TItem, T3> p3,
        Parser<TItem, T4> p4,
        Parser<TItem, T5> p5,
        Parser<TItem, T6> p6,
        Parser<TItem, T7> p7) => Transform(
            Seq(Seq(p1, p2, p3, p4, p5, p6), p7),
            v => (v.Item1.Item1, v.Item1.Item2, v.Item1.Item3, v.Item1.Item4, v.Item1.Item5, v.Item1.Item6, v.Item2));

    /// <summary>
    /// Utility for constructing arbitrary length sequences. See
    /// <see cref="Seq{TItem, TLeftResult, TRightResult}(Parser{TItem, TLeftResult}, Parser{TItem, TRightResult})"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <typeparam name="T1">The result type of the first parser.</typeparam>
    /// <typeparam name="T2">The result type of the second parser.</typeparam>
    /// <typeparam name="T3">The result type of the third parser.</typeparam>
    /// <typeparam name="T4">The result type of the fourth parser.</typeparam>
    /// <typeparam name="T5">The result type of the fifth parser.</typeparam>
    /// <typeparam name="T6">The result type of the sixth parser.</typeparam>
    /// <typeparam name="T7">The result type of the seventh parser.</typeparam>
    /// <typeparam name="T8">The result type of the eight parser.</typeparam>
    /// <param name="p1">The first sequenced parser.</param>
    /// <param name="p2">The second sequenced parser.</param>
    /// <param name="p3">The third sequenced parser.</param>
    /// <param name="p4">The fourth sequenced parser.</param>
    /// <param name="p5">The fifth sequenced parser.</param>
    /// <param name="p6">The sixth sequenced parser.</param>
    /// <param name="p7">The seventh sequenced parser.</param>
    /// <param name="p8">The eight sequenced parser.</param>
    /// <returns>The combined sequential parsers.</returns>
    public static Parser<TItem, (T1, T2, T3, T4, T5, T6, T7, T8)> Seq<TItem, T1, T2, T3, T4, T5, T6, T7, T8>(
        Parser<TItem, T1> p1,
        Parser<TItem, T2> p2,
        Parser<TItem, T3> p3,
        Parser<TItem, T4> p4,
        Parser<TItem, T5> p5,
        Parser<TItem, T6> p6,
        Parser<TItem, T7> p7,
        Parser<TItem, T8> p8) => Transform(
            Seq(Seq(p1, p2, p3, p4, p5, p6, p7), p8),
            v => (v.Item1.Item1, v.Item1.Item2, v.Item1.Item3, v.Item1.Item4, v.Item1.Item5, v.Item1.Item6, v.Item1.Item7, v.Item2));

    #endregion
}
