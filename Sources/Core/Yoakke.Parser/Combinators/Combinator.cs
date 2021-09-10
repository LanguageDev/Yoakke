// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Streams;

namespace Yoakke.Parser.Combinators
{
    /// <summary>
    /// Combinator extensions.
    /// </summary>
    public static class Combinator
    {
        #region Alternatives

        /// <summary>
        /// Parses an alternative of parsers.
        /// </summary>
        /// <typeparam name="TResult">The parse result type.</typeparam>
        /// <param name="p1">The first alternative parser.</param>
        /// <returns>The constructed parser.</returns>
        public static IParser<TResult> Alt<TResult>(IParser<TResult> p1) => p1;

        /// <summary>
        /// Parses an alternative of parsers.
        /// </summary>
        /// <typeparam name="TResult">The parse result type.</typeparam>
        /// <param name="p1">The first alternative parser.</param>
        /// <param name="p2">The second alternative parser.</param>
        /// <returns>The constructed parser.</returns>
        public static IParser<TResult> Alt<TResult>(IParser<TResult> p1, IParser<TResult> p2) =>
            new Alternative<TResult>(p1, p2);

        /// <summary>
        /// Parses an alternative of parsers.
        /// </summary>
        /// <typeparam name="TResult">The parse result type.</typeparam>
        /// <param name="p1">The first alternative parser.</param>
        /// <param name="p2">The second alternative parser.</param>
        /// <param name="p3">The third alternative parser.</param>
        /// <returns>The constructed parser.</returns>
        public static IParser<TResult> Alt<TResult>(IParser<TResult> p1, IParser<TResult> p2, IParser<TResult> p3) =>
            Alt(Alt(p1, p2), p3);

        /// <summary>
        /// Parses an alternative of parsers.
        /// </summary>
        /// <typeparam name="TResult">The parse result type.</typeparam>
        /// <param name="p1">The first alternative parser.</param>
        /// <param name="p2">The second alternative parser.</param>
        /// <param name="p3">The third alternative parser.</param>
        /// <param name="p4">The fourth alternative parser.</param>
        /// <returns>The constructed parser.</returns>
        public static IParser<TResult> Alt<TResult>(IParser<TResult> p1, IParser<TResult> p2, IParser<TResult> p3, IParser<TResult> p4) =>
            Alt(Alt(p1, p2, p3), p4);

        /// <summary>
        /// Parses an alternative of parsers.
        /// </summary>
        /// <typeparam name="TResult">The parse result type.</typeparam>
        /// <param name="p1">The first alternative parser.</param>
        /// <param name="p2">The second alternative parser.</param>
        /// <param name="p3">The third alternative parser.</param>
        /// <param name="p4">The fourth alternative parser.</param>
        /// <param name="p5">The fifth alternative parser.</param>
        /// <returns>The constructed parser.</returns>
        public static IParser<TResult> Alt<TResult>(IParser<TResult> p1, IParser<TResult> p2, IParser<TResult> p3, IParser<TResult> p4, IParser<TResult> p5) =>
            Alt(Alt(p1, p2, p3, p4), p5);

        #endregion

        #region Sequences

        /// <summary>
        /// Parses a sequence of parsers.
        /// </summary>
        /// <typeparam name="TResult">The parse result type.</typeparam>
        /// <param name="p1">The first sequenced parser.</param>
        /// <returns>The constructed parser.</returns>
        public static IParser<TResult> Seq<TResult>(IParser<TResult> p1) => p1;

        /// <summary>
        /// Parses a sequence of parsers.
        /// </summary>
        /// <typeparam name="T1">The first parse result type.</typeparam>
        /// <typeparam name="T2">The second parse result type.</typeparam>
        /// <param name="p1">The first sequenced parser.</param>
        /// <param name="p2">The second sequenced parser.</param>
        /// <returns>The constructed parser.</returns>
        public static IParser<(T1, T2)> Seq<T1, T2>(IParser<T1> p1, IParser<T2> p2) => new Sequence<T1, T2>(p1, p2);

        /// <summary>
        /// Parses a sequence of parsers.
        /// </summary>
        /// <typeparam name="T1">The first parse result type.</typeparam>
        /// <typeparam name="T2">The second parse result type.</typeparam>
        /// <typeparam name="T3">The third parse result type.</typeparam>
        /// <param name="p1">The first sequenced parser.</param>
        /// <param name="p2">The second sequenced parser.</param>
        /// <param name="p3">The third sequenced parser.</param>
        /// <returns>The constructed parser.</returns>
        public static IParser<(T1, T2, T3)> Seq<T1, T2, T3>(IParser<T1> p1, IParser<T2> p2, IParser<T3> p3) =>
            Transform(Seq(Seq(p1, p2), p3), i => (i.Item1.Item1, i.Item1.Item2, i.Item2));

