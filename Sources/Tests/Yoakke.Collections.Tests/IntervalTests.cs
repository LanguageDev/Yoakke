// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Tests
{
    [TestClass]
    public class IntervalTests
    {
        private static IEnumerable<object[]> IntervalToStringData { get; } = new object[][]
        {
            new object[] { Interval<int>.Full, "(-∞; +∞)" },
            new object[] { new Interval<int>(new LowerBound<int>.Exclusive(-12), new UpperBound<int>.Exclusive(56)), "(-12; 56)" },
            new object[] { new Interval<int>(new LowerBound<int>.Inclusive(-12), new UpperBound<int>.Exclusive(56)), "[-12; 56)" },
            new object[] { new Interval<int>(new LowerBound<int>.Exclusive(-12), new UpperBound<int>.Inclusive(56)), "(-12; 56]" },
            new object[] { new Interval<int>(new LowerBound<int>.Inclusive(-12), new UpperBound<int>.Inclusive(56)), "[-12; 56]" },
            new object[] { new Interval<int>(LowerBound<int>.Unbounded.Instance, new UpperBound<int>.Inclusive(56)), "(-∞; 56]" },
            new object[] { new Interval<int>(new LowerBound<int>.Inclusive(-12), UpperBound<int>.Unbounded.Instance), "[-12; +∞)" },
        };

        private static IEnumerable<object[]> IntervalParseData { get; } = new object[][]
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

        private static IEnumerable<object[]> LowerBoundLowerBoundComparisonData { get; } = new object[][]
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

        private static IEnumerable<object[]> UpperBoundUpperBoundComparisonData { get; } = new object[][]
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

        private static IEnumerable<object[]> LowerBoundUpperBoundComparisonData { get; } = new object[][]
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

        private static IEnumerable<object[]> TouchingBoundsData { get; } = new object[][]
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

        private static IEnumerable<object[]> ContainmentData { get; } = new object[][]
        {
            new object[] { "(-oo; +oo)", 0, true },
            new object[] { "(-oo; 0)", 0, false },
            new object[] { "(-oo; 0]", 0, true },
            new object[] { "[0; 0]", 0, true },
            new object[] { "(0; 0]", 0, false },
            new object[] { "[0; 0)", 0, false },
            new object[] { "(0; 0)", 0, false },
            new object[] { "[-1; 0)", 0, false },
            new object[] { "[-1; 0]", 0, true },
        };

        private static IEnumerable<object[]> EmptinessData { get; } = new object[][]
        {
            new object[] { "(-oo; +oo)", false },
            new object[] { "(-oo; 0)", false },
            new object[] { "(-oo; 0]", false },
            new object[] { "[0; 0]", false },
            new object[] { "(0; 0]", true },
            new object[] { "[0; 0)", true },
            new object[] { "(0; 0)", true },
            new object[] { "[-1; 0)", false },
            new object[] { "[-1; 0]", false },
            new object[] { "(0; -1)", true },
            new object[] { "[0; -1)", true },
            new object[] { "(0; -1]", true },
            new object[] { "[0; -1]", true },
        };

        private static IEnumerable<object[]> RelationData { get; } = new object[][]
        {
            // Empty intervals are equal
            new object[] { "(0; 0)", "(1; 1)", typeof(IntervalRelation<int>.Equal), "(0; 0)", "(0; 0)", "(0; 0)" },
            new object[] { "[0; 0)", "(0; 0)", typeof(IntervalRelation<int>.Equal), "(0; 0)", "(0; 0)", "(0; 0)" },
            new object[] { "[0; -1]", "(0; 0)", typeof(IntervalRelation<int>.Equal), "(0; 0)", "(0; 0)", "(0; 0)" },
            // Disjunct
            new object[] { "(1; 2)", "[3; 4)", typeof(IntervalRelation<int>.Disjunct), "(1; 2)", "(0; 0)", "[3; 4)" },
            new object[] { "(1; 2)", "(2; 4)", typeof(IntervalRelation<int>.Disjunct), "(1; 2)", "(0; 0)", "(2; 4)" },
            // Touching
            new object[] { "(1; 2)", "[2; 4)", typeof(IntervalRelation<int>.Touching), "(1; 2)", "(0; 0)", "[2; 4)" },
            new object[] { "(1; 2]", "(2; 4)", typeof(IntervalRelation<int>.Touching), "(1; 2]", "(0; 0)", "(2; 4)" },
            // Overlapping
            new object[] { "(1; 3)", "(2; 4)", typeof(IntervalRelation<int>.Overlapping), "(1; 2]", "(2; 3)", "[3; 4)" },
            new object[] { "(1; 3]", "(2; 4)", typeof(IntervalRelation<int>.Overlapping), "(1; 2]", "(2; 3]", "(3; 4)" },
            new object[] { "(1; 3)", "[2; 4)", typeof(IntervalRelation<int>.Overlapping), "(1; 2)", "[2; 3)", "[3; 4)" },
            new object[] { "(1; 3]", "[2; 4)", typeof(IntervalRelation<int>.Overlapping), "(1; 2)", "[2; 3]", "(3; 4)" },
        };

        [DataTestMethod]
        [DynamicData(nameof(IntervalToStringData))]
        public void IntervalToString(Interval<int> interval, string text)
        {
            Assert.AreEqual(text, interval.ToString());
        }

        [DataTestMethod]
        [DynamicData(nameof(IntervalParseData))]
        public void IntervalParse(string text, Interval<int> interval)
        {
            // Parse all ways
            var parsedFromString = Interval<int>.Parse(text, int.Parse);
            var parsedFromSpan = Interval<int>.Parse(text.AsSpan(), span => int.Parse(span));
            Assert.IsTrue(Interval<int>.TryParse(text, int.TryParse, out var tryParsedFromString));
            Assert.IsTrue(Interval<int>.TryParse(text.AsSpan(), int.TryParse, out var tryParsedFromSpan));

            // All must be equal to the expected
            Assert.AreEqual(interval, parsedFromString);
            Assert.AreEqual(interval, parsedFromSpan);
            Assert.AreEqual(interval, tryParsedFromString);
            Assert.AreEqual(interval, tryParsedFromSpan);
        }

        [DataTestMethod]
        [DynamicData(nameof(LowerBoundLowerBoundComparisonData))]
        public void LowerBoundLowerBoundCompare(LowerBound<int> a, LowerBound<int> b, int cmp)
        {
            if (cmp < 0) AssertLess(a, b);
            else if (cmp > 0) AssertGreater(a, b);
            else AssertEquals(a, b);
        }

        [DataTestMethod]
        [DynamicData(nameof(UpperBoundUpperBoundComparisonData))]
        public void UpperBoundUpperBoundCompare(UpperBound<int> a, UpperBound<int> b, int cmp)
        {
            if (cmp < 0) AssertLess(a, b);
            else if (cmp > 0) AssertGreater(a, b);
            else AssertEquals(a, b);
        }

        [DataTestMethod]
        [DynamicData(nameof(LowerBoundUpperBoundComparisonData))]
        public void LowerBoundUpperBoundCompare(LowerBound<int> a, UpperBound<int> b, int cmp)
        {
            if (cmp < 0) AssertLess(a, b);
            else AssertGreater(a, b);
        }

        [DataTestMethod]
        [DynamicData(nameof(TouchingBoundsData))]
        public void TouchingBounds(LowerBound<int> a, UpperBound<int> b, bool touching)
        {
            if (touching)
            {
                // Touching assertions
                Assert.IsTrue(a.IsTouching(b));
                Assert.IsTrue(b.IsTouching(a));
                Assert.IsTrue(BoundComparer<int>.Default.IsTouching(a, b));
                Assert.IsTrue(BoundComparer<int>.Default.IsTouching(b, a));

                // Touching endpoint equality
                AssertEquals(a.Touching!, b);
                AssertEquals(a, b.Touching!);
            }
            else
            {
                // Not touching assertions
                Assert.IsFalse(a.IsTouching(b));
                Assert.IsFalse(b.IsTouching(a));
                Assert.IsFalse(BoundComparer<int>.Default.IsTouching(a, b));
                Assert.IsFalse(BoundComparer<int>.Default.IsTouching(b, a));
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(ContainmentData))]
        public void Containment(string text, int value, bool contains)
        {
            var interval = Interval<int>.Parse(text, int.Parse);
            if (contains)
            {
                Assert.IsTrue(interval.Contains(value));
                Assert.IsTrue(IntervalComparer<int>.Default.Contains(interval, value));
            }
            else
            {
                Assert.IsFalse(interval.Contains(value));
                Assert.IsFalse(IntervalComparer<int>.Default.Contains(interval, value));
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(EmptinessData))]
        public void Emptiness(string text, bool empty)
        {
            var interval = Interval<int>.Parse(text, int.Parse);
            if (empty)
            {
                Assert.IsTrue(interval.IsEmpty);
                Assert.IsTrue(IntervalComparer<int>.Default.IsEmpty(interval));
            }
            else
            {
                Assert.IsFalse(interval.IsEmpty);
                Assert.IsFalse(IntervalComparer<int>.Default.IsEmpty(interval));
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(RelationData))]
        public void Relation(string ivText1, string ivText2, Type exactRelationType, string lowerDisjText, string overlapText, string upperDisjText)
        {
            var iv1 = Interval<int>.Parse(ivText1, int.Parse);
            var iv2 = Interval<int>.Parse(ivText2, int.Parse);

            var lowerDisjunct = Interval<int>.Parse(lowerDisjText, int.Parse);
            var overlapping = Interval<int>.Parse(overlapText, int.Parse);
            var upperDisjunct = Interval<int>.Parse(upperDisjText, int.Parse);

            var rel1 = iv1.RelationTo(iv2);
            var rel2 = iv2.RelationTo(iv1);

            Assert.AreEqual(exactRelationType, rel1.GetType());
            Assert.AreEqual(lowerDisjunct, rel1.LowerDisjunct);
            Assert.AreEqual(overlapping, rel1.Intersecting);
            Assert.AreEqual(upperDisjunct, rel1.UpperDisjunct);

            Assert.AreEqual(exactRelationType, rel2.GetType());
            Assert.AreEqual(lowerDisjunct, rel2.LowerDisjunct);
            Assert.AreEqual(overlapping, rel2.Intersecting);
            Assert.AreEqual(upperDisjunct, rel2.UpperDisjunct);
        }

        #region Comparers

        private static void AssertGreater(Bound<int> a, Bound<int> b)
        {
            Assert.IsTrue(a.CompareTo(b) > 0);
            Assert.IsTrue(BoundComparer<int>.Default.Compare(a, b) > 0);
            Assert.IsTrue(b.CompareTo(a) < 0);
            Assert.IsTrue(BoundComparer<int>.Default.Compare(b, a) < 0);
        }

        private static void AssertLess(Bound<int> a, Bound<int> b)
        {
            Assert.IsTrue(a.CompareTo(b) < 0);
            Assert.IsTrue(BoundComparer<int>.Default.Compare(a, b) < 0);
            Assert.IsTrue(b.CompareTo(a) > 0);
            Assert.IsTrue(BoundComparer<int>.Default.Compare(b, a) > 0);
        }

        private static void AssertEquals(Bound<int> a, Bound<int> b)
        {
            Assert.IsTrue(a == b);
            Assert.IsTrue(b == a);
            Assert.IsTrue(BoundComparer<int>.Default.Equals(a, b));
            Assert.IsTrue(BoundComparer<int>.Default.Equals(b, a));
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(a));
            Assert.AreEqual(0, a.CompareTo(b));
            Assert.AreEqual(0, b.CompareTo(a));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        #endregion
    }
}
