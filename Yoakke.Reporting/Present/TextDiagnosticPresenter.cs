using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yoakke.Text;

namespace Yoakke.Reporting.Present
{
    /// <summary>
    /// A diagnostic presenter that just writes to a text buffer or console with color.
    /// </summary>
    public class TextDiagnosticPresenter : IDiagnosticPresenter
    {
        private abstract class LinePrimitive { }
        private class SourceLine : LinePrimitive
        {
            public ISourceFile? Source { get; set; }
            public int Line { get; set; }
        }
        private class DotLine : LinePrimitive { }
        private class AnnotationLine : LinePrimitive
        {
            public int AnnotatedLine { get; set; }
            public IEnumerable<SourceDiagnosticInfo>? Annotations { get; set; }
        }

        /// <summary>
        /// A default text presenter that writes to the console error.
        /// </summary>
        public static readonly TextDiagnosticPresenter Default = new(Console.Error);

        public DiagnosticStyle Style { get; set; } = DiagnosticStyle.Default;
        public ISyntaxHighlighter SyntaxHighlighter { get; set; } = ISyntaxHighlighter.Null;

        /// <summary>
        /// The <see cref="TextWriter"/> this presenter writes to.
        /// </summary>
        public TextWriter Writer { get; }

        private readonly ColoredBuffer buffer;

        /// <summary>
        /// Initializes a new <see cref="TextDiagnosticPresenter"/>.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        public TextDiagnosticPresenter(TextWriter writer)
        {
            Writer = writer;
            buffer = new ColoredBuffer();
        }

        public void Present(Diagnostic diagnostic)
        {
            // Clear the buffer
            buffer.Clear();

            // Write the head
            // error[CS123]: Some message
            WriteDiagnosticHead(diagnostic);

            // Collect out source information grouped by the files and ordered by their positions
            var sourceInfo = diagnostic.Information
                .OfType<SourceDiagnosticInfo>()
                .OrderBy(si => si.Location.Range.Start)
                .GroupBy(si => si.Location.File);
            // Print the groups
            foreach (var info in sourceInfo) WriteSourceGroup(info);

            // Finally print the footnores
            var footnotes = diagnostic.Information.OfType<FootnoteDiagnosticInfo>();
            foreach (var footnote in footnotes) WriteFootnote(footnote);

            // Dump results to the buffer
            buffer.OutputTo(Writer);
        }

        // Format is
        // error[CS123]: Some message
        private void WriteDiagnosticHead(Diagnostic diagnostic)
        {
            if (diagnostic.Severity != null)
            {
                buffer.ForegroundColor = Style.GetSeverityColor(diagnostic.Severity.Value);
                buffer.Write(diagnostic.Severity.Value.Name);
            }
            if (diagnostic.Code != null)
            {
                if (diagnostic.Severity != null) buffer.Write('[');
                buffer.Write(diagnostic.Code);
                if (diagnostic.Severity != null) buffer.Write(']');
                if (diagnostic.Message != null) buffer.Write(": ");
            }
            if (diagnostic.Message != null)
            {
                buffer.ForegroundColor = Style.DefaultColor;
                buffer.Write(diagnostic.Message);
            }
            if (diagnostic.Severity != null || diagnostic.Code != null || diagnostic.Message != null) buffer.WriteLine();
        }

        private void WriteSourceGroup(IEnumerable<SourceDiagnosticInfo> infos)
        {
            // Get the source file for this group
            var sourceFile = infos.First().Location.File;
            // Get what we assume to be the primary information
            // It's the first info with the biggest severity, or the first info if there are no severities
            var primaryInfo = infos
                .Where(info => info.Severity != null)
                .OrderByDescending(info => info.Severity)
                .FirstOrDefault()
                ?? infos.First();

            // Generate all line primitives
            var linePrimitives = CollectLinePrimitives(infos).ToList();
            // Find the largest line index printed
            var maxLineIndex = linePrimitives.OfType<SourceLine>().Select(l => l.Line).Max();
            // Create a padding to fit all line numbers from the largest of the group
            var lineNumberPadding = new string(' ', (maxLineIndex + 1).ToString().Length);

            // Print the ┌─ <file name>
            buffer.ForegroundColor = Style.DefaultColor;
            buffer.Write($"{lineNumberPadding} ┌─ {sourceFile.Path}");
            // If there is a primary info, write the line and column
            if (primaryInfo != null) buffer.Write($":{primaryInfo.Location.Range.Start.Line + 1}:{primaryInfo.Location.Range.Start.Column + 1}");
            buffer.WriteLine();

            // Write a single separator line
            buffer.WriteLine($"{lineNumberPadding} │");

            // Now print all the line primitives
            foreach (var line in linePrimitives)
            {
                switch (line)
                {
                case SourceLine sourceLine:
                    buffer.ForegroundColor = Style.LineNumberColor;
                    buffer.Write((sourceLine.Line + 1).ToString().PadLeft(lineNumberPadding.Length, Style.LineNumberPadding));
                    buffer.ForegroundColor = Style.DefaultColor;
                    buffer.Write(" │ ");
                    WriteSourceLine(sourceLine);
                    buffer.WriteLine();
                    break;

                case AnnotationLine annotation:
                    WriteAnnotationLine(annotation, $"{lineNumberPadding} │ ");
                    break;

                case DotLine dotLine:
                    buffer.ForegroundColor = Style.DefaultColor;
                    buffer.WriteLine($"{lineNumberPadding} │ ...");
                    break;
                }
            }

            // Write a single separator line
            buffer.WriteLine($"{lineNumberPadding} │");
        }