        /// <summary>
        /// Parses a sequence of parsers.
        /// </summary>
        /// <typeparam name="T1">The first parse result type.</typeparam>
        /// <typeparam name="T2">The second parse result type.</typeparam>
        /// <typeparam name="T3">The third parse result type.</typeparam>
        /// <typeparam name="T4">The fourth parse result type.</typeparam>
        /// <param name="p1">The first sequenced parser.</param>
        /// <param name="p2">The second sequenced parser.</param>
        /// <param name="p3">The third sequenced parser.</param>
        /// <param name="p4">The fourth sequenced parser.</param>
        /// <returns>The constructed parser.</returns>
        public static IParser<(T1, T2, T3, T4)> Seq<T1, T2, T3, T4>(IParser<T1> p1, IParser<T2> p2, IParser<T3> p3, IParser<T4> p4) =>
            Transform(Seq(Seq(p1, p2, p3), p4), i => (i.Item1.Item1, i.Item1.Item2, i.Item1.Item3, i.Item2));

        /// <summary>
        /// Parses a sequence of parsers.
        /// </summary>
        /// <typeparam name="T1">The first parse result type.</typeparam>
        /// <typeparam name="T2">The second parse result type.</typeparam>
        /// <typeparam name="T3">The third parse result type.</typeparam>
        /// <typeparam name="T4">The fourth parse result type.</typeparam>
        /// <typeparam name="T5">The fifth parse result type.</typeparam>
        /// <param name="p1">The first sequenced parser.</param>
        /// <param name="p2">The second sequenced parser.</param>
        /// <param name="p3">The third sequenced parser.</param>
        /// <param name="p4">The fourth sequenced parser.</param>
        /// <param name="p5">The fifth sequenced parser.</param>
        /// <returns>The constructed parser.</returns>
        public static IParser<(T1, T2, T3, T4, T5)> Seq<T1, T2, T3, T4, T5>(IParser<T1> p1, IParser<T2> p2, IParser<T3> p3, IParser<T4> p4, IParser<T5> p5) =>
            Transform(Seq(Seq(p1, p2, p3, p4), p5), i => (i.Item1.Item1, i.Item1.Item2, i.Item1.Item3, i.Item1.Item4, i.Item2));

        #endregion

        /// <summary>
        /// Creates an optional of a parser.
        /// </summary>
        /// <typeparam name="TResult">The parsed type.</typeparam>
        /// <param name="p">The parser.</param>
        /// <returns>The transformed parser.</returns>
        public static IParser<TResult?> Opt<TResult>(IParser<TResult> p) => new Optional<TResult>(p);

        /// <summary>
        /// Creates a 0 of more repetitions a parser.
        /// </summary>
        /// <typeparam name="TResult">The parsed type.</typeparam>
        /// <param name="p">The parser.</param>
        /// <returns>The transformed parser.</returns>
        public static IParser<IReadOnlyList<TResult>> Rep0<TResult>(IParser<TResult> p) => new Repeat0<TResult>(p);

        /// <summary>
        /// Creates a 1 of more repetitions a parser.
        /// </summary>
        /// <typeparam name="TResult">The parsed type.</typeparam>
        /// <param name="p">The parser.</param>
        /// <returns>The transformed parser.</returns>
        public static IParser<IReadOnlyList<TResult>> Rep1<TResult>(IParser<TResult> p) => new Repeat1<TResult>(p);

        /// <summary>
        /// Creates a parser from a function.
        /// </summary>
        /// <typeparam name="TItem">The item type of the parse stream.</typeparam>
        /// <typeparam name="TResult">The parsed type.</typeparam>
        /// <param name="f">The parser function.</param>
        /// <returns>The created parser.</returns>
        public static IParser<TResult> Call<TItem, TResult>(Func<IPeekableStream<TItem>, int, ParseResult<TResult>> f) =>
            new Call<TItem, TResult>(f);

        /// <summary>
        /// Creates a filter function.
        /// </summary>
        /// <typeparam name="TResult">The parsed type.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="predicate">The predicate to filter with.</param>
        /// <returns>The transformed parser.</returns>
        public static IParser<TResult> Filter<TResult>(IParser<TResult> parser, Predicate<TResult> predicate) =>
            new Filter<TResult>(parser, predicate);

        /// <summary>
        /// Creates a transformation of a parser.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <typeparam name="TParsed">The parsed type.</typeparam>
        /// <param name="p">The parser.</param>
        /// <param name="transformer">The transformer.</param>
        /// <returns>The transformed parser.</returns>
        public static IParser<TResult> Transform<TResult, TParsed>(IParser<TParsed> p, Func<TParsed, TResult> transformer) =>
            new Transform<TParsed, TResult>(p, transformer);

        /// <summary>
        /// Default implementation for <see cref="IParser{TResult}.Parse{TItem}(IPeekableStream{TItem})"/>.
        /// </summary>
        /// <typeparam name="TResult">The parsed value type.</typeparam>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <param name="parser">The parser to parse with.</param>
        /// <param name="stream">The stream to parse from.</param>
        /// <returns>The result of the parse.</returns>
        public static ParseResult<TResult> Parse<TResult, TItem>(IParser<TResult> parser, IPeekableStream<TItem> stream)
        {
            var result = parser.Parse(stream);
            if (result.IsOk) stream.Consume(result.Ok.Offset);
            return result;
        }
    }
}
