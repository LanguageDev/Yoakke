// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.Streams;
using Yoakke.Text;

namespace Yoakke.Lexer;

/// <summary>
/// A general character stream to read from for lexers.
/// </summary>
public interface ICharStream : IPeekableStream<char>
{
  /// <summary>
  /// The current <see cref="Text.Position"/> the stream is at.
  /// </summary>
  public Position Position { get; }
}
