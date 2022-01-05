// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Yoakke.SynKit.Reporting.Present;

/// <summary>
/// A simple colored back-buffer to be able to write and recolor text without directly writing to the console.
/// </summary>
internal class ColoredBuffer
{
    private class Line
    {
        public StringBuilder Text { get; set; } = new StringBuilder();

        public List<(ConsoleColor Foreground, ConsoleColor Background)> Color { get; set; } = new List<(ConsoleColor Foreground, ConsoleColor Background)>();
    }

    /// <summary>
    /// The current foreground color to write with.
    /// </summary>
    public ConsoleColor ForegroundColor { get; set; }

    /// <summary>
    /// The current background color to write with.
    /// </summary>
    public ConsoleColor BackgroundColor { get; set; }

    /// <summary>
    /// The current x position of the cursor.
    /// </summary>
    public int CursorX { get; set; }

    /// <summary>
    /// The current y position of the cursor.
    /// </summary>
    public int CursorY { get; set; }

    private readonly List<Line> lines = new();

    /// <summary>
    /// Clears e buffers, setting everythung to empty.
    /// </summary>
    public void Clear()
    {
        this.CursorX = 0;
        this.CursorY = 0;
        this.ResetColor();
        this.lines.Clear();
    }

    /// <summary>
    /// Resets all colors to the default console colors.
    /// </summary>
    public void ResetColor()
    {
        this.ForegroundColor = Console.ForegroundColor;
        this.BackgroundColor = Console.BackgroundColor;
    }

    /// <summary>
    /// Writes a character at the given position.
    /// </summary>
    /// <param name="x">The x position to write the character to.</param>
    /// <param name="y">The y position to write the character to.</param>
    /// <param name="ch">The character to write.</param>
    public void Plot(int x, int y, char ch)
    {
        this.EnsureBuffer(x, y);
        var line = this.lines[y];
        line.Text[x] = ch;
        line.Color[x] = (this.ForegroundColor, this.BackgroundColor);
        this.CursorX = x + 1;
        this.CursorY = y;
    }

    /// <summary>
    /// Writes a string at the given position.
    /// </summary>
    /// <param name="left">The starting x position to write the character to.</param>
    /// <param name="top">The starting y position to write the character to.</param>
    /// <param name="str">The string to write.</param>
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

    /// <summary>
    /// Writes a character at the current cursor position.
    /// </summary>
    /// <param name="ch">The character to write.</param>
    public void Write(char ch) => this.Plot(this.CursorX, this.CursorY, ch);

    /// <summary>
    /// Writes a string starting from the current cursor position.
    /// </summary>
    /// <param name="str">The string to write.</param>
    public void Write(string str) => this.WriteAt(this.CursorX, this.CursorY, str);

    /// <summary>
    /// Starts a new line.
    /// </summary>
    public void WriteLine()
    {
        this.CursorX = 0;
        this.CursorY += 1;
    }

    /// <summary>
    /// Writes a string and starts a new line.
    /// </summary>
    /// <param name="str">The string to write.</param>
    public void WriteLine(string str)
    {
        this.Write(str);
        this.WriteLine();
    }

    /// <summary>
    /// Fills a rectangle with a given character.
    /// </summary>
    /// <param name="left">The left of the rectangle (start x position).</param>
    /// <param name="top">The top of the rectangle (start y position).</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="ch">The character to fill the rectangle with.</param>
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

    /// <summary>
    /// Recolors a given cell to the currently set colors.
    /// </summary>
    /// <param name="x">The x position of the cell to recolor.</param>
    /// <param name="y">The y position of the cell to recolor.</param>
    public void Recolor(int x, int y)
    {
        this.EnsureBuffer(x, y);
        var line = this.lines[y];
        line.Color[x] = (this.ForegroundColor, this.BackgroundColor);
    }

    /// <summary>
    /// Recolors a given rectangle to the currently set colors.
    /// </summary>
    /// <param name="left">The left of the rectangle (start x position).</param>
    /// <param name="top">The top of the rectangle (start y position).</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
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

    /// <summary>
    /// Outputs the contents of this <see cref="ColoredBuffer"/> to the given <see cref="TextWriter"/>.
    /// Keeps setting the colors to the appropriate color, so if it's being printed on the console, it will
    /// have the proper colors.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter"/> to write to.</param>
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
                    // Pass
                }
                // Print portion
                writer.Write(lineStr[start..i]);
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
