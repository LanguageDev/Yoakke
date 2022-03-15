// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Tests;

public sealed class IntervalSetTests
{
    // NOTE: We use this wrapper type to check for proper comparer usage

    internal sealed class IntWrap
    {
        public int Value { get; set; }

        public IntWrap(int value)
        {
            this.Value = value;
        }
    }

    internal sealed class IntWrapComparer : IEqualityComparer<IntWrap>, IComparer<IntWrap>
    {
        public static IntWrapComparer Instance { get; } = new();
        public static EndpointComparer<IntWrap> EndpointInstance { get; } = new(Instance, Instance);
        public static IntervalComparer<IntWrap> IntervalInstance { get; } = new(EndpointInstance);

        public int Compare(IntWrap? x, IntWrap? y) => (x?.Value ?? 0) - (y?.Value ?? 0);
        public bool Equals(IntWrap? x, IntWrap? y) => x?.Value == y?.Value;
        public int GetHashCode([DisallowNull] IntWrap obj) => obj.Value.GetHashCode();
    }

    [InlineData("", 4, "[4; 4]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", 4, "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", 7, "(-oo; 5] U [7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", 12, "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", 10, "(-oo; 5] U (7; 9) U [10; 10] U [12; 16]")]
    [InlineData("(-oo; 0) U (0; +oo)", 0, "(-oo; +oo)")]
    [Theory]
    public void AddItem(string setText, int item, string resultText)
    {
        var originalSet = ParseIntervalSet(setText);
        var resultingSet = ParseIntervalSet(resultText);
        var shouldChange = !originalSet.SequenceEqual(resultingSet, originalSet.IntervalComparer);

        var newAdded = originalSet.Add(new IntWrap(item));

        AssertEquals(originalSet, resultingSet);
        Assert.Equal(shouldChange, newAdded);
    }

