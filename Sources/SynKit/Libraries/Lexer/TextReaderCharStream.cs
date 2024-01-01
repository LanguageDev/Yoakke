// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.IO;
using Yoakke.Collections;
using Yoakke.Streams;
using Yoakke.SynKit.Text;

namespace Yoakke.SynKit.Lexer;

/// <summary>
/// An <see cref="ICharStream"/> that wraps a <see cref="TextReader"/>.
/// </summary>
public class TextReaderCharStream : ICharStream
{
    /// <summary>
    /// The underlying <see cref="TextReader"/>.
    /// </summary>
    public TextReader Underlying => this.SourceFile.Reader;

    /// <inheritdoc/>
    public Position Position { get; private set; }

    /// <inheritdoc/>
    public bool IsEnd => !this.TryPeek(out _);

    /// <inheritdoc/>
    public ISourceFile SourceFile => this.underlying;

    private readonly RingBuffer<char> peek = new();
    private char prevChar;
    private ISourceFile underlying;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextReaderCharStream"/> class.
    /// </summary>
    /// <param name="underlying">The unerlying <see cref="ISourceFile"/> to read from.</param>
    public TextReaderCharStream(ISourceFile underlying)
    {
        this.underlying = underlying;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextReaderCharStream"/> class.
    /// </summary>
    /// <param name="underlying">The unerlying <see cref="SourceFile"/> to read from.</param>
    public TextReaderCharStream(SourceFile underlying)
    {
        this.underlying = underlying;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextReaderCharStream"/> class.
    /// </summary>
    /// <param name="underlying">The unerlying <see cref="TextReader"/> to read from.</param>
    public TextReaderCharStream(TextReader underlying)
    {
        this.underlying = new SourceFile("<no-location>", underlying);
    }

    /// <inheritdoc/>
    public bool TryPeek(out char ch) => this.TryLookAhead(0, out ch);

    /// <inheritdoc/>
    public bool TryLookAhead(int offset, out char ch)
    {
        while (this.peek.Count <= offset)
        {
            var next = this.Underlying.Read();
            if (next == -1)
            {
                ch = default;
                return false;
            }
            this.peek.AddBack((char)next);
        }
        ch = this.peek[offset];
        return true;
    }

    /// <inheritdoc/>
    public bool TryConsume(out char ch)
    {
        if (!this.TryPeek(out ch)) return false;
        var current = this.peek.RemoveFront();
        this.Position = NextPosition(this.Position, this.prevChar, current);
        this.prevChar = current;
        return true;
    }

    /// <inheritdoc/>
    public int Consume(int amount) => StreamExtensions.Consume(this, amount);

    private static Position NextPosition(Position pos, char lastChar, char currentChar)
    {
        // Windows-style, already advanced line at \r
        if (lastChar == '\r' && currentChar == '\n') return pos;
        if (currentChar == '\r' || currentChar == '\n') return pos.Newline();
        if (char.IsControl(currentChar)) return pos;
        return pos.Advance();
    }

    /// <inheritdoc/>
    public void Defer(char item) => throw new NotSupportedException();
}
