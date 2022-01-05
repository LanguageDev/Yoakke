// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.Parser.Generator;

/// <summary>
/// Common type-names so we don't have hardcoded strings in the source-generator.
/// </summary>
internal static class TypeNames
{
    private const string SynKitNamespace = "Yoakke.SynKit";
    private const string ParserNamespace = $"{SynKitNamespace}.Parser";
    private const string LexerNamespace = $"{SynKitNamespace}.Lexer";

    /// <summary>
    /// System.Collections.Generic.IReadOnlyList.
    /// </summary>
    public const string IReadOnlyList = "System.Collections.Generic.IReadOnlyList";

    /// <summary>
    /// System.Collections.Generic.IList.
    /// </summary>
    public const string IList = "System.Collections.Generic.IList";

    /// <summary>
    /// System.Collections.Generic.List.
    /// </summary>
    public const string List = "System.Collections.Generic.List";

    /// <summary>
    /// System.Collections.Generic.IEnumerable.
    /// </summary>
    public const string IEnumerable = "System.Collections.Generic.IEnumerable";

    /// <summary>
    /// System.Diagnostics.CodeAnalysis.MaybeNullWhen.
    /// </summary>
    public const string MaybeNullWhen = "System.Diagnostics.CodeAnalysis.MaybeNullWhen";

    /// <summary>
    /// The attribute that makes the Source Generator look for rule annotations in a class.
    /// </summary>
    public static readonly string ParserAttribute = $"{ParserNamespace}.Attributes.ParserAttribute";

    /// <summary>
    /// The attribute to mark a member as a source of tokens.
    /// </summary>
    public static readonly string TokenSourceAttribute = $"{ParserNamespace}.Attributes.TokenSourceAttribute";

    /// <summary>
    /// The annotation for a grammar rule over the transformer method.
    /// </summary>
    public static readonly string RuleAttribute = $"{ParserNamespace}.Attributes.RuleAttribute";

    /// <summary>
    /// Annotation for a left-associative precedence level.
    /// </summary>
    public static readonly string LeftAttribute = $"{ParserNamespace}.Attributes.LeftAttribute";

    /// <summary>
    /// Annotation for a right-associative precedence level.
    /// </summary>
    public static readonly string RightAttribute = $"{ParserNamespace}.Attributes.RightAttribute";

    /// <summary>
    /// Parse error type.
    /// </summary>
    public static readonly string ParseError = $"{ParserNamespace}.ParseError";

    /// <summary>
    /// Parse result type.
    /// </summary>
    public static readonly string ParseResult = $"{ParserNamespace}.ParseResult";

    /// <summary>
    /// Token interface type.
    /// </summary>
    public static readonly string IToken = $"{LexerNamespace}.IToken";

    /// <summary>
    /// Stream type.
    /// </summary>
    public static readonly string IStream = "Yoakke.Streams.IStream";

    /// <summary>
    /// Peekable stream type.
    /// </summary>
    public static readonly string IPeekableStream = "Yoakke.Streams.IPeekableStream";

    /// <summary>
    /// Enumerable stream adapter.
    /// </summary>
    public static readonly string EnumerableStream = "Yoakke.Streams.EnumerableStream";

    /// <summary>
    /// Lexer interface type.
    /// </summary>
    public static readonly string ILexer = $"{LexerNamespace}.ILexer";
}
