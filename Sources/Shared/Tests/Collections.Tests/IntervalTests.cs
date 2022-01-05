// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Xunit;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Tests;

public class IntervalTests
{
    public static IEnumerable<object[]> IntervalToStringData { get; } = new object[][]
    {
            new object[] { Interval<int>.Full, "(-∞; +∞)" },
            new object[] { new Interval<int>(new LowerBound<int>.Exclusive(-12), new UpperBound<int>.Exclusive(56)), "(-12; 56)" },
            new object[] { new Interval<int>(new LowerBound<int>.Inclusive(-12), new UpperBound<int>.Exclusive(56)), "[-12; 56)" },
            new object[] { new Interval<int>(new LowerBound<int>.Exclusive(-12), new UpperBound<int>.Inclusive(56)), "(-12; 56]" },
            new object[] { new Interval<int>(new LowerBound<int>.Inclusive(-12), new UpperBound<int>.Inclusive(56)), "[-12; 56]" },
            new object[] { new Interval<int>(LowerBound<int>.Unbounded.Instance, new UpperBound<int>.Inclusive(56)), "(-∞; 56]" },
            new object[] { new Interval<int>(new LowerBound<int>.Inclusive(-12), UpperBound<int>.Unbounded.Instance), "[-12; +∞)" },
    };

    public static IEnumerable<object[]> IntervalParseData { get; } = new object[][]
    {
            new object[] { "(-oo; +oo)", Interval<int>.Full },
            new object[] { "(-infty;∞)", Interval<int>.Full },
            new object[] { "(-infty; + infinity)", Interval<int>.Full },
            new object[] { "(-;)", Interval<int>.Full },
            new object[] { "(-12; 56)", new Interval<int>(new LowerBound<int>.Exclusive(-12), new UpperBound<int>.Exclusive(56)) },
            new object[] { "[-12; 56)", new Interval<int>(new LowerBound<int>.Inclusive(-12), new UpperBound<int>.Exclusive(56)) },
            new object[] { "(-12; 56]", new Interval<int>(new LowerBound<int>.Exclusive(-12), new UpperBound<int>.Inclusive(56)) },
            new object[] { "[-12; 56]", new Interval<int>(new LowerBound<int>.Inclusive(-12), new UpperBound<int>.Inclusive(56)) },
            new object[] { "[-12; oo)", new Interval<int>(new LowerBound<int>.Inclusive(-12), UpperBound<int>.Unbounded.Instance) },
            new object[] { "(-oo; 56]", new Interval<int>(LowerBound<int>.Unbounded.Instance, new UpperBound<int>.Inclusive(56)) },
            new object[] { "]-infty;∞[", Interval<int>.Full },
            new object[] { "]-12; 56[", new Interval<int>(new LowerBound<int>.Exclusive(-12), new UpperBound<int>.Exclusive(56)) },
            new object[] { "[-12; 56[", new Interval<int>(new LowerBound<int>.Inclusive(-12), new UpperBound<int>.Exclusive(56)) },
            new object[] { "]-12; 56]", new Interval<int>(new LowerBound<int>.Exclusive(-12), new UpperBound<int>.Inclusive(56)) },
    };

    public static IEnumerable<object[]> LowerBoundLowerBoundComparisonData { get; } = new object[][]
    {
            new object[] { LowerBound<int>.Unbounded.Instance, LowerBound<int>.Unbounded.Instance, 0 },
            new object[] { LowerBound<int>.Unbounded.Instance, new LowerBound<int>.Exclusive(0), -1 },
            new object[] { LowerBound<int>.Unbounded.Instance, new LowerBound<int>.Inclusive(0), -1 },
            new object[] { new LowerBound<int>.Exclusive(0), new LowerBound<int>.Exclusive(0), 0 },
            new object[] { new LowerBound<int>.Exclusive(0), new LowerBound<int>.Inclusive(0), 1 },
            new object[] { new LowerBound<int>.Inclusive(0), new LowerBound<int>.Inclusive(0), 0 },
            new object[] { new LowerBound<int>.Exclusive(0), new LowerBound<int>.Exclusive(1), -1 },
            new object[] { new LowerBound<int>.Exclusive(0), new LowerBound<int>.Inclusive(1), -1 },
            new object[] { new LowerBound<int>.Inclusive(0), new LowerBound<int>.Inclusive(1), -1 },
    };

