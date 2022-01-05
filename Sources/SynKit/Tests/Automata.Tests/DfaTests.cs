// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Linq;
using Xunit;
using Yoakke.SynKit.Automata.Sparse;

namespace Yoakke.SynKit.Automata.Tests;

public class DfaTests : AutomatonTestBase
{
    [Theory]
    [InlineData(new string[] { }, false)]
    [InlineData(new string[] { "a -> A" }, false)]
    [InlineData(new string[] { "b -> B" }, false)]
    [InlineData(new string[] { "a -> A", "a -> AA" }, false)]
    [InlineData(new string[] { "a -> A", "b -> AB" }, true)]
    [InlineData(new string[] { "b -> B", "b -> BB" }, false)]
    [InlineData(new string[] { "b -> B", "b -> BB", "a -> BA" }, true)]
    [InlineData(new string[] { "a -> A", "b -> AB", "a -> BA" }, true)]
    [InlineData(new string[] { "a -> A", "b -> AB", "a -> BA", "a -> AA" }, false)]
    [InlineData(new string[] { "a -> A", "b -> AB", "a -> BA", "b -> AB" }, true)]
    public void Last2DifferentAcceptsTests(string[] transitionTexts, bool accepts)
    {
        var dfa = BuildLast2DifferentCharsDfa();

        var transitions = transitionTexts.Select(ParseTransition).ToList();

        var state = dfa.InitialState;
        foreach (var (inputChar, expectedNextState) in transitions)
        {
            Assert.True(dfa.TryGetTransition(state!, inputChar, out var nextState));
            Assert.Equal(expectedNextState, nextState);
            state = nextState;
        }

        var input = transitions.Select(t => t.Item1);
        Assert.Equal(accepts, dfa.Accepts(input));
    }

    [Theory]
    [InlineData(new string[] { }, false)]
    [InlineData(new string[] { "a -> A, AA" }, false)]
    [InlineData(new string[] { "b -> B, BB" }, false)]
    [InlineData(new string[] { "a -> A, AA", "a -> A, AA" }, false)]
    [InlineData(new string[] { "a -> A, AA", "b -> AB" }, true)]
    [InlineData(new string[] { "b -> B, BB", "b -> B, BB" }, false)]
    [InlineData(new string[] { "b -> B, BB", "b -> B, BB", "a -> BA" }, true)]
    [InlineData(new string[] { "a -> A, AA", "b -> AB", "a -> BA" }, true)]
    [InlineData(new string[] { "a -> A, AA", "b -> AB", "a -> BA", "a -> A, AA" }, false)]
    [InlineData(new string[] { "a -> A, AA", "b -> AB", "a -> BA", "b -> AB" }, true)]
    public void Last2DifferentAcceptsMinimizedTests(string[] transitionTexts, bool accepts)
    {
        var dfa = BuildLast2DifferentCharsDfa().Minimize();

        var transitions = transitionTexts.Select(ParseTransition).ToList();

        var state = dfa.InitialState;
        foreach (var (inputChar, expectedNextStateText) in transitions)
        {
            var expectedNextState = ParseStateSet(expectedNextStateText);
            Assert.True(dfa.TryGetTransition(state!, inputChar, out var nextState));
            Assert.Equal(expectedNextState, nextState);
            state = nextState;
        }

        var input = transitions.Select(t => t.Item1);
        Assert.Equal(accepts, dfa.Accepts(input));
    }

    [Fact]
    public void Last2DifferentMinimize()
    {
        var dfa = BuildLast2DifferentCharsDfa().Minimize();

        var expectedStates = new[] { "S", "A, AA", "B, BB", "AB", "BA" }.Select(ParseStateSet);
        var gotStates = dfa.States.ToHashSet();

        var expectedAcceptingStates = new[] { "AB", "BA" }.Select(ParseStateSet);
        var gotAcceptingStates = dfa.AcceptingStates.ToHashSet();

        Assert.True(gotStates.SetEquals(expectedStates));
        Assert.True(gotAcceptingStates.SetEquals(expectedAcceptingStates));

        Assert.Equal(10, dfa.Transitions.Count);
        AssertTransition(dfa, "S", 'a', "A, AA");
        AssertTransition(dfa, "S", 'b', "B, BB");
        AssertTransition(dfa, "A, AA", 'a', "A, AA");
        AssertTransition(dfa, "A, AA", 'b', "AB");
        AssertTransition(dfa, "B, BB", 'a', "BA");
        AssertTransition(dfa, "B, BB", 'b', "B, BB");
        AssertTransition(dfa, "AB", 'a', "BA");
        AssertTransition(dfa, "AB", 'b', "B, BB");
        AssertTransition(dfa, "BA", 'a', "A, AA");
        AssertTransition(dfa, "BA", 'b', "AB");
    }

