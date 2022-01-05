// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.IO;
using System.Linq;
using Yoakke.SynKit.Text;

namespace Yoakke.SynKit.Reporting.Present;

/// <summary>
/// A diagnostic presenter that just writes to a text buffer or console with color.
/// </summary>
public class TextDiagnosticsPresenter : IDiagnosticsPresenter
{
    private abstract class LinePrimitive
    {
    }

    private class SourceLine : LinePrimitive
    {
        public ISourceFile? Source { get; set; }

        public int Line { get; set; }
    }

    private class DotLine : LinePrimitive
    {
    }

    private class AnnotationLine : LinePrimitive
    {
        public int AnnotatedLine { get; set; }

        public IEnumerable<SourceDiagnosticInfo>? Annotations { get; set; }
    }

    /// <summary>
    /// A default text presenter that writes to the console error.
    /// </summary>
    public static readonly TextDiagnosticsPresenter Default = new(Console.Error);

    /// <inheritdoc/>
    public DiagnosticsStyle Style { get; set; } = DiagnosticsStyle.Default;

    /// <inheritdoc/>
    public ISyntaxHighlighter SyntaxHighlighter { get; set; } = NullSyntaxHighlighter.Instance;

    /// <summary>
    /// The <see cref="TextWriter"/> this presenter writes to.
    /// </summary>
    public TextWriter Writer { get; }

    private readonly ColoredBuffer buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextDiagnosticsPresenter"/> class.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    public TextDiagnosticsPresenter(TextWriter writer)
    {
        this.Writer = writer;
        this.buffer = new ColoredBuffer();
    }

    /// <inheritdoc/>
    public void Present(Diagnostics diagnostic)
    {
        // Clear the buffer
        this.buffer.Clear();

        // Write the head
        // error[CS123]: Some message
        this.WriteDiagnosticHead(diagnostic);

        // Collect out source information grouped by the files and ordered by their positions
        var sourceInfo = diagnostic.Information
            .OfType<SourceDiagnosticInfo>()
            .OrderBy(si => si.Location.Range.Start)
            .GroupBy(si => si.Location.File);
        // Print the groups
        foreach (var info in sourceInfo) this.WriteSourceGroup(info);

        // Finally print the footnores
        var footnotes = diagnostic.Information.OfType<FootnoteDiagnosticInfo>();
        foreach (var footnote in footnotes) this.WriteFootnote(footnote);

        // Dump results to the buffer
        this.buffer.OutputTo(this.Writer);
    }