    public static IEnumerable<object[]> UpperBoundUpperBoundComparisonData { get; } = new object[][]
    {
            new object[] { UpperBound<int>.Unbounded.Instance, UpperBound<int>.Unbounded.Instance, 0 },
            new object[] { UpperBound<int>.Unbounded.Instance, new UpperBound<int>.Exclusive(0), 1 },
            new object[] { UpperBound<int>.Unbounded.Instance, new UpperBound<int>.Inclusive(0), 1 },
            new object[] { new UpperBound<int>.Exclusive(0), new UpperBound<int>.Exclusive(0), 0 },
            new object[] { new UpperBound<int>.Exclusive(0), new UpperBound<int>.Inclusive(0), -1 },
            new object[] { new UpperBound<int>.Inclusive(0), new UpperBound<int>.Inclusive(0), 0 },
            new object[] { new UpperBound<int>.Exclusive(0), new UpperBound<int>.Exclusive(1), -1 },
            new object[] { new UpperBound<int>.Exclusive(0), new UpperBound<int>.Inclusive(1), -1 },
            new object[] { new UpperBound<int>.Inclusive(0), new UpperBound<int>.Inclusive(1), -1 },
    };

    public static IEnumerable<object[]> LowerBoundUpperBoundComparisonData { get; } = new object[][]
    {
            new object[] { LowerBound<int>.Unbounded.Instance, UpperBound<int>.Unbounded.Instance, -1 },
            new object[] { LowerBound<int>.Unbounded.Instance, new UpperBound<int>.Exclusive(0), -1 },
            new object[] { LowerBound<int>.Unbounded.Instance, new UpperBound<int>.Inclusive(0), -1 },
            new object[] { new LowerBound<int>.Exclusive(0), new UpperBound<int>.Exclusive(0), 1 },
            new object[] { new LowerBound<int>.Exclusive(0), new UpperBound<int>.Inclusive(0), 1 },
            new object[] { new LowerBound<int>.Inclusive(0), new UpperBound<int>.Exclusive(0), 1 },
            new object[] { new LowerBound<int>.Inclusive(0), new UpperBound<int>.Inclusive(0), -1 },
            new object[] { new LowerBound<int>.Exclusive(0), new UpperBound<int>.Exclusive(1), -1 },
            new object[] { new LowerBound<int>.Exclusive(0), new UpperBound<int>.Inclusive(1), -1 },
            new object[] { new LowerBound<int>.Inclusive(0), new UpperBound<int>.Exclusive(1), -1 },
            new object[] { new LowerBound<int>.Inclusive(0), new UpperBound<int>.Inclusive(1), -1 },
    };

    public static IEnumerable<object[]> TouchingBoundsData { get; } = new object[][]
    {
            new object[] { LowerBound<int>.Unbounded.Instance, UpperBound<int>.Unbounded.Instance, false },
            new object[] { LowerBound<int>.Unbounded.Instance, new UpperBound<int>.Exclusive(0), false },
            new object[] { LowerBound<int>.Unbounded.Instance, new UpperBound<int>.Inclusive(0), false },
            new object[] { new LowerBound<int>.Exclusive(0), new UpperBound<int>.Exclusive(0), false },
            new object[] { new LowerBound<int>.Exclusive(0), new UpperBound<int>.Inclusive(0), true },
            new object[] { new LowerBound<int>.Inclusive(0), new UpperBound<int>.Exclusive(0), true },
            new object[] { new LowerBound<int>.Inclusive(0), new UpperBound<int>.Inclusive(0), false },
            new object[] { new LowerBound<int>.Exclusive(0), new UpperBound<int>.Exclusive(1), false },
            new object[] { new LowerBound<int>.Exclusive(0), new UpperBound<int>.Inclusive(1), false },
            new object[] { new LowerBound<int>.Inclusive(0), new UpperBound<int>.Exclusive(1), false },
            new object[] { new LowerBound<int>.Inclusive(0), new UpperBound<int>.Inclusive(1), false },
    };

    [Theory]
    [MemberData(nameof(IntervalToStringData))]
    public void IntervalToString(Interval<int> interval, string text)
    {
        Assert.Equal(text, interval.ToString());
    }

    [Theory]
    [MemberData(nameof(IntervalParseData))]
    public void IntervalParse(string text, Interval<int> interval)
    {
        // Parse all ways
        var parsedFromString = Interval<int>.Parse(text, int.Parse);
        var parsedFromSpan = Interval<int>.Parse(text.AsSpan(), span => int.Parse(span));
        Assert.True(Interval<int>.TryParse(text, int.TryParse, out var tryParsedFromString));
        Assert.True(Interval<int>.TryParse(text.AsSpan(), int.TryParse, out var tryParsedFromSpan));

        // All must be equal to the expected
        Assert.Equal(interval, parsedFromString);
        Assert.Equal(interval, parsedFromSpan);
        Assert.Equal(interval, tryParsedFromString);
        Assert.Equal(interval, tryParsedFromSpan);
    }