    [Fact]
    public void CompleteSimple()
    {
        // We build a DFA that accepts a*
        // And we complete it over { a, b }
        var dfa = new Dfa<char, char>();
        dfa.InitialState = 'q';
        dfa.AcceptingStates.Add('q');
        dfa.AddTransition('q', 'a', 'q');
        dfa.Alphabet.Add('b');

        Assert.True(dfa.Complete('t'));
        var expectedStates = new[] { 'q', 't' }.ToHashSet();
        Assert.True(expectedStates.SetEquals(dfa.States));
        Assert.Equal(4, dfa.Transitions.Count);
        AssertTransition(dfa, 'q', 'a', 'q');
        AssertTransition(dfa, 'q', 'b', 't');
        AssertTransition(dfa, 't', 'a', 't');
        AssertTransition(dfa, 't', 'b', 't');
    }

    [Fact]
    public void MinimizeIncomplete()
    {
        // We build a simple, incomplete DFA
        var dfa = new Dfa<string, char>();
        dfa.InitialState = "A";
        dfa.AcceptingStates.Add("D");
        dfa.AddTransition("A", '0', "B");
        dfa.AddTransition("A", '1', "C");
        dfa.AddTransition("B", '0', "C");
        dfa.AddTransition("C", '0', "B");
        dfa.AddTransition("C", '1', "D");

        // Minimize it
        var minDfa = dfa.Minimize();

        var expectedStates = new[] { "A", "B", "C", "D" }.Select(ParseStateSet);
        var gotStates = minDfa.States.ToHashSet();

        var expectedAcceptingStates = new[] { "D" }.Select(ParseStateSet);
        var gotAcceptingStates = minDfa.AcceptingStates.ToHashSet();

        Assert.True(gotStates.SetEquals(expectedStates));
        Assert.True(gotAcceptingStates.SetEquals(expectedAcceptingStates));

        Assert.Equal(5, minDfa.Transitions.Count);
        AssertTransition(minDfa, "A", '0', "B");
        AssertTransition(minDfa, "A", '1', "C");
        AssertTransition(minDfa, "B", '0', "C");
        AssertTransition(minDfa, "C", '0', "B");
        AssertTransition(minDfa, "C", '1', "D");
    }

    [Fact]
    public void MinimizeIncompleteIssue()
    {
        var dfa = new Dfa<string, char>();
        dfa.InitialState = "q0";
        dfa.AcceptingStates.Add("q3");
        dfa.AcceptingStates.Add("q4");
        dfa.AddTransition("q0", '0', "q1");
        dfa.AddTransition("q1", 'x', "q2");
        dfa.AddTransition("q2", '0', "q3");
        dfa.AddTransition("q3", '0', "q4");
        dfa.AddTransition("q4", '0', "q4");

        var minDfa = dfa.Minimize();

        var expectedStates = new[] { "q0", "q1", "q2", "q3, q4" }.Select(ParseStateSet);
        var gotStates = minDfa.States.ToHashSet();

        var expectedAcceptingStates = new[] { "q3, q4" }.Select(ParseStateSet);
        var gotAcceptingStates = minDfa.AcceptingStates.ToHashSet();

        Assert.True(gotStates.SetEquals(expectedStates));
        Assert.True(gotAcceptingStates.SetEquals(expectedAcceptingStates));

        Assert.Equal(4, minDfa.Transitions.Count);
        AssertTransition(minDfa, "q0", '0', "q1");
        AssertTransition(minDfa, "q1", 'x', "q2");
        AssertTransition(minDfa, "q2", '0', "q3, q4");
        AssertTransition(minDfa, "q3, q4", '0', "q3, q4");
    }

    private static Dfa<string, char> BuildLast2DifferentCharsDfa()
    {
        var dfa = new Dfa<string, char>();
        dfa.InitialState = "S";
        dfa.AcceptingStates.Add("AB");
        dfa.AcceptingStates.Add("BA");
        dfa.AddTransition("S", 'a', "A");
        dfa.AddTransition("S", 'b', "B");
        dfa.AddTransition("A", 'a', "AA");
        dfa.AddTransition("A", 'b', "AB");
        dfa.AddTransition("AA", 'a', "AA");
        dfa.AddTransition("AA", 'b', "AB");
        dfa.AddTransition("B", 'a', "BA");
        dfa.AddTransition("B", 'b', "BB");
        dfa.AddTransition("BB", 'a', "BA");
        dfa.AddTransition("BB", 'b', "BB");
        dfa.AddTransition("AB", 'a', "BA");
        dfa.AddTransition("AB", 'b', "BB");
        dfa.AddTransition("BA", 'a', "AA");
        dfa.AddTransition("BA", 'b', "AB");
        return dfa;
    }
}
