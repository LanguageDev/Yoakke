// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yoakke.Text
{
    /// <summary>
    /// A source file implementation with <see cref="TextReader"/>s.
    /// </summary>
    public class SourceFile : TextReader, ISourceFile
    {
        public string Path { get; }

        public TextReader Reader => this;

        public int LineCount => this.lineStarts.Count;

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

        public SourceFile(string path, TextReader underlying)
        {
            this.Path = path;
            this.underlying = underlying;
            this.sourceText = new StringBuilder();
            this.lineStarts = new List<int>();
        }

        public SourceFile(string path, string source)
            : this(path, new StringReader(source))
        {
            while (this.ReadNextLine()) ;
            this.index = 0;
        }

        public string GetLine(int index)
        {
            if (!this.EnsureLineCount(index + 1)) return string.Empty;
            var from = this.lineStarts[index];
            if (index + 1 >= this.lineStarts.Count) return this.sourceText.ToString(from, this.sourceText.Length - from);
            else return this.sourceText.ToString(from, this.lineStarts[index + 1] - from);
        }

        public override void Close() => this.underlying.Close();

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

        public override int Peek()
        {
            if (!this.EnsureLength(this.index + 1)) return -1;
            return this.sourceText[this.index];
        }

        public override int Read()
        {
            var ch = this.Peek();
            if (ch != -1) this.index += 1;
            return ch;
        }

        public override int Read(Span<char> buffer)
        {
            this.EnsureLength(this.index + buffer.Length);
            var readUntil = Math.Min(this.index + buffer.Length, this.sourceText.Length);
            var count = readUntil - this.index;
            this.sourceText.CopyTo(this.index, buffer, count);
            this.index += count;
            return count;
        }

        public override int Read(char[] buffer, int index, int count) => this.Read(buffer.AsSpan(index, count));

        public override ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(this.Read(buffer.Span));

        public override Task<int> ReadAsync(char[] buffer, int index, int count) =>
            Task.FromResult(this.Read(buffer, index, count));

        public override int ReadBlock(Span<char> buffer) => this.Read(buffer);

        public override int ReadBlock(char[] buffer, int index, int count) => this.Read(buffer, index, count);

        public override ValueTask<int> ReadBlockAsync(Memory<char> buffer, CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(this.Read(buffer.Span));

        public override Task<int> ReadBlockAsync(char[] buffer, int index, int count) =>
            Task.FromResult(this.Read(buffer, index, count));

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

        public override Task<string?> ReadLineAsync() => Task.FromResult(this.ReadLine());

        public override string ReadToEnd()
        {
            var wasAt = this.index;
            while (this.ReadNextLine()) ;
            this.index = this.sourceText.Length;
            return this.sourceText.ToString(wasAt, this.index - wasAt);
        }

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
}
