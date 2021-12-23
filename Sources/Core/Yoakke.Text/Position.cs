// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Text;

/// <summary>
/// Represents 2D position inside some text.
/// </summary>
public readonly struct Position : IEquatable<Position>, IComparable, IComparable<Position>
{
  /// <summary>
  /// The 0-based line index.
  /// </summary>
  public readonly int Line;

  /// <summary>
  /// The 0-based column index.
  /// </summary>
  public readonly int Column;

  /// <summary>
  /// Initializes a new instance of the <see cref="Position"/> struct.
  /// </summary>
  /// <param name="line">The 0-based line index.</param>
  /// <param name="column">The 0-based column index.</param>
  public Position(int line, int column)
  {
    this.Line = line;
    this.Column = column;
  }

  /// <inheritdoc/>
  public override bool Equals(object? obj) => obj is Position position && this.Equals(position);

  /// <inheritdoc/>
  public bool Equals(Position other) => this.CompareTo(other) == 0;

  /// <inheritdoc/>
  public override int GetHashCode() => HashCode.Combine(this.Line, this.Column);

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
  /// Compares two <see cref="Position"/>s for equality.
  /// </summary>
  /// <param name="p1">The first <see cref="Position"/> to compare.</param>
  /// <param name="p2">The second <see cref="Position"/> to compare.</param>
  /// <returns>True, if <paramref name="p1"/> and <paramref name="p2"/> are equal.</returns>
  public static bool operator ==(Position p1, Position p2) => p1.CompareTo(p2) == 0;

  /// <summary>
  /// Compares two <see cref="Position"/>s for inequality.
  /// </summary>
  /// <param name="p1">The first <see cref="Position"/> to compare.</param>
  /// <param name="p2">The second <see cref="Position"/> to compare.</param>
  /// <returns>True, if <paramref name="p1"/> and <paramref name="p2"/> are not equal.</returns>
  public static bool operator !=(Position p1, Position p2) => p1.CompareTo(p2) != 0;

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

  /// <inheritdoc/>
  public override string ToString() => $"line {this.Line + 1}, column {this.Column + 1}";

  /// <summary>
  /// Creates a <see cref="Position"/> that's advanced in the current line by the given amount.
  /// </summary>
  /// <param name="amount">The amount to advance in the current line.</param>
  /// <returns>The <see cref="Position"/> in the same line, advanced by columns.</returns>
  public Position Advance(int amount = 1) => new(line: this.Line, column: this.Column + amount);

  /// <summary>
  /// Creates a <see cref="Position"/> that points to the first character of the next line.
  /// </summary>
  /// <returns>A <see cref="Position"/> in the next line's first character.</returns>
  public Position Newline() => new(line: this.Line + 1, column: 0);
}
