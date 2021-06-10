using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Yoakke.Text
{
    /// <summary>
    /// A source file implementation with <see cref="TextReader"/>s.
    /// </summary>
    public class SourceFile : TextReader, ISourceFile
    {
        public string Path { get; }
        public TextReader Reader => this;
        public int LineCount => lineStarts.Count;

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
            Path = path;
            this.underlying = underlying;
            sourceText = new StringBuilder();
            lineStarts = new List<int>();
        }

        public SourceFile(string path, string source)
            : this(path, new StringReader(source))
        {
            while (ReadNextLine()) ;
            index = 0;
        }

        public string GetLine(int index)
        {
            if (!EnsureLineCount(index + 1)) return string.Empty;
            var from = lineStarts[index];
            if (index + 1 >= lineStarts.Count) return sourceText.ToString(from, sourceText.Length - from);
            else return sourceText.ToString(from, lineStarts[index + 1] - from);
        }

        public override void Close() => underlying.Close();
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposed) return;
            if (disposing)
            {
                underlying.Dispose();
                sourceText.Clear();
                lineStarts.Clear();
                sourceText = null!;
                lineStarts = null!;
            }
            disposed = true;
        }

        public override int Peek()
        {
            if (!EnsureLength(index + 1)) return -1;
            return sourceText[index];
        }

        public override int Read()
        {
            var ch = Peek();
            if (ch != -1) index += 1;
            return ch;
        }

        public override int Read(Span<char> buffer)
        {
            EnsureLength(index + buffer.Length);
            var readUntil = Math.Min(index + buffer.Length, sourceText.Length);
            int count = readUntil - index;
            sourceText.CopyTo(index, buffer, count);
            index += count;
            return count;
        }

        public override int Read(char[] buffer, int index, int count) => Read(buffer.AsSpan(index, count));

        public override ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(Read(buffer.Span));

        public override Task<int> ReadAsync(char[] buffer, int index, int count) =>
            Task.FromResult(Read(buffer, index, count));

        public override int ReadBlock(Span<char> buffer) => Read(buffer);
        public override int ReadBlock(char[] buffer, int index, int count) => Read(buffer, index, count);

        public override ValueTask<int> ReadBlockAsync(Memory<char> buffer, CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(Read(buffer.Span));

        public override Task<int> ReadBlockAsync(char[] buffer, int index, int count) =>
            Task.FromResult(Read(buffer, index, count));

        public override string? ReadLine()
        {
            if (index == sourceText.Length && !ReadNextLine()) return null;
            var lineIndex = lineStarts.BinarySearch(index);
            if (lineIndex < 0) lineIndex = -lineIndex - 1;
            string result;
            if (lineIndex + 1 >= lineStarts.Count) result = sourceText.ToString(lineStarts[lineIndex], sourceText.Length - lineStarts[lineIndex]);
            else result = sourceText.ToString(lineStarts[lineIndex], lineStarts[lineIndex + 1] - lineStarts[lineIndex]);
            index += result.Length;
            return result;
        }

        public override Task<string?> ReadLineAsync() => Task.FromResult(ReadLine());

        public override string ReadToEnd()
        {
            var wasAt = index;
            while (ReadNextLine()) ;
            index = sourceText.Length;
            return sourceText.ToString(wasAt, index - wasAt);
        }

        public override Task<string> ReadToEndAsync() => Task.FromResult(ReadToEnd());

        private bool EnsureLineCount(int count)
        {
            while (lineStarts.Count < count)
            {
                var line = underlying.ReadLine();
                if (line == null) return false;
                // There was a line to read
                lineStarts.Add(sourceText.Length);
                sourceText.AppendLine(line);
            }
            return true;
        }

        private bool ReadNextLine() => EnsureLineCount(lineStarts.Count + 1);

        private bool EnsureLength(int length)
        {
            while (sourceText.Length < length)
            {
                if (!ReadNextLine()) return false;
            }
            return true;
        }
    }
}
