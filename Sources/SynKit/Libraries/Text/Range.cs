// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SynKit.Text;

/// <summary>
/// An 2D interval of text positions with an inclusive starting position and an exclusive ending position.
/// </summary>
public readonly record struct Range : IEquatable<Range>
{
    /// <summary>
    /// The first <see cref="Position"/> that's inside this <see cref="Range"/>.
    /// </summary>
    public readonly Position Start;

    /// <summary>
    /// The first <see cref="Position"/> after this <see cref="Range"/>.
    /// </summary>
    public readonly Position End;

    /// <summary>
    /// Initializes a new instance of the <see cref="Range"/> struct.
    /// </summary>
    /// <param name="start">The first <see cref="Position"/> that's inside this range.</param>
    /// <param name="end">The first <see cref="Position"/> after this range.</param>
    public Range(Position start, Position end)
    {
        if (end < start) throw new ArgumentException("The end cannot be smaller than the start");

        this.Start = start;
        this.End = end;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Range"/> struct.
    /// </summary>
    /// <param name="start">The first <see cref="Position"/> that's inside this range.</param>
    /// <param name="length">The length of this range.</param>
    public Range(Position start, int length)
        : this(start, start.Advance(length))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Range"/> struct.
    /// </summary>
    /// <param name="from">The starting range.</param>
    /// <param name="to">The ending range.</param>
    public Range(Range from, Range to)
        : this(from.Start, to.End)
    {
    }

    /// <inheritdoc/>
    public override string ToString() => $"from {this.Start} to {this.End}";

    /// <summary>
    /// Checks if a given <see cref="Position"/> is within the bounds of this <see cref="Range"/>.
    /// </summary>
    /// <param name="position">The <see cref="Position"/> to check.</param>
    /// <returns>True, if the <see cref="Position"/> is contained in this <see cref="Range"/>.</returns>
    public bool Contains(Position position) => this.Start <= position && position < this.End;

    /// <summary>
    /// Checks if this <see cref="Range"/> intersects with another one.
    /// </summary>
    /// <param name="other">The other <see cref="Range"/> to check intersection with.</param>
    /// <returns>True, if the two <see cref="Range"/>s intersect.</returns>
    public bool Intersects(Range other) => !(this.Start >= other.End || other.Start >= this.End);
}