    [Theory]
    [MemberData(nameof(LowerBoundLowerBoundComparisonData))]
    public void LowerBoundLowerBoundCompare(LowerBound<int> a, LowerBound<int> b, int cmp)
    {
        if (cmp < 0) AssertLess(a, b);
        else if (cmp > 0) AssertGreater(a, b);
        else AssertEquals(a, b);
    }

    [Theory]
    [MemberData(nameof(UpperBoundUpperBoundComparisonData))]
    public void UpperBoundUpperBoundCompare(UpperBound<int> a, UpperBound<int> b, int cmp)
    {
        if (cmp < 0) AssertLess(a, b);
        else if (cmp > 0) AssertGreater(a, b);
        else AssertEquals(a, b);
    }

    [Theory]
    [MemberData(nameof(LowerBoundUpperBoundComparisonData))]
    public void LowerBoundUpperBoundCompare(LowerBound<int> a, UpperBound<int> b, int cmp)
    {
        if (cmp < 0) AssertLess(a, b);
        else AssertGreater(a, b);
    }

    [Theory]
    [MemberData(nameof(TouchingBoundsData))]
    public void TouchingBounds(LowerBound<int> a, UpperBound<int> b, bool touching)
    {
        if (touching)
        {
            // Touching assertions
            Assert.True(a.IsTouching(b));
            Assert.True(b.IsTouching(a));
            Assert.True(BoundComparer<int>.Default.IsTouching(a, b));
            Assert.True(BoundComparer<int>.Default.IsTouching(b, a));

            // Touching endpoint equality
            AssertEquals(a.Touching!, b);
            AssertEquals(a, b.Touching!);
        }
        else
        {
            // Not touching assertions
            Assert.False(a.IsTouching(b));
            Assert.False(b.IsTouching(a));
            Assert.False(BoundComparer<int>.Default.IsTouching(a, b));
            Assert.False(BoundComparer<int>.Default.IsTouching(b, a));
        }
    }

    [Theory]
    [InlineData("(-oo; +oo)", 0, true)]
    [InlineData("(-oo; 0)", 0, false)]
    [InlineData("(-oo; 0]", 0, true)]
    [InlineData("[0; 0]", 0, true)]
    [InlineData("(0; 0]", 0, false)]
    [InlineData("[0; 0)", 0, false)]
    [InlineData("(0; 0)", 0, false)]
    [InlineData("[-1; 0)", 0, false)]
    [InlineData("[-1; 0]", 0, true)]
    public void Containment(string text, int value, bool contains)
    {
        var interval = Interval<int>.Parse(text, int.Parse);
        if (contains)
        {
            Assert.True(interval.Contains(value));
            Assert.True(IntervalComparer<int>.Default.Contains(interval, value));
        }
        else
        {
            Assert.False(interval.Contains(value));
            Assert.False(IntervalComparer<int>.Default.Contains(interval, value));
        }
    }

    [Theory]
    [InlineData("(-oo; +oo)", false)]
    [InlineData("(-oo; 0)", false)]
    [InlineData("(-oo; 0]", false)]
    [InlineData("[0; 0]", false)]
    [InlineData("(0; 0]", true)]
    [InlineData("[0; 0)", true)]
    [InlineData("(0; 0)", true)]
    [InlineData("[-1; 0)", false)]
    [InlineData("[-1; 0]", false)]
    [InlineData("(0; -1)", true)]
    [InlineData("[0; -1)", true)]
    [InlineData("(0; -1]", true)]
    [InlineData("[0; -1]", true)]
    public void Emptiness(string text, bool empty)
    {
        var interval = Interval<int>.Parse(text, int.Parse);
        if (empty)
        {
            Assert.True(interval.IsEmpty);
            Assert.True(IntervalComparer<int>.Default.IsEmpty(interval));
        }
        else
        {
            Assert.False(interval.IsEmpty);
            Assert.False(IntervalComparer<int>.Default.IsEmpty(interval));
        }
    }