    // Format is
    // error[CS123]: Some message
    private void WriteDiagnosticHead(Diagnostics diagnostic)
    {
        if (diagnostic.Severity != null)
        {
            this.buffer.ForegroundColor = this.Style.GetSeverityColor(diagnostic.Severity.Value);
            this.buffer.Write(diagnostic.Severity.Value.Name);
        }
        if (diagnostic.Code != null)
        {
            if (diagnostic.Severity != null) this.buffer.Write('[');
            this.buffer.Write(diagnostic.Code);
            if (diagnostic.Severity != null) this.buffer.Write(']');
        }
        if (diagnostic.Message != null)
        {
            if (diagnostic.Severity != null || diagnostic.Code != null) this.buffer.Write(": ");
            this.buffer.ForegroundColor = this.Style.DefaultColor;
            this.buffer.Write(diagnostic.Message);
        }
        if (diagnostic.Severity != null || diagnostic.Code != null || diagnostic.Message != null) this.buffer.WriteLine();
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
        var linePrimitives = this.CollectLinePrimitives(infos).ToList();
        // Find the largest line index printed
        var maxLineIndex = linePrimitives.OfType<SourceLine>().Select(l => l.Line).Max();
        // Create a padding to fit all line numbers from the largest of the group
        var lineNumberPadding = new string(' ', (maxLineIndex + 1).ToString().Length);

        // Print the ┌─ <file name>
        this.buffer.ForegroundColor = this.Style.DefaultColor;
        this.buffer.Write($"{lineNumberPadding} ┌─ {sourceFile.Path}");
        // If there is a primary info, write the line and column
        if (primaryInfo != null) this.buffer.Write($":{primaryInfo.Location.Range.Start.Line + 1}:{primaryInfo.Location.Range.Start.Column + 1}");
        this.buffer.WriteLine();

        // Write a single separator line
        this.buffer.WriteLine($"{lineNumberPadding} │");

        // Now print all the line primitives
        foreach (var line in linePrimitives)
        {
            switch (line)
            {
            case SourceLine sourceLine:
                this.buffer.ForegroundColor = this.Style.LineNumberColor;
                this.buffer.Write((sourceLine.Line + 1).ToString().PadLeft(lineNumberPadding.Length, this.Style.LineNumberPadding));
                this.buffer.ForegroundColor = this.Style.DefaultColor;
                this.buffer.Write(" │ ");
                this.WriteSourceLine(sourceLine);
                this.buffer.WriteLine();
                break;

            case AnnotationLine annotation:
                this.WriteAnnotationLine(annotation, $"{lineNumberPadding} │ ");
                break;

            case DotLine dotLine:
                this.buffer.ForegroundColor = this.Style.DefaultColor;
                this.buffer.WriteLine($"{lineNumberPadding} │ ...");
                break;
            }
        }

        // Write a single separator line
        this.buffer.ForegroundColor = this.Style.DefaultColor;
        this.buffer.WriteLine($"{lineNumberPadding} │");
    }

    private void WriteFootnote(FootnoteDiagnosticInfo info)
    {
        this.buffer.ForegroundColor = this.Style.DefaultColor;
        if (info.Message != null) this.buffer.WriteLine(info.Message);
    }