    [InlineData("", 4, "")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", 4, "(-oo; 4) U (4; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", 6, "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", 12, "(-oo; 5] U (7; 9) U (12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", 8, "(-oo; 5] U (7; 8) U (8; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", 14, "(-oo; 5] U (7; 9) U [12; 14) U (14; 16]")]
    [InlineData("(-oo; +oo)", 0, "(-oo; 0) U (0; +oo)")]
    [Theory]
    public void RemoveItem(string setText, int item, string resultText)
    {
        var originalSet = ParseIntervalSet(setText);
        var resultingSet = ParseIntervalSet(resultText);
        var shouldChange = !originalSet.SequenceEqual(resultingSet, originalSet.IntervalComparer);

        var oldRemoved = originalSet.Remove(new IntWrap(item));

        AssertEquals(originalSet, resultingSet);
        Assert.Equal(shouldChange, oldRemoved);
    }

    [InlineData("", "(2; 4]", "(2; 4]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[1; 1]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[6; 6]", "(-oo; 5] U [6; 6] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(0; 0)", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(3; 5]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(14; 15]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(7; 9)", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(5; 7]", "(-oo; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(9; 13)", "(-oo; 5] U (7; 9) U (9; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(10; 11]", "(-oo; 5] U (7; 9) U (10; 11] U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(9; 11]", "(-oo; 5] U (7; 9) U (9; 11] U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(8; 12)", "(-oo; 5] U (7; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(4; 8)", "(-oo; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[5; 8]", "(-oo; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[3; 13)", "(-oo; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; +oo)", "(-oo; +oo)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 24)", "(-oo; 24)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-2; 24]", "(-oo; 24]")]
    // Legacy tests
    [InlineData("", "[2; 3)", "[2; 3)")]
    [InlineData("[5; 7) U [12; 15)", "[2; 3)", "[2; 3) U [5; 7) U [12; 15)")]
    [InlineData("[5; 7) U [12; 15)", "[17; 19)", "[5; 7) U [12; 15) U [17; 19)")]
    [InlineData("[5; 7) U [12; 15)", "[8; 11)", "[5; 7) U [8; 11) U [12; 15)")]
    [InlineData("[5; 7) U [12; 15)", "[3; 5)", "[3; 7) U [12; 15)")]
    [InlineData("[5; 7) U [12; 15)", "[3; 6)", "[3; 7) U [12; 15)")]
    [InlineData("[5; 7) U [12; 15)", "[7; 12)", "[5; 15)")]
    [InlineData("[5; 7) U [12; 15)", "[3; 16)", "[3; 16)")]
    [Theory]
    public void AddInterval(string setText, string intervalText, string resultText)
    {
        var originalSet = ParseIntervalSet(setText);
        var resultingSet = ParseIntervalSet(resultText);
        var shouldChange = !originalSet.SequenceEqual(resultingSet, originalSet.IntervalComparer);
        var interval = ParseInterval(intervalText);

        var newAdded = originalSet.Add(interval);

        AssertEquals(originalSet, resultingSet);
        Assert.Equal(shouldChange, newAdded);
    }

    [InlineData("", "(2; 4]", "")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[1; 1]", "(-oo; 1) U (1; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[6; 6]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(0; 0)", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(3; 5]", "(-oo; 3] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(14; 15]", "(-oo; 5] U (7; 9) U [12; 14] U (15; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(7; 9)", "(-oo; 5] U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(5; 7]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(9; 13)", "(-oo; 5] U (7; 9) U [13; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(10; 11]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(9; 11]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(8; 12)", "(-oo; 5] U (7; 8] U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(4; 8)", "(-oo; 4] U [8; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[5; 8]", "(-oo; 5) U (8; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[3; 13)", "(-oo; 3) U [13; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; +oo)", "")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 24)", "")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-2; 24]", "(-oo; -2]")]
    [InlineData("(-oo; +oo)", "[0; 3)", "(-oo; 0) U [3; +oo)")]
    [Theory]
    public void RemoveInterval(string setText, string intervalText, string resultText)
    {
        var originalSet = ParseIntervalSet(setText);
        var resultingSet = ParseIntervalSet(resultText);
        var shouldChange = !originalSet.SequenceEqual(resultingSet, originalSet.IntervalComparer);
        var interval = ParseInterval(intervalText);

        var oldRemoved = originalSet.Remove(interval);

        AssertEquals(originalSet, resultingSet);
        Assert.Equal(shouldChange, oldRemoved);
    }

    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(5; 7] U [9; 12) U (16; +oo)")]
    [InlineData("(4; 23]", "(-oo; 4] U (23; +oo)")]
    [InlineData("[0; +oo)", "(-oo; 0)")]
    [InlineData("(2; 5) U (7; 9)", "(-oo; 2] U [5; 7] U [9; +oo)")]
    [InlineData("[2; 5] U [7; 9]", "(-oo; 2) U (5; 7) U (9; +oo)")]
    [InlineData("[1; 1]", "(-oo; 1) U (1; +oo)")]
    // Legacy tests
    [InlineData("", "(-oo; +oo)")]
    [InlineData("(-oo; +oo)", "")]
    [InlineData("[3; 5)", "(-oo; 3) U [5; +oo)")]
    [InlineData("[3; 5) U [7; 9)", "(-oo; 3) U [5; 7) U [9; +oo)")]
    [InlineData("[3; 5) U [7; 8) U [10; 14) U [18; 21)", "(-oo; 3) U [5; 7) U [8; 10) U [14; 18) U [21; +oo)")]
    [InlineData("[3; 5) U [7; 8) U [10; 14) U [18; 21) U [24; 26)", "(-oo; 3) U [5; 7) U [8; 10) U [14; 18) U [21; 24) U [26; +oo)")]
    [InlineData("(-oo; 5)", "[5; +oo)")]
    [InlineData("[5; +oo)", "(-oo; 5)")]
    [InlineData("[2; 4) U [6; 7) U [8; 11) U [14; +oo)", "(-oo; 2) U [4; 6) U [7; 8) U [11; 14)")]
    [InlineData("[2; 4) U [6; 7) U [8; 11) U [14; 16) U [18; +oo)", "(-oo; 2) U [4; 6) U [7; 8) U [11; 14) U [16; 18)")]
    [InlineData("(-oo; 2) U [6; 7) U [8; 11) U [14; 16)", "[2; 6) U [7; 8) U [11; 14) U [16; +oo)")]
    [InlineData("(-oo; 2) U [6; 7) U [8; 11) U [14; 16) U [18; 21)", "[2; 6) U [7; 8) U [11; 14) U [16; 18) U [21; +oo)")]
    [Theory]
    public void Complement(string setText, string resultText)
    {
        var originalSet = ParseIntervalSet(setText);
        var resultingSet = ParseIntervalSet(resultText);

        originalSet.Complement();

        AssertEquals(originalSet, resultingSet);
    }

    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", 4)]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", 12)]
    [InlineData("(-oo; +oo)", 0)]
    [Theory]
    public void ContainsItem(string setText, int item)
    {
        var set = ParseIntervalSet(setText);
        Assert.True(set.Contains(new IntWrap(item)));
    }

    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", 7)]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", 6)]
    [InlineData("(-oo; 0) U (0; +oo)", 0)]
    [Theory]
    public void DoesNotContainItem(string setText, int item)
    {
        var set = ParseIntervalSet(setText);
        Assert.False(set.Contains(new IntWrap(item)));
    }

    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(7; 9)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[2; 5]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[5; 5]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(13; 15]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(0; 0)")]
    [InlineData("(-oo; +oo)", "[-123; 54]")]
    [Theory]
    public void ContainsInterval(string setText, string intervalText)
    {
        var set = ParseIntervalSet(setText);
        var interval = ParseInterval(intervalText);

        Assert.Contains(interval, set);
    }

    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[2; 6]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(5; 7]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(7; 16]")]
    [Theory]
    public void DoesNotContainInterval(string setText, string intervalText)
    {
        var set = ParseIntervalSet(setText);
        var interval = ParseInterval(intervalText);

        // NOTE: Assert.DoesNotContain relies on a heuristic to find the comparer
#pragma warning disable xUnit2017 // Do not use Contains() to check if a value exists in a collection
        Assert.False(set.Contains(interval));
#pragma warning restore xUnit2017 // Do not use Contains() to check if a value exists in a collection
    }

    [InlineData("", "")]
    [InlineData("(-oo; 5] U (7; 9)", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("[12; 16]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(12; 16]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("[16; 16]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; +oo)")]
    [Theory]
    public void Subset(string set1Text, string set2Text)
    {
        var set1 = ParseIntervalSet(set1Text);
        var set2 = ParseIntervalSet(set2Text);

        Assert.True(set1.IsSubsetOf(set2));
        Assert.True(set2.IsSupersetOf(set1));
    }

    [InlineData("(-oo; +oo)", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("[3; 7) U [9; 13)", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("[9; 9]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(5; 7] U [9; 12)", "(-oo; 5] U (7; 9) U [12; 16]")]
    [Theory]
    public void NotSubset(string set1Text, string set2Text)
    {
        var set1 = ParseIntervalSet(set1Text);
        var set2 = ParseIntervalSet(set2Text);

        Assert.False(set1.IsSubsetOf(set2));
        Assert.False(set2.IsSupersetOf(set1));
    }

    [InlineData("(-oo; 5] U (7; 9)", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("[12; 16]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(12; 16]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("[16; 16]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; +oo)")]
    [Theory]
    public void ProperSubset(string set1Text, string set2Text)
    {
        var set1 = ParseIntervalSet(set1Text);
        var set2 = ParseIntervalSet(set2Text);

        // Non-proper
        Assert.True(set1.IsSubsetOf(set2));
        Assert.True(set2.IsSupersetOf(set1));
        // Proper
        Assert.True(set1.IsProperSubsetOf(set2));
        Assert.True(set2.IsProperSupersetOf(set1));
    }

    [InlineData("", "")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; +oo)", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("[3; 7) U [9; 13)", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("[9; 9]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(5; 7] U [9; 12)", "(-oo; 5] U (7; 9) U [12; 16]")]
    [Theory]
    public void NotProperSubset(string set1Text, string set2Text)
    {
        var set1 = ParseIntervalSet(set1Text);
        var set2 = ParseIntervalSet(set2Text);

        Assert.False(set1.IsProperSubsetOf(set2));
        Assert.False(set2.IsProperSupersetOf(set1));
    }

    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 5]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; +oo)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[2; 7)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[13; 14)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[1; 1]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[2; 20)")]
    [InlineData("[5; 5]", "[5; 5]")]
    [Theory]
    public void OverlappingSetWithInterval(string setText, string intervalText)
    {
        var set = ParseIntervalSet(setText);
        var interval = ParseInterval(intervalText);

        Assert.True(set.Overlaps(interval));
    }

    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[9; 12)")]
    [InlineData("", "(0; 0)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[6; 6]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(0; 0)")]
    [InlineData("", "(2; 6)")]
    [InlineData("[5; 5]", "[1; 1]")]
    [Theory]
    public void NonOverlappingSetWithInterval(string setText, string intervalText)
    {
        var set = ParseIntervalSet(setText);
        var interval = ParseInterval(intervalText);

        Assert.False(set.Overlaps(interval));
    }

    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 4) U (4; +oo)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[5; 7) U [18; 20)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[3; 7) U [18; 20)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[3; 10) U [18; 20)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[3; 10) U [13; 20)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[1; 1]")]
    [Theory]
    public void OverlappingSet(string set1Text, string set2Text)
    {
        var set1 = ParseIntervalSet(set1Text);
        var set2 = ParseIntervalSet(set2Text);

        Assert.True(set1.Overlaps(set2));
        Assert.True(set2.Overlaps(set1));
    }

    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(5; 7] U [9; 12) U (16; +oo)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(6; 7) U [18; 20)")]
    [Theory]
    public void NonOverlappingSet(string set1Text, string set2Text)
    {
        var set1 = ParseIntervalSet(set1Text);
        var set2 = ParseIntervalSet(set2Text);

        Assert.False(set1.Overlaps(set2));
        Assert.False(set2.Overlaps(set1));
    }

    [InlineData("", "")]
    [InlineData("[1; 1]", "[1; 1]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 5] U (7; 9) U [12; 16]")]
    [InlineData("(1; 2) U (3; 4)", "(1; 2) U (3; 4)")]
    [InlineData("[1; 2] U (3; 4)", "[1; 2] U (3; 4)")]
    [Theory]
    public void EqualSets(string set1Text, string set2Text)
    {
        var set1 = ParseIntervalSet(set1Text);
        var set2 = ParseIntervalSet(set2Text);

        AssertEquals(set1, set2);
    }

    [InlineData("[1; 2] U (3; 4)", "(1; 2] U (3; 4)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(7; 9) U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 5] U [12; 16]")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; 5] U (7; 9)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(-oo; +oo)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(5; 7] U [9; 12) U (16; +oo)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[5; 7) U [18; 20)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "(6; 7) U [18; 20)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[3; 7) U [18; 20)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[3; 10) U [18; 20)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[3; 10) U [13; 20)")]
    [InlineData("(-oo; 5] U (7; 9) U [12; 16]", "[1; 1]")]
    [Theory]
    public void InequalSets(string set1Text, string set2Text)
    {
        var set1 = ParseIntervalSet(set1Text);
        var set2 = ParseIntervalSet(set2Text);

        Assert.False(set1.SetEquals(set2));
        Assert.False(set2.SetEquals(set1));
    }

    private static void AssertEquals<T>(IntervalSet<T> a, IntervalSet<T> b)
    {
        // Same intervals
        Assert.True(a.SequenceEqual(b, a.IntervalComparer));
        // Set-equality
        Assert.True(a.SetEquals(b));
        Assert.True(b.SetEquals(a));
        // Subset-ness
        Assert.True(a.IsSubsetOf(b));
        Assert.True(b.IsSubsetOf(a));
        // Proper subset-ness
        Assert.False(a.IsProperSubsetOf(b));
        Assert.False(b.IsProperSubsetOf(a));
        // Superset-ness
        Assert.True(a.IsSupersetOf(b));
        Assert.True(b.IsSupersetOf(a));
        // Proper superset-ness
        Assert.False(a.IsProperSupersetOf(b));
        Assert.False(b.IsProperSupersetOf(a));
    }

    private static IntervalSet<IntWrap> ParseIntervalSet(string text)
    {
        text = text.Trim();
        var result = new IntervalSet<IntWrap>(IntWrapComparer.IntervalInstance);

        // Empty string means empty set
        if (text.Length == 0) return result;

        // Split by Union and parse intervals
        var intervalParts = text.Split('U');
        var intervals = intervalParts.Select(ParseInterval);

        // Construct the dense set
        foreach (var iv in intervals) result.Add(iv);

        // Check, if the constructed set is indeed the specified one
        Assert.True(intervals.SequenceEqual(result, result.IntervalComparer));

        return result;
    }

    internal static Interval<IntWrap> ParseInterval(string text) =>
        IntervalTests.ParseInterval(text, p => new IntWrap(int.Parse(p)));
}
