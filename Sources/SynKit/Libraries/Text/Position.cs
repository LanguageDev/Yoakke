// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SynKit.Text;

/// <summary>
/// Represents 2D position inside some text.
/// </summary>
/// <param name="Line">The 0-based line index.</param>
/// <param name="Column">The 0-based column index.</param>
public readonly record struct Position(int Line, int Column) : IComparable, IComparable<Position>
{
    /// <inheritdoc/>
    public override string ToString() => $"line {this.Line + 1}, column {this.Column + 1}";

    /// <inheritdoc/>
    public int CompareTo(object obj) => obj is Position pos
        ? this.CompareTo(pos)
        : throw new ArgumentException("Argument must be a Position", nameof(obj));

    /// <inheritdoc/>
    public int CompareTo(Position other)
    {
        var l = this.Line.CompareTo(other.Line);
        return l == 0 ? this.Column.CompareTo(other.Column) : l;
    }

    /// <summary>
    /// Less-than compares two <see cref="Position"/>s.
    /// </summary>
    /// <param name="p1">The first <see cref="Position"/> to compare.</param>
    /// <param name="p2">The second <see cref="Position"/> to compare.</param>
    /// <returns>True, if <paramref name="p1"/> comes before <paramref name="p2"/> in a text.</returns>
    public static bool operator <(Position p1, Position p2) => p1.CompareTo(p2) < 0;

    /// <summary>
    /// Greater-than compares two <see cref="Position"/>s.
    /// </summary>
    /// <param name="p1">The first <see cref="Position"/> to compare.</param>
    /// <param name="p2">The second <see cref="Position"/> to compare.</param>
    /// <returns>True, if <paramref name="p1"/> comes after <paramref name="p2"/> in a text.</returns>
    public static bool operator >(Position p1, Position p2) => p1.CompareTo(p2) > 0;

    /// <summary>
    /// Less-than or equals compares two <see cref="Position"/>s.
    /// </summary>
    /// <param name="p1">The first <see cref="Position"/> to compare.</param>
    /// <param name="p2">The second <see cref="Position"/> to compare.</param>
    /// <returns>True, if <paramref name="p1"/> comes before <paramref name="p2"/> in a text, or they happen
    /// to be the exact same <see cref="Position"/>.</returns>
    public static bool operator <=(Position p1, Position p2) => p1.CompareTo(p2) <= 0;

    /// <summary>
    /// Greater-than or equals compares two <see cref="Position"/>s.
    /// </summary>
    /// <param name="p1">The first <see cref="Position"/> to compare.</param>
    /// <param name="p2">The second <see cref="Position"/> to compare.</param>
    /// <returns>True, if <paramref name="p1"/> comes after <paramref name="p2"/> in a text, or they happen
    /// to be the exact same <see cref="Position"/>.</returns>
    public static bool operator >=(Position p1, Position p2) => p1.CompareTo(p2) >= 0;

    /// <summary>
    /// Creates a <see cref="Position"/> that's advanced in the current line by the given amount.
    /// </summary>
    /// <param name="amount">The amount to advance in the current line.</param>
    /// <returns>The <see cref="Position"/> in the same line, advanced by columns.</returns>
    public Position Advance(int amount = 1) => new(Line: this.Line, Column: this.Column + amount);

    /// <summary>
    /// Creates a <see cref="Position"/> that points to the first character of the next line.
    /// </summary>
    /// <returns>A <see cref="Position"/> in the next line's first character.</returns>
    public Position Newline() => new(Line: this.Line + 1, Column: 0);
}