        private void WriteFootnote(FootnoteDiagnosticInfo info)
        {
            buffer.ForegroundColor = Style.DefaultColor;
            if (info.Message != null) buffer.WriteLine(info.Message);
        }

        private void WriteSourceLine(SourceLine line)
        {
            var xOffset = buffer.CursorX;
            // First print the source line with the default color
            buffer.ForegroundColor = SyntaxHighlighter.Style.DefaultColor;
            var lineText = line.Source!.GetLine(line.Line);
            var lineCur = 0;
            foreach (var ch in lineText)
            {
                if (ch == '\r' || ch == '\n') break;
                if (AdvanceCursor(ref lineCur, ch)) buffer.Write(ch);
                buffer.CursorX = xOffset + lineCur;
            }
            // Get the syntax highlight info
            var coloredTokens = SyntaxHighlighter.GetHighlightingForLine(line.Source, line.Line)
                .OrderBy(info => info.Start)
                .ToList();
            if (coloredTokens.Count > 0)
            {
                // There is info to highlight
                lineCur = 0;
                var charIndex = 0;
                foreach (var token in coloredTokens)
                {
                    // Walk there until the next token
                    for (; charIndex < token.Start; AdvanceCursor(ref lineCur, lineText[charIndex++])) ;
                    // Go through the token
                    var tokenStart = lineCur;
                    for (; charIndex < token.Start + token.Length; AdvanceCursor(ref lineCur, lineText[charIndex++])) ;
                    var tokenEnd = lineCur;
                    // Recolor it
                    buffer.ForegroundColor = SyntaxHighlighter.Style.GetTokenColor(token.Kind);
                    buffer.Recolor(xOffset + tokenStart, buffer.CursorY, tokenEnd - tokenStart, 1);
                }
            }
        }

