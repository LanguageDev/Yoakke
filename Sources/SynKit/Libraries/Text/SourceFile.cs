// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.SynKit.Text;

/// <summary>
/// A source file implementation with <see cref="TextReader"/>s.
/// </summary>
public class SourceFile : TextReader, ISourceFile
{
    /// <inheritdoc/>
    public string Path { get; }

    /// <inheritdoc/>
    public TextReader Reader => this.underlying;

    /// <inheritdoc/>
    public int AvailableLines => this.lineStarts.Count;

    // The reader we read from
    private readonly TextReader underlying;
    // The buffer we read into
    private StringBuilder sourceText;
    // The offset we are at in the buffer
    private int index;
    // The line start indices
    private List<int> lineStarts;
    // If we have been disposed already
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceFile"/> class.
    /// </summary>
    /// <param name="path">The path of the file. This does not actually have to exist, it's symbolic.</param>
    /// <param name="underlying">The underlying <see cref="TextReader"/> to read the text from.</param>
    public SourceFile(string path, TextReader underlying)
    {
        this.Path = path;
        this.underlying = underlying;
        this.sourceText = new StringBuilder();
        this.lineStarts = new List<int>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SourceFile"/> class.
    /// </summary>
    /// <param name="path">The path of the file. This does not actually have to exist, it's symbolic.</param>
    /// <param name="source">The source text itself.</param>
    public SourceFile(string path, string source)
        : this(path, new StringReader(source))
    {
        while (this.ReadNextLine())
        {
        }
        this.index = 0;

        // this is to reset state of the reader to the "initial".
        // I resort to this hack, since constructor from string pre-scan lines, but did not reset.
        this.underlying = new StringReader(source);
    }

    /// <inheritdoc/>
    public string GetLine(int index)
    {
        if (!this.EnsureLineCount(index + 1)) return string.Empty;
        var from = this.lineStarts[index];
        if (index + 1 >= this.lineStarts.Count) return this.sourceText.ToString(from, this.sourceText.Length - from);
        else return this.sourceText.ToString(from, this.lineStarts[index + 1] - from);
    }

    /// <inheritdoc/>
    public override void Close() => this.underlying.Close();

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (this.disposed) return;
        if (disposing)
        {
            this.underlying.Dispose();
            this.sourceText.Clear();
            this.lineStarts.Clear();
            this.sourceText = null!;
            this.lineStarts = null!;
        }
        this.disposed = true;
    }

    /// <inheritdoc/>
    public override int Peek()
    {
        if (!this.EnsureLength(this.index + 1)) return -1;
        return this.sourceText[this.index];
    }

    /// <inheritdoc/>
    public override int Read()
    {
        var ch = this.Peek();
        if (ch != -1) this.index += 1;
        return ch;
    }

    /// <inheritdoc/>
    public override int Read(char[] buffer, int index, int count)
    {
        this.EnsureLength(this.index + count);
        var readUntil = Math.Min(this.index + count, this.sourceText.Length);
        var ableToRead = readUntil - this.index;
        this.sourceText.CopyTo(this.index, buffer, index, ableToRead);
        this.index += ableToRead;
        return ableToRead;
    }

    /// <inheritdoc/>
    public override Task<int> ReadAsync(char[] buffer, int index, int count) =>
        Task.FromResult(this.Read(buffer, index, count));

    /// <inheritdoc/>
    public override int ReadBlock(char[] buffer, int index, int count) => this.Read(buffer, index, count);

    /// <inheritdoc/>
    public override Task<int> ReadBlockAsync(char[] buffer, int index, int count) =>
        Task.FromResult(this.Read(buffer, index, count));

    /// <inheritdoc/>
    public override string? ReadLine()
    {
        if (this.index == this.sourceText.Length && !this.ReadNextLine()) return null;
        var lineIndex = this.lineStarts.BinarySearch(this.index);
        if (lineIndex < 0) lineIndex = -lineIndex - 1;
        string result;
        if (lineIndex + 1 >= this.lineStarts.Count) result = this.sourceText.ToString(this.lineStarts[lineIndex], this.sourceText.Length - this.lineStarts[lineIndex]);
        else result = this.sourceText.ToString(this.lineStarts[lineIndex], this.lineStarts[lineIndex + 1] - this.lineStarts[lineIndex]);
        this.index += result.Length;
        return result;
    }

    /// <inheritdoc/>
    public override Task<string?> ReadLineAsync() => Task.FromResult(this.ReadLine());

    /// <inheritdoc/>
    public override string ReadToEnd()
    {
        var wasAt = this.index;
        while (this.ReadNextLine())
        {
        }
        this.index = this.sourceText.Length;
        return this.sourceText.ToString(wasAt, this.index - wasAt);
    }

    /// <inheritdoc/>
    public override Task<string> ReadToEndAsync() => Task.FromResult(this.ReadToEnd());

    private bool EnsureLineCount(int count)
    {
        while (this.lineStarts.Count < count)
        {
            var line = this.underlying.ReadLine();
            if (line == null) return false;
            // There was a line to read
            this.lineStarts.Add(this.sourceText.Length);
            this.sourceText.AppendLine(line);
        }
        return true;
    }

    private bool ReadNextLine() => this.EnsureLineCount(this.lineStarts.Count + 1);

    private bool EnsureLength(int length)
    {
        while (this.sourceText.Length < length)
        {
            if (!this.ReadNextLine()) return false;
        }
        return true;
    }
}
