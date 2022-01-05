// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Yoakke.Collections.Intervals;

/// <summary>
/// Represents a finite or infinite interval.
/// </summary>
/// <typeparam name="T">The type of the endpoint value.</typeparam>
public readonly struct Interval<T> : IEquatable<Interval<T>>
{
    /// <summary>
    /// A value parser function.
    /// </summary>
    /// <param name="text">The text to parse from.</param>
    /// <param name="value">The parsed value gets written here.</param>
    /// <returns>True, if parsing the value was successful.</returns>
    public delegate bool TryParseValueString(string text, [MaybeNullWhen(false)] out T value);

    /// <summary>
    /// A value parser function.
    /// </summary>
    /// <param name="text">The text to parse from.</param>
    /// <param name="value">The parsed value gets written here.</param>
    /// <returns>True, if parsing the value was successful.</returns>
    public delegate bool TryParseValueSpan(ReadOnlySpan<char> text, [MaybeNullWhen(false)] out T value);

    /// <summary>
    /// A value parser function.
    /// </summary>
    /// <param name="text">The text to parse from.</param>
    /// <returns>The parsed value.</returns>
    public delegate T ParseValueString(string text);

    /// <summary>
    /// A value parser function.
    /// </summary>
    /// <param name="text">The text to parse from.</param>
    /// <returns>The parsed value.</returns>
    public delegate T ParseValueSpan(ReadOnlySpan<char> text);

    /// <summary>
    /// A full interval.
    /// </summary>
    public static Interval<T> Full { get; } = new(LowerBound<T>.Unbounded.Instance, UpperBound<T>.Unbounded.Instance);

    /// <summary>
    /// An empty interval.
    /// </summary>
    public static Interval<T> Empty { get; } = new(new LowerBound<T>.Exclusive(default!), new UpperBound<T>.Exclusive(default!));

    /// <summary>
    /// The lower-bound of the interval.
    /// </summary>
    public LowerBound<T> Lower { get; init; }

    /// <summary>
    /// The upper-bound of the interval.
    /// </summary>
    public UpperBound<T> Upper { get; init; }

    /// <summary>
    /// True, if this interval is empty.
    /// </summary>
    public bool IsEmpty => IntervalComparer<T>.Default.IsEmpty(this);

    /// <summary>
    /// Initializes a new instance of the <see cref="Interval{T}"/> struct.
    /// </summary>
    /// <param name="lower">The lower-bound of the interval.</param>
    /// <param name="upper">The upper-bound of the interval.</param>
    public Interval(LowerBound<T> lower, UpperBound<T> upper)
    {
        this.Lower = lower;
        this.Upper = upper;
    }

    /// <summary>
    /// Constructs an interval that contains a single element.
    /// </summary>
    /// <param name="value">The element that is contained by the interval.</param>
    /// <returns>A new interval, that only contains <paramref name="value"/>.</returns>
    public static Interval<T> Singleton(T value) => new(new LowerBound<T>.Inclusive(value), new UpperBound<T>.Inclusive(value));

    /// <summary>
    /// Implicitly converts a value to an interval that only contains the value.
    /// </summary>
    /// <param name="value">The value to make into a singleton interval.</param>
    public static implicit operator Interval<T>(T value) => Singleton(value);

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is Interval<T> other && this.Equals(other);

    /// <inheritdoc/>
    public bool Equals(Interval<T> other) => IntervalComparer<T>.Default.Equals(this, other);

    /// <inheritdoc/>
    public override int GetHashCode() => IntervalComparer<T>.Default.GetHashCode(this);

    /// <inheritdoc/>
    public override string ToString() => $"{this.Lower}; {this.Upper}";

    /// <summary>
    /// Checks if a value is inside an interval.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True, if this interval contains <paramref name="value"/>.</returns>
    public bool Contains(T value) => IntervalComparer<T>.Default.Contains(this, value);

    /// <summary>
    /// Checks if this interval is completely before another one, without overlapping.
    /// </summary>
    /// <param name="other">The interval to check.</param>
    /// <returns>True, if this is completely before <paramref name="other"/>.</returns>
    public bool IsBefore(Interval<T> other) => IntervalComparer<T>.Default.IsBefore(this, other);

    /// <summary>
    /// Checks if an interval is disjunct with this one.
    /// </summary>
    /// <param name="other">The other interval to check.</param>
    /// <returns>True, if this and <paramref name="other"/> are completely disjunct.</returns>
    public bool IsDisjunct(Interval<T> other) => IntervalComparer<T>.Default.IsDisjunct(this, other);

    /// <summary>
    /// Checks if an interval is touching another one.
    /// </summary>
    /// <param name="other">The other interval to check.</param>
    /// <returns>True, if this and <paramref name="other"/> are touching on an endpoint.</returns>
    public bool IsTouching(Interval<T> other) => IntervalComparer<T>.Default.IsTouching(this, other);

    /// <summary>
    /// Returns the intersection of this interval with another.
    /// </summary>
    /// <param name="other">The other interval to get the intersection with.</param>
    /// <returns>The intersection of this and <paramref name="other"/>.</returns>
    public Interval<T> Intersection(Interval<T> other) => IntervalComparer<T>.Default.Intersection(this, other);

    /// <summary>
    /// Calculates the relation to another interval.
    /// </summary>
    /// <param name="other">The interval to calculate the relation to.</param>
    /// <returns>An <see cref="IntervalRelation{T}"/> of this and <paramref name="other"/>.</returns>
    public IntervalRelation<T> RelationTo(Interval<T> other) => IntervalComparer<T>.Default.Relation(this, other);

    /// <summary>
    /// Compares two <see cref="Interval{T}"/>s for equality.
    /// </summary>
    /// <param name="a">The first <see cref="Interval{T}"/> to compare.</param>
    /// <param name="b">The second <see cref="Interval{T}"/> to compare.</param>
    /// <returns>True, if <paramref name="a"/> and <paramref name="b"/> are equal.</returns>
    public static bool operator ==(Interval<T> a, Interval<T> b) => a.Equals(b);

    /// <summary>
    /// Compares two <see cref="Interval{T}"/>s for inequality.
    /// </summary>
    /// <param name="a">The first <see cref="Interval{T}"/> to compare.</param>
    /// <param name="b">The second <see cref="Interval{T}"/> to compare.</param>
    /// <returns>True, if <paramref name="a"/> and <paramref name="b"/> are not equal.</returns>
    public static bool operator !=(Interval<T> a, Interval<T> b) => !a.Equals(b);

    #region Parsing

    /// <summary>
    /// Parses an <see cref="Interval{T}"/> from a text representation.
    /// </summary>
    /// <param name="text">The text representation to parse from.</param>
    /// <param name="valueParser">The value parser function.</param>
    /// <param name="interval">The parsed interval gets written here.</param>
    /// <returns>True, if the parse was successful.</returns>
    public static bool TryParse(ReadOnlySpan<char> text, TryParseValueSpan valueParser, [MaybeNullWhen(false)] out Interval<T> interval)
    {
        bool TryParseAdapter(ReadOnlySpan<char> text, [MaybeNullWhen(false)] out T value) => valueParser(text, out value);

        return TryParseInternal(text, TryParseAdapter, out interval);
    }

    /// <summary>
    /// Parses an <see cref="Interval{T}"/> from a text representation.
    /// </summary>
    /// <param name="text">The text representation to parse from.</param>
    /// <param name="valueParser">The value parser function.</param>
    /// <param name="interval">The parsed interval gets written here.</param>
    /// <returns>True, if the parse was successful.</returns>
    public static bool TryParse(string text, TryParseValueString valueParser, [MaybeNullWhen(false)] out Interval<T> interval)
    {
        bool TryParseAdapter(ReadOnlySpan<char> text, [MaybeNullWhen(false)] out T value) => valueParser(text.ToString(), out value);

        return TryParseInternal(text.AsSpan(), TryParseAdapter, out interval);
    }

    /// <summary>
    /// Parses an <see cref="Interval{T}"/> from a text representation.
    /// </summary>
    /// <param name="text">The text representation to parse from.</param>
    /// <param name="valueParser">The value parser function.</param>
    /// <returns>The parsed interval.</returns>
    public static Interval<T> Parse(ReadOnlySpan<char> text, ParseValueSpan valueParser)
    {
        bool TryParseAdapter(ReadOnlySpan<char> text, [MaybeNullWhen(false)] out T value)
        {
            value = valueParser(text);
            return true;
        }
        if (!TryParseInternal(text, TryParseAdapter, out var interval)) throw new FormatException("The string did not represent a valid interval");
        return interval;
    }

    /// <summary>
    /// Parses an <see cref="Interval{T}"/> from a text representation.
    /// </summary>
    /// <param name="text">The text representation to parse from.</param>
    /// <param name="valueParser">The value parser function.</param>
    /// <returns>The parsed interval.</returns>
    public static Interval<T> Parse(string text, ParseValueString valueParser) =>
        Parse(text.AsSpan(), text => valueParser(text.ToString()));

    private static bool TryParseInfinity(ReadOnlySpan<char> text, bool negative)
    {
        // First we consume the sign
        if (negative)
        {
            if (text.Length == 0 || text[0] != '-') return false;
            text = text[1..].TrimStart();
        }
        else if (text.Length > 0 && text[0] == '+')
        {
            text = text[1..].TrimStart();
        }
        // Then we check if the string is contained within the valid infinities
        for (var i = 0; i < Interval.InfinityStrings.Count; ++i)
        {
            if (text.Equals(Interval.InfinityStrings[i].AsSpan(), StringComparison.InvariantCulture)) return true;
        }
        return false;
    }

    private static bool TryParseInternal(ReadOnlySpan<char> text, TryParseValueSpan parser, [MaybeNullWhen(false)] out Interval<T> interval)
    {
        interval = default;
        // '('/'[', ';' and ')'/']' at minimum (so 3 chars)
        text = text.Trim();
        if (text.Length < 3) return false;
        // First, we check for the two ends
        var firstChar = text[0];
        var lastChar = text[^1];
        // Check, if the borderline characters are OK
        if (firstChar != '(' && firstChar != '[' && firstChar != ']') return false;
        if (lastChar != ')' && lastChar != ']' && lastChar != '[') return false;
        // Search for the ';'
        var semicolIndex = text.IndexOf(';');
        if (semicolIndex == -1) return false;
        // Get the sub-spans of the lower- and upper bounds
        var lowerSpan = text[1..semicolIndex].Trim();
        var upperSpan = text[(semicolIndex + 1)..^1].Trim();
        // We handle some defaults for infinities
        LowerBound<T>? lower = (firstChar == '(' || firstChar == ']') && TryParseInfinity(lowerSpan, true) ? LowerBound<T>.Unbounded.Instance : null;
        UpperBound<T>? upper = (lastChar == ')' || lastChar == '[') && TryParseInfinity(upperSpan, false) ? UpperBound<T>.Unbounded.Instance : null;
        // Call the parser function for the remaining needed bounds
        if (lower is null)
        {
            if (!parser(lowerSpan, out var lowerValue)) return false;
            lower = firstChar == '[' ? new LowerBound<T>.Inclusive(lowerValue) : new LowerBound<T>.Exclusive(lowerValue);
        }
        if (upper is null)
        {
            if (!parser(upperSpan, out var upperValue)) return false;
            upper = lastChar == ']' ? new UpperBound<T>.Inclusive(upperValue) : new UpperBound<T>.Exclusive(upperValue);
        }
        // We are done
        interval = new(lower, upper);
        return true;
    }

    #endregion
}