    private void WriteSourceLine(SourceLine line)
    {
        var xOffset = this.buffer.CursorX;
        // First print the source line with the default color
        this.buffer.ForegroundColor = this.SyntaxHighlighter.Style.DefaultColor;
        var lineText = line.Source!.GetLine(line.Line);
        var lineCur = 0;
        foreach (var ch in lineText)
        {
            if (ch == '\r' || ch == '\n') break;
            if (this.AdvanceCursor(ref lineCur, ch)) this.buffer.Write(ch);
            this.buffer.CursorX = xOffset + lineCur;
        }
        // Get the syntax highlight info
        var coloredTokens = this.SyntaxHighlighter.GetHighlightingForLine(line.Source, line.Line)
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
                for (; charIndex < token.Start; this.AdvanceCursor(ref lineCur, lineText[charIndex++]))
                {
                }
                // Go through the token
                var tokenStart = lineCur;
                for (; charIndex < token.Start + token.Length; this.AdvanceCursor(ref lineCur, lineText[charIndex++]))
                {
                }
                var tokenEnd = lineCur;
                // Recolor it
                this.buffer.ForegroundColor = this.SyntaxHighlighter.Style.GetTokenColor(token.Kind);
                this.buffer.Recolor(xOffset + tokenStart, this.buffer.CursorY, tokenEnd - tokenStart, 1);
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
        this.buffer.ForegroundColor = this.Style.DefaultColor;
        this.buffer.Write(prefix);
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
                    this.AdvanceCursor(ref lineCur, lineText[charIdx]);
                }
                else
                {
                    // After the line
                    lineCur += 1;
                }
            }
            this.buffer.CursorX = prefix.Length + lineCur;
            // Now we are inside the span
            var arrowHead = annot.Severity != null ? '^' : '-';
            var startColumn = this.buffer.CursorX;
            arrowHeadColumns.Add((startColumn, annot));
            if (annot.Severity != null) this.buffer.ForegroundColor = this.Style.GetSeverityColor(annot.Severity.Value);
            for (; charIdx < annot.Location.Range.End.Column; ++charIdx)
            {
                if (charIdx < lineText.Length)
                {
                    // Still in range of the line
                    var oldOffset = lineCur;
                    this.AdvanceCursor(ref lineCur, lineText[charIdx]);
                    // Fill with arrow head
                    this.buffer.Fill(prefix.Length + oldOffset, this.buffer.CursorY, lineCur - oldOffset, 1, arrowHead);
                    this.buffer.CursorX = prefix.Length + lineCur;
                }
                else
                {
                    // After the line
                    this.buffer.Write(arrowHead);
                }
            }
            var endColumn = this.buffer.CursorX;
            // Recolor the source line too
            if (annot.Severity != null) this.buffer.Recolor(startColumn, this.buffer.CursorY - 1, endColumn - startColumn, 1);
        }
        // Now we are done with arrows in the line, it's time to do the arrow bodies downwards
        // The first one will have N, the last 0 length bodies, decreasing by one
        // The last one just has the message inline
        {
            var lastAnnot = annotationsOrdered.Last();
            if (lastAnnot.Message != null) this.buffer.Write($" {lastAnnot.Message}");
            this.buffer.WriteLine();
        }
        // From now on all previous ones will be one longer than the ones later
        var arrowBaseLine = this.buffer.CursorY;
        var arrowBodyLength = 0;
        // We only consider annotations with messages
        foreach (var (col, annot) in arrowHeadColumns.SkipLast(1).Reverse().Where(a => a.Info.Message != null))
        {
            if (annot.Severity != null) this.buffer.ForegroundColor = this.Style.GetSeverityColor(annot.Severity.Value);
            // Draw the arrow
            this.buffer.Fill(col, arrowBaseLine, 1, arrowBodyLength, '│');
            this.buffer.Plot(col, arrowBaseLine + arrowBodyLength, '└');
            arrowBodyLength += 1;
            // Append the message
            this.buffer.Write($" {annot.Message}");
            if (annot.Severity != null) this.buffer.ForegroundColor = this.Style.DefaultColor;
        }
        // Fill the in between lines with the prefix
        for (var i = 0; i < arrowBodyLength; ++i)
        {
            this.buffer.WriteAt(0, arrowBaseLine + i, prefix);
        }
        // Reset cursor position
        this.buffer.CursorX = 0;
        this.buffer.CursorY = arrowBaseLine + arrowBodyLength;
    }

    private IEnumerable<LinePrimitive> CollectLinePrimitives(IEnumerable<SourceDiagnosticInfo> infos)
    {
        // We need to group the spanned informations per line
        var groupedInfos = infos.GroupBy(si => si.Location.Range.Start.Line).ToList();
        var sourceFile = infos.First().Location.File;

        // Now we collect each line primitive
        int? lastLineIndex = null;
        for (var j = 0; j < groupedInfos.Count; ++j)
        {
            var infoGroup = groupedInfos[j];
            // First we determine the range we need to print for this info
            var currentLineIndex = infoGroup.Key;
            var minLineIndex = Math.Max(lastLineIndex ?? 0, currentLineIndex - this.Style.SurroundingLines);
            var maxLineIndex = Math.Min(sourceFile.AvailableLines, currentLineIndex + this.Style.SurroundingLines + 1);
            // Trim empty source lines at edges
            if (this.Style.TrimEmptySourceLinesAtEdges)
            {
                for (var i = minLineIndex; i < currentLineIndex && string.IsNullOrWhiteSpace(sourceFile.GetLine(i)); ++i)
                {
                    ++minLineIndex;
                }
                for (var i = maxLineIndex - 1; i > currentLineIndex && string.IsNullOrWhiteSpace(sourceFile.GetLine(i)); --i)
                {
                    --maxLineIndex;
                }
            }
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
                if (difference <= this.Style.ConnectUpLines)
                {
                    // Difference is negligible, connect them up, no reason to dot it out
                    for (var i = 0; i < difference; ++i)
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
            for (var i = minLineIndex; i < maxLineIndex; ++i)
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
            pos += this.Style.TabSize - (pos % this.Style.TabSize);
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