    [Theory]
    // Empty intervals are equal
    [InlineData("(0; 0)", "(1; 1)", typeof(IntervalRelation<int>.Equal), "(0; 0)", "(0; 0)", "(0; 0)")]
    [InlineData("[0; 0)", "(0; 0)", typeof(IntervalRelation<int>.Equal), "(0; 0)", "(0; 0)", "(0; 0)")]
    [InlineData("[0; -1]", "(0; 0)", typeof(IntervalRelation<int>.Equal), "(0; 0)", "(0; 0)", "(0; 0)")]
    // Disjunct
    [InlineData("(1; 2)", "[3; 4)", typeof(IntervalRelation<int>.Disjunct), "(1; 2)", "(0; 0)", "[3; 4)")]
    [InlineData("(1; 2)", "(2; 4)", typeof(IntervalRelation<int>.Disjunct), "(1; 2)", "(0; 0)", "(2; 4)")]
    // Touching
    [InlineData("(1; 2)", "[2; 4)", typeof(IntervalRelation<int>.Touching), "(1; 2)", "(0; 0)", "[2; 4)")]
    [InlineData("(1; 2]", "(2; 4)", typeof(IntervalRelation<int>.Touching), "(1; 2]", "(0; 0)", "(2; 4)")]
    // Overlapping
    [InlineData("(1; 3)", "(2; 4)", typeof(IntervalRelation<int>.Overlapping), "(1; 2]", "(2; 3)", "[3; 4)")]
    [InlineData("(1; 3]", "(2; 4)", typeof(IntervalRelation<int>.Overlapping), "(1; 2]", "(2; 3]", "(3; 4)")]
    [InlineData("(1; 3)", "[2; 4)", typeof(IntervalRelation<int>.Overlapping), "(1; 2)", "[2; 3)", "[3; 4)")]
    [InlineData("(1; 3]", "[2; 4)", typeof(IntervalRelation<int>.Overlapping), "(1; 2)", "[2; 3]", "(3; 4)")]
    // Containing
    [InlineData("(1; 4)", "(2; 3)", typeof(IntervalRelation<int>.Containing), "(1; 2]", "(2; 3)", "[3; 4)")]
    [InlineData("(1; 4)", "[2; 3)", typeof(IntervalRelation<int>.Containing), "(1; 2)", "[2; 3)", "[3; 4)")]
    [InlineData("(1; 4)", "(2; 3]", typeof(IntervalRelation<int>.Containing), "(1; 2]", "(2; 3]", "(3; 4)")]
    [InlineData("(1; 4)", "[2; 3]", typeof(IntervalRelation<int>.Containing), "(1; 2)", "[2; 3]", "(3; 4)")]
    [InlineData("[1; 4]", "(1; 4)", typeof(IntervalRelation<int>.Containing), "[1; 1]", "(1; 4)", "[4; 4]")]
    // Starting
    [InlineData("(1; 4)", "(1; 3)", typeof(IntervalRelation<int>.Starting), "(0; 0)", "(1; 3)", "[3; 4)")]
    [InlineData("(1; 4)", "(1; 3]", typeof(IntervalRelation<int>.Starting), "(0; 0)", "(1; 3]", "(3; 4)")]
    // Finishing
    [InlineData("(1; 4)", "(3; 4)", typeof(IntervalRelation<int>.Finishing), "(1; 3]", "(3; 4)", "(0; 0)")]
    [InlineData("(1; 4)", "[3; 4)", typeof(IntervalRelation<int>.Finishing), "(1; 3)", "[3; 4)", "(0; 0)")]
    // Equal
    [InlineData("(1; 4)", "(1; 4)", typeof(IntervalRelation<int>.Equal), "(0; 0)", "(1; 4)", "(0; 0)")]
    [InlineData("[1; 4)", "[1; 4)", typeof(IntervalRelation<int>.Equal), "(0; 0)", "[1; 4)", "(0; 0)")]
    [InlineData("(1; 4]", "(1; 4]", typeof(IntervalRelation<int>.Equal), "(0; 0)", "(1; 4]", "(0; 0)")]
    [InlineData("[1; 4]", "[1; 4]", typeof(IntervalRelation<int>.Equal), "(0; 0)", "[1; 4]", "(0; 0)")]
    // Legacy tests
    [InlineData("[1; 4)", "[5; 7)", typeof(IntervalRelation<int>.Disjunct), "[1; 4)", "(0; 0)", "[5; 7)")]
    [InlineData("[1; 4)", "[4; 7)", typeof(IntervalRelation<int>.Touching), "[1; 4)", "(0; 0)", "[4; 7)")]
    [InlineData("[4; 8)", "[4; 6)", typeof(IntervalRelation<int>.Starting), "(0; 0)", "[4; 6)", "[6; 8)")]
    [InlineData("[6; 8)", "[4; 8)", typeof(IntervalRelation<int>.Finishing), "[4; 6)", "[6; 8)", "(0; 0)")]
    [InlineData("[4; 7)", "[2; 10)", typeof(IntervalRelation<int>.Containing), "[2; 4)", "[4; 7)", "[7; 10)")]
    [InlineData("[4; 6]", "[6; 8)", typeof(IntervalRelation<int>.Overlapping), "[4; 6)", "[6; 6]", "(6; 8)")]
    [InlineData("[2; 7)", "[4; 9)", typeof(IntervalRelation<int>.Overlapping), "[2; 4)", "[4; 7)", "[7; 9)")]
    public void Relation(string ivText1, string ivText2, Type exactRelationType, string lowerDisjText, string overlapText, string upperDisjText)
    {
        var iv1 = Interval<int>.Parse(ivText1, int.Parse);
        var iv2 = Interval<int>.Parse(ivText2, int.Parse);

        var lowerDisjunct = Interval<int>.Parse(lowerDisjText, int.Parse);
        var overlapping = Interval<int>.Parse(overlapText, int.Parse);
        var upperDisjunct = Interval<int>.Parse(upperDisjText, int.Parse);

        var rel1 = iv1.RelationTo(iv2);
        var rel2 = iv2.RelationTo(iv1);

        Assert.Equal(exactRelationType, rel1.GetType());
        Assert.Equal(lowerDisjunct, rel1.LowerDisjunct);
        Assert.Equal(overlapping, rel1.Intersecting);
        Assert.Equal(upperDisjunct, rel1.UpperDisjunct);

        Assert.Equal(exactRelationType, rel2.GetType());
        Assert.Equal(lowerDisjunct, rel2.LowerDisjunct);
        Assert.Equal(overlapping, rel2.Intersecting);
        Assert.Equal(upperDisjunct, rel2.UpperDisjunct);
    }

