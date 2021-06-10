using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private readonly List<Line> lines = new List<Line>();

        public void Clear()
        {
            CursorX = 0;
            CursorY = 0;
            ResetColor();
            lines.Clear();
        }

        public void ResetColor()
        {
            ForegroundColor = Console.ForegroundColor;
            BackgroundColor = Console.BackgroundColor;
        }

        public void Plot(int x, int y, char ch)
        {
            EnsureBuffer(x, y);
            var line = lines[y];
            line.Text[x] = ch;
            line.Color[x] = (ForegroundColor, BackgroundColor);
            CursorX = x + 1;
            CursorY = y;
        }

        public void WriteAt(int left, int top, string str)
        {
            EnsureBuffer(left + str.Length - 1, top);
            var line = lines[top];
            for (int i = 0; i < str.Length; ++i)
            {
                line.Text[left + i] = str[i];
                line.Color[left + i] = (ForegroundColor, BackgroundColor);
            }
            CursorX = left + str.Length;
            CursorY = top;
        }

        public void Write(char ch) => Plot(CursorX, CursorY, ch);
        public void Write(string str) => WriteAt(CursorX, CursorY, str);

        public void WriteLine()
        {
            CursorX = 0;
            CursorY += 1;
        }

        public void WriteLine(string str)
        {
            Write(str);
            WriteLine();
        }

        public void Fill(int left, int top, int width, int height, char ch)
        {
            for (int j = 0; j < height; ++j)
            {
                int yp = top + j;
                EnsureBuffer(left + width - 1, yp);
                var line = lines[yp];
                for (int i = 0; i < width; ++i)
                {
                    int xp = left + i;
                    line.Text[xp] = ch;
                    line.Color[xp] = (ForegroundColor, BackgroundColor);
                }
            }
            CursorX = left + width;
            CursorY = top + height - 1;
        }

        public void Recolor(int x, int y)
        {
            EnsureBuffer(x, y);
            var line = lines[y];
            line.Color[x] = (ForegroundColor, BackgroundColor);
        }

        public void Recolor(int left, int top, int width, int height)
        {
            for (int j = 0; j < height; ++j)
            {
                int yp = top + j;
                EnsureBuffer(left + width - 1, yp);
                var line = lines[yp];
                for (int i = 0; i < width; ++i)
                {
                    int xp = left + i;
                    line.Color[xp] = (ForegroundColor, BackgroundColor);
                }
            }
        }

        public void OutputTo(TextWriter writer)
        {
            var lastColor = (Console.ForegroundColor, Console.BackgroundColor);
            foreach (var line in lines)
            {
                var lineStr = line.Text.ToString();
                int i = 0;
                while (i < line.Text.Length)
                {
                    int start = i;
                    for (; i < line.Text.Length && line.Color[i] == lastColor; ++i) ;
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
            for (; lines.Count <= y; lines.Add(new Line())) ;
            // Now ensure x character in line y
            var line = lines[y];
            int requiredChars = x - line.Text.Length + 1;
            if (requiredChars > 0)
            {
                line.Text.Append(' ', requiredChars);
                if (line.Color.Capacity < x + 1) line.Color.Capacity = x + 1;
                for (int i = 0; i < requiredChars; ++i) line.Color.Add((ForegroundColor, BackgroundColor));
            }
        }
    }
}
