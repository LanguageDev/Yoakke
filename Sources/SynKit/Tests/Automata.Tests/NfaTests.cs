// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Linq;
using Xunit;
using Yoakke.SynKit.Automata.Sparse;

namespace Yoakke.SynKit.Automata.Tests;

public class NfaTests : AutomatonTestBase
{
    [Theory]
    [InlineData(new string[] { }, false)]
    [InlineData(new string[] { "0 -> A" }, false)]
    [InlineData(new string[] { "1 -> A, B" }, false)]
    [InlineData(new string[] { "0 -> A", "1 -> A, B" }, false)]
    [InlineData(new string[] { "1 -> A, B", "0 -> A, C" }, false)]
    [InlineData(new string[] { "1 -> A, B", "1 -> A, B" }, false)]
    [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "0 -> A" }, false)]
    [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "1 -> A, B, D" }, true)]
    [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "1 -> A, B, D", "1 -> A, B, D" }, true)]
    [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "1 -> A, B, D", "0 -> A, C, D" }, true)]
    [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "1 -> A, B, D", "0 -> A, C, D", "0 -> A, D" }, true)]
    [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "1 -> A, B, D", "0 -> A, C, D", "1 -> A, B, D" }, true)]
    [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "1 -> A, B, D", "0 -> A, C, D", "0 -> A, D", "0 -> A, D" }, true)]
    [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "1 -> A, B, D", "0 -> A, C, D", "0 -> A, D", "1 -> A, B, D" }, true)]
    public void Has101AcceptsTests(string[] transitionTexts, bool accepts)
    {
        // NOTE: We test the determinized DFA alongside
        var nfa = BuildHas101Nfa();
        var dfa = nfa.Determinize();

        var transitions = transitionTexts.Select(ParseTransition).ToList();

        var state = new StateSet<string>(nfa.InitialStates, EqualityComparer<string>.Default);
        foreach (var (inputChar, expectedNextText) in transitions)
        {
            var expectedNext = ParseStateSet(expectedNextText).ToHashSet();
            // NFA
            var nextNfState = nfa.GetTransitions(state, inputChar);
            Assert.True(expectedNext.SetEquals(nextNfState));
            // DFA
            Assert.True(dfa.TryGetTransition(state, inputChar, out var nextDfState));
            Assert.True(expectedNext.SetEquals(nextDfState));
            // Update state
            state = nextNfState;
        }

        var input = transitions.Select(t => t.Item1);
        Assert.Equal(accepts, nfa.Accepts(input));
    }

    [Fact]
    public void Has101Determinization()
    {
        var dfa = BuildHas101Nfa().Determinize();

        var expectedStates = new[] { "A", "A, B", "A, C", "A, B, D", "A, D", "A, C, D" }.Select(ParseStateSet);
        var gotStates = dfa.States.ToHashSet();

        var expectedAcceptingStates = new[] { "A, B, D", "A, D", "A, C, D" }.Select(ParseStateSet);
        var gotAcceptingStates = dfa.AcceptingStates.ToHashSet();

        Assert.True(gotStates.SetEquals(expectedStates));
        Assert.True(gotAcceptingStates.SetEquals(expectedAcceptingStates));

        Assert.Equal(12, dfa.Transitions.Count);
        AssertTransition(dfa, "A", '0', "A");
        AssertTransition(dfa, "A", '1', "A, B");
        AssertTransition(dfa, "A, B", '0', "A, C");
        AssertTransition(dfa, "A, B", '1', "A, B");
        AssertTransition(dfa, "A, C", '0', "A");
        AssertTransition(dfa, "A, C", '1', "A, B, D");
        AssertTransition(dfa, "A, B, D", '0', "A, C, D");
        AssertTransition(dfa, "A, B, D", '1', "A, B, D");
        AssertTransition(dfa, "A, C, D", '0', "A, D");
        AssertTransition(dfa, "A, C, D", '1', "A, B, D");
        AssertTransition(dfa, "A, D", '0', "A, D");
        AssertTransition(dfa, "A, D", '1', "A, B, D");
    }

    [Fact]
    public void Has101DeterminizationAndMinimization()
    {
        var dfa = BuildHas101Nfa().Determinize().Minimize(StateCombiner<string>.DefaultSetCombiner);

        var expectedStates = new[] { "A", "A, B", "A, C", "A, B, C, D" }.Select(ParseStateSet);
        var gotStates = dfa.States.ToHashSet();

        var expectedAcceptingStates = new[] { "A, B, C, D" }.Select(ParseStateSet);
        var gotAcceptingStates = dfa.AcceptingStates.ToHashSet();

        Assert.True(gotStates.SetEquals(expectedStates));
        Assert.True(gotAcceptingStates.SetEquals(expectedAcceptingStates));

        Assert.Equal(8, dfa.Transitions.Count);
        AssertTransition(dfa, "A", '0', "A");
        AssertTransition(dfa, "A", '1', "A, B");
        AssertTransition(dfa, "A, B", '0', "A, C");
        AssertTransition(dfa, "A, B", '1', "A, B");
        AssertTransition(dfa, "A, C", '0', "A");
        AssertTransition(dfa, "A, C", '1', "A, B, C, D");
        AssertTransition(dfa, "A, B, C, D", '0', "A, B, C, D");
        AssertTransition(dfa, "A, B, C, D", '1', "A, B, C, D");
    }

    private static Nfa<string, char> BuildHas101Nfa()
    {
        var nfa = new Nfa<string, char>();
        nfa.InitialStates.Add("A");
        nfa.AcceptingStates.Add("D");
        nfa.AddTransition("A", '0', "A");
        nfa.AddTransition("A", '1', "A");
        nfa.AddTransition("A", '1', "B");
        nfa.AddTransition("B", '0', "C");
        nfa.AddTransition("C", '1', "D");
        nfa.AddTransition("D", '0', "D");
        nfa.AddTransition("D", '1', "D");
        return nfa;
    }
}