        private void WriteAnnotationLine(AnnotationLine line, string prefix)
        {
            var sourceFile = line.Annotations!.First().Location.File;
            var lineText = sourceFile.GetLine(line.AnnotatedLine).TrimEnd();

            // Order annotations by starting position
            var annotationsOrdered = line.Annotations!.OrderBy(si => si.Location.Range.Start).ToList();
            // Now we draw the arrows to their correct places under the annotated line
            // Also collect physical column positions to extend the arrows
            var arrowHeadColumns = new List<(int Column, SourceDiagnosticInfo Info)>();
            buffer.ForegroundColor = Style.DefaultColor;
            buffer.Write(prefix);
            var lineCur = 0;
            var charIdx = 0;
            foreach (var annot in annotationsOrdered)
            {
                // From the last character index until the start of this annotation we need to fill with spaces
                for (; charIdx < annot.Location.Range.Start.Column; ++charIdx)
                {
                    if (charIdx < lineText.Length)
                    {
                        // Still in range of the line
                        AdvanceCursor(ref lineCur, lineText[charIdx]);
                    }
                    else
                    {
                        // After the line
                        lineCur += 1;
                    }
                }
                buffer.CursorX = prefix.Length + lineCur;
                // Now we are inside the span
                var arrowHead = annot.Severity != null ? '^' : '-';
                var startColumn = buffer.CursorX;
                arrowHeadColumns.Add((startColumn, annot));
                if (annot.Severity != null) buffer.ForegroundColor = Style.GetSeverityColor(annot.Severity.Value);
                for (; charIdx < annot.Location.Range.End.Column; ++charIdx)
                {
                    if (charIdx < lineText.Length)
                    {
                        // Still in range of the line
                        var oldOffset = lineCur;
                        AdvanceCursor(ref lineCur, lineText[charIdx]);
                        // Fill with arrow head
                        buffer.Fill(prefix.Length + oldOffset, buffer.CursorY, lineCur - oldOffset, 1, arrowHead);
                        buffer.CursorX = prefix.Length + lineCur;
                    }
                    else
                    {
                        // After the line
                        buffer.Write(arrowHead);
                    }
                }
                var endColumn = buffer.CursorX;
                // Recolor the source line too
                if (annot.Severity != null) buffer.Recolor(startColumn, buffer.CursorY - 1, endColumn - startColumn, 1);
            }
            // Now we are done with arrows in the line, it's time to do the arrow bodies downwards
            // The first one will have N, the last 0 length bodies, decreasing by one
            // The last one just has the message inline
            {
                var lastAnnot = annotationsOrdered.Last();
                if (lastAnnot.Message != null) buffer.Write($" {lastAnnot.Message}");
                buffer.WriteLine();
            }
            // From now on all previous ones will be one longer than the ones later
            int arrowBaseLine = buffer.CursorY;
            int arrowBodyLength = 0;
            // We only consider annotations with messages
            foreach (var (col, annot) in arrowHeadColumns.SkipLast(1).Reverse().Where(a => a.Info.Message != null))
            {
                if (annot.Severity != null) buffer.ForegroundColor = Style.GetSeverityColor(annot.Severity.Value);
                // Draw the arrow
                buffer.Fill(col, arrowBaseLine, 1, arrowBodyLength, '│');
                buffer.Plot(col, arrowBaseLine + arrowBodyLength, '└');
                arrowBodyLength += 1;
                // Append the message
                buffer.Write($" {annot.Message}");
                if (annot.Severity != null) buffer.ForegroundColor = Style.DefaultColor;
            }
            // Fill the in between lines with the prefix
            for (int i = 0; i < arrowBodyLength; ++i)
            {
                buffer.WriteAt(0, arrowBaseLine + i, prefix);
            }
            // Reset cursor position
            buffer.CursorX = 0;
            buffer.CursorY = arrowBaseLine + arrowBodyLength;
        }

        private IEnumerable<LinePrimitive> CollectLinePrimitives(IEnumerable<SourceDiagnosticInfo> infos)
        {
            // We need to group the spanned informations per line
            var groupedInfos = infos.GroupBy(si => si.Location.Range.Start.Line).ToList();
            var sourceFile = infos.First().Location.File;

            // Now we collect each line primitive
            int? lastLineIndex = null;
            for (int j = 0; j < groupedInfos.Count; ++j)
            {
                var infoGroup = groupedInfos[j];
                // First we determine the range we need to print for this info
                var currentLineIndex = infoGroup.Key;
                var minLineIndex = Math.Max(lastLineIndex ?? 0, currentLineIndex - Style.SurroundingLines);
                var maxLineIndex = Math.Min(sourceFile.LineCount, currentLineIndex + Style.SurroundingLines + 1);
                if (j < groupedInfos.Count - 1)
                {
                    // There's a chance we step over to the next annotation
                    var nextGroupLineIndex = groupedInfos[j + 1].Key;
                    maxLineIndex = Math.Min(maxLineIndex, nextGroupLineIndex);
                }
                // Determine if we need dotting or a line in between
                if (lastLineIndex != null)
                {
                    var difference = minLineIndex - lastLineIndex.Value;
                    if (difference <= Style.ConnectUpLines)
                    {
                        // Difference is negligible, connect them up, no reason to dot it out
                        for (int i = 0; i < difference; ++i)
                        {
                            yield return new SourceLine { Source = sourceFile, Line = lastLineIndex.Value + i };
                        }
                    }
                    else
                    {
                        // Bigger difference, dot out
                        yield return new DotLine { };
                    }
                }
                lastLineIndex = maxLineIndex;
                // Now we need to print all the relevant lines
                for (int i = minLineIndex; i < maxLineIndex; ++i)
                {
                    yield return new SourceLine { Source = sourceFile, Line = i };
                    // If this was an annotated line, yield the annotation
                    if (i == infoGroup.Key) yield return new AnnotationLine { AnnotatedLine = i, Annotations = infoGroup };
                }
            }
        }

        private bool AdvanceCursor(ref int pos, char ch)
        {
            if (ch == '\t')
            {
                pos += Style.TabSize - pos % Style.TabSize;
                return false;
            }
            else if (!char.IsControl(ch))
            {
                pos += 1;
                return true;
            }
            return false;
        }
    }
}