    [Theory]
    // Empty intervals are equal
    [InlineData("(0; 0)", "(0; 0)", true)]
    [InlineData("(0; 0)", "(1; 1)", true)]
    [InlineData("[0; 0)", "(0; 0)", true)]
    [InlineData("[0; -1]", "(0; 0)", true)]
    // Non-empty tests
    [InlineData("(-oo; +oo)", "(-oo; +oo)", true)]
    [InlineData("(0; 1)", "(0; 1)", true)]
    [InlineData("[2; 4)", "[2; 4)", true)]
    [InlineData("(0; 2)", "(0; 1)", false)]
    [InlineData("[0; 1)", "[0; 1]", false)]
    [InlineData("[0; 2]", "(0; 2)", false)]
    public void Equality(string ivText1, string ivText2, bool eq)
    {
        var a = Interval<int>.Parse(ivText1, int.Parse);
        var b = Interval<int>.Parse(ivText2, int.Parse);

        if (eq) AssertEquals(a, b);
        else AssertNotEquals(a, b);
    }

    #region Comparers

    private static void AssertGreater(Bound<int> a, Bound<int> b)
    {
        Assert.True(a.CompareTo(b) > 0);
        Assert.True(BoundComparer<int>.Default.Compare(a, b) > 0);
        Assert.True(b.CompareTo(a) < 0);
        Assert.True(BoundComparer<int>.Default.Compare(b, a) < 0);
    }

    private static void AssertLess(Bound<int> a, Bound<int> b)
    {
        Assert.True(a.CompareTo(b) < 0);
        Assert.True(BoundComparer<int>.Default.Compare(a, b) < 0);
        Assert.True(b.CompareTo(a) > 0);
        Assert.True(BoundComparer<int>.Default.Compare(b, a) > 0);
    }

    private static void AssertEquals(Bound<int> a, Bound<int> b)
    {
        Assert.True(a == b);
        Assert.True(b == a);
        Assert.True(BoundComparer<int>.Default.Equals(a, b));
        Assert.True(BoundComparer<int>.Default.Equals(b, a));
        Assert.True(a.Equals(b));
        Assert.True(b.Equals(a));
        Assert.Equal(0, a.CompareTo(b));
        Assert.Equal(0, b.CompareTo(a));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    private static void AssertEquals(Interval<int> a, Interval<int> b)
    {
        Assert.True(a == b);
        Assert.True(b == a);
        Assert.True(IntervalComparer<int>.Default.Equals(a, b));
        Assert.True(IntervalComparer<int>.Default.Equals(b, a));
        Assert.True(a.Equals(b));
        Assert.True(b.Equals(a));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    private static void AssertNotEquals(Interval<int> a, Interval<int> b)
    {
        Assert.False(a == b);
        Assert.False(b == a);
        Assert.False(IntervalComparer<int>.Default.Equals(a, b));
        Assert.False(IntervalComparer<int>.Default.Equals(b, a));
        Assert.False(a.Equals(b));
        Assert.False(b.Equals(a));
    }

    #endregion
}
