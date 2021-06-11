// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Yoakke.Reporting.Present
{
    internal class ColoredBuffer
    {
        private class Line
        {
            public StringBuilder Text { get; set; } = new StringBuilder();

            public List<(ConsoleColor Foreground, ConsoleColor Background)> Color { get; set; } = new List<(ConsoleColor Foreground, ConsoleColor Background)>();
        }

        public ConsoleColor ForegroundColor { get; set; }

        public ConsoleColor BackgroundColor { get; set; }

        public int CursorX { get; set; }

        public int CursorY { get; set; }

        private readonly List<Line> lines = new();

        public void Clear()
        {
            this.CursorX = 0;
            this.CursorY = 0;
            this.ResetColor();
            this.lines.Clear();
        }

        public void ResetColor()
        {
            this.ForegroundColor = Console.ForegroundColor;
            this.BackgroundColor = Console.BackgroundColor;
        }

        public void Plot(int x, int y, char ch)
        {
            this.EnsureBuffer(x, y);
            var line = this.lines[y];
            line.Text[x] = ch;
            line.Color[x] = (this.ForegroundColor, this.BackgroundColor);
            this.CursorX = x + 1;
            this.CursorY = y;
        }

        public void WriteAt(int left, int top, string str)
        {
            this.EnsureBuffer(left + str.Length - 1, top);
            var line = this.lines[top];
            for (var i = 0; i < str.Length; ++i)
            {
                line.Text[left + i] = str[i];
                line.Color[left + i] = (this.ForegroundColor, this.BackgroundColor);
            }
            this.CursorX = left + str.Length;
            this.CursorY = top;
        }

        public void Write(char ch) => this.Plot(this.CursorX, this.CursorY, ch);

        public void Write(string str) => this.WriteAt(this.CursorX, this.CursorY, str);

        public void WriteLine()
        {
            this.CursorX = 0;
            this.CursorY += 1;
        }

        public void WriteLine(string str)
        {
            this.Write(str);
            this.WriteLine();
        }

        public void Fill(int left, int top, int width, int height, char ch)
        {
            for (var j = 0; j < height; ++j)
            {
                var yp = top + j;
                this.EnsureBuffer(left + width - 1, yp);
                var line = this.lines[yp];
                for (var i = 0; i < width; ++i)
                {
                    var xp = left + i;
                    line.Text[xp] = ch;
                    line.Color[xp] = (this.ForegroundColor, this.BackgroundColor);
                }
            }
            this.CursorX = left + width;
            this.CursorY = top + height - 1;
        }

        public void Recolor(int x, int y)
        {
            this.EnsureBuffer(x, y);
            var line = this.lines[y];
            line.Color[x] = (this.ForegroundColor, this.BackgroundColor);
        }

        public void Recolor(int left, int top, int width, int height)
        {
            for (var j = 0; j < height; ++j)
            {
                var yp = top + j;
                this.EnsureBuffer(left + width - 1, yp);
                var line = this.lines[yp];
                for (var i = 0; i < width; ++i)
                {
                    var xp = left + i;
                    line.Color[xp] = (this.ForegroundColor, this.BackgroundColor);
                }
            }
        }

        public void OutputTo(TextWriter writer)
        {
            var lastColor = (Console.ForegroundColor, Console.BackgroundColor);
            foreach (var line in this.lines)
            {
                var lineStr = line.Text.ToString();
                var i = 0;
                while (i < line.Text.Length)
                {
                    var start = i;
                    for (; i < line.Text.Length && line.Color[i] == lastColor; ++i)
                    {
                    }
                    // Print portion
                    writer.Write(lineStr.AsSpan(start, i - start));
                    // If the line has not ended, we must have changed color
                    if (i < line.Text.Length)
                    {
                        var (fg, bg) = line.Color[i];
                        Console.ForegroundColor = fg;
                        Console.BackgroundColor = bg;
                        lastColor = (fg, bg);
                    }
                }
                writer.WriteLine();
            }
            writer.Flush();
            Console.ResetColor();
        }

        private void EnsureBuffer(int x, int y)
        {
            // First we ensure y exists
            for (; this.lines.Count <= y; this.lines.Add(new Line()))
            {
            }
            // Now ensure x character in line y
            var line = this.lines[y];
            var requiredChars = x - line.Text.Length + 1;
            if (requiredChars > 0)
            {
                line.Text.Append(' ', requiredChars);
                if (line.Color.Capacity < x + 1) line.Color.Capacity = x + 1;
                for (var i = 0; i < requiredChars; ++i) line.Color.Add((this.ForegroundColor, this.BackgroundColor));
            }
        }
    }
}
