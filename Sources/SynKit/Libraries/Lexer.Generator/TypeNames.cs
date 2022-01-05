// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.Lexer.Generator;

/// <summary>
/// Common type-names so we don't have hardcoded strings in the source-generator.
/// </summary>
internal static class TypeNames
{
    private const string SynKitNamespace = "Yoakke.SynKit";
    private const string LexerNamespace = $"{SynKitNamespace}.Lexer";
    private const string StreamsNamespace = "Yoakke.Streams";

    /// <summary>
    /// System.InvalidOperationException.
    /// </summary>
    public const string InvalidOperationException = "System.InvalidOperationException";

    /// <summary>
    /// System.IO.TextReader.
    /// </summary>
    public const string TextReader = "System.IO.TextReader";

    /// <summary>
    /// System.IO.StringReader.
    /// </summary>
    public const string StringReader = "System.IO.StringReader";

    /// <summary>
    /// MaybeNullWhen attribute.
    /// </summary>
    public const string MaybeNullWhen = "System.Diagnostics.CodeAnalysis.MaybeNullWhen";

    /// <summary>
    /// The attribute that turns an enumeration into a generated lexer.
    /// </summary>
    public static readonly string LexerAttribute = $"{LexerNamespace}.Attributes.LexerAttribute";

    /// <summary>
    /// Annotates the field that should serve as the source character stream.
    /// </summary>
    public static readonly string CharSourceAttribute = $"{LexerNamespace}.Attributes.CharSourceAttribute";

    /// <summary>
    /// Annotates that a token type should be used as the end token.
    /// </summary>
    public static readonly string EndAttribute = $"{LexerNamespace}.Attributes.EndAttribute";

    /// <summary>
    /// Annotates that a token type should be used as the error - or unknown - token.
    /// </summary>
    public static readonly string ErrorAttribute = $"{LexerNamespace}.Attributes.ErrorAttribute";

    /// <summary>
    /// Annotates that a token should be skipped on match.
    /// </summary>
    public static readonly string IgnoreAttribute = $"{LexerNamespace}.Attributes.IgnoreAttribute";

    /// <summary>
    /// Annotates a token definition that is based on a regex.
    /// </summary>
    public static readonly string RegexAttribute = $"{LexerNamespace}.Attributes.RegexAttribute";

    /// <summary>
    /// Annotates a token definition that is based on a literal string.
    /// </summary>
    public static readonly string TokenAttribute = $"{LexerNamespace}.Attributes.TokenAttribute";

    /// <summary>
    /// The character-stream interface.
    /// </summary>
    public static readonly string ICharStream = $"{LexerNamespace}.ICharStream";

    /// <summary>
    /// A text-reader char stream implementation.
    /// </summary>
    public static readonly string TextReaderCharStream = $"{LexerNamespace}.TextReaderCharStream";

    /// <summary>
    /// The lexer interface.
    /// </summary>
    public static readonly string ILexer = $"{LexerNamespace}.ILexer";

    /// <summary>
    /// The stream interface.
    /// </summary>
    public static readonly string IStream = $"{StreamsNamespace}.IStream";

    /// <summary>
    /// The pre-defined token class.
    /// </summary>
    public static readonly string Token = $"{LexerNamespace}.Token";

    /// <summary>
    /// Text position type.
    /// </summary>
    public static readonly string Position = $"{SynKitNamespace}.Text.Position";
}
