// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.Streams;
using Yoakke.SynKit.Text;

namespace Yoakke.SynKit.Lexer;

/// <summary>
/// A general character stream to read from for lexers.
/// </summary>
public interface ICharStream : IPeekableStream<char>
{
    public ISourceFile SourceFile { get; }

    /// <summary>
    /// The current <see cref="Text.Position"/> the stream is at.
    /// </summary>
    public Position Position { get; }
}
