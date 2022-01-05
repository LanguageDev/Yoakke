// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Yoakke.SynKit.Automata.Sparse;

namespace Yoakke.SynKit.Automata.Tests;

public class EpsilonNfaTests : AutomatonTestBase
{
    [Theory]
    [InlineData(new string[] { }, false)]
    [InlineData(new string[] { "0 -> A" }, false)]
    [InlineData(new string[] { "1 -> A, B, C" }, false)]
    [InlineData(new string[] { "0 -> A", "0 -> A" }, false)]
    [InlineData(new string[] { "1 -> A, B, C", "0 -> A, C" }, false)]
    [InlineData(new string[] { "1 -> A, B, C", "1 -> A, B, C, D" }, true)]
    [InlineData(new string[] { "1 -> A, B, C", "0 -> A, C", "0 -> A" }, false)]
    [InlineData(new string[] { "1 -> A, B, C", "0 -> A, C", "1 -> A, B, C, D" }, true)]
    [InlineData(new string[] { "1 -> A, B, C", "0 -> A, C", "1 -> A, B, C, D", "0 -> A, C, D" }, true)]
    [InlineData(new string[] { "1 -> A, B, C", "0 -> A, C", "1 -> A, B, C, D", "1 -> A, B, C, D" }, true)]
    [InlineData(new string[] { "1 -> A, B, C", "0 -> A, C", "1 -> A, B, C, D", "0 -> A, C, D", "0 -> A, D" }, true)]
    [InlineData(new string[] { "1 -> A, B, C", "0 -> A, C", "1 -> A, B, C, D", "0 -> A, C, D", "1 -> A, B, C, D" }, true)]
    [InlineData(new string[] { "1 -> A, B, C", "0 -> A, C", "1 -> A, B, C, D", "0 -> A, C, D", "0 -> A, D", "0 -> A, D" }, true)]
    [InlineData(new string[] { "1 -> A, B, C", "0 -> A, C", "1 -> A, B, C, D", "0 -> A, C, D", "0 -> A, D", "1 -> A, B, C, D" }, true)]
    public void Has101Or11NfaAcceptsTests(string[] transitionTexts, bool accepts)
    {
        // NOTE: We test the epsilonless NFA and the determinized DFA alongside
        var nfa = BuildHas101Or11Nfa();
        // var epsilonlessNfa = nfa.EliminateEpsilonTransitions();
        var dfa = nfa.Determinize();

        var transitions = transitionTexts.Select(ParseTransition).ToList();

        var state = new StateSet<string>(nfa.InitialStates, EqualityComparer<string>.Default);
        // var epsilonlessState = new StateSet<StateSet<string>>(new[] { state });
        foreach (var (inputChar, expectedNextText) in transitions)
        {
            var expectedNext = ParseStateSet(expectedNextText).ToHashSet();
            // NFA
            var nextNfState = nfa.GetTransitions(state, inputChar);
            Assert.True(expectedNext.SetEquals(nextNfState));
            // Epsilonless NFA
            // var nextEpsilonlessNfState = epsilonlessNfa.GetTransitions(epsilonlessState, inputChar);
            // Assert.True(expectedNext.SetEquals(nextEpsilonlessNfState.SelectMany(s => s).ToHashSet()));
            // DFA
            Assert.True(dfa.TryGetTransition(state, inputChar, out var nextDfState));
            Assert.True(expectedNext.SetEquals(nextDfState));
            // Update state
            state = nextNfState;
        }

        var input = transitions.Select(t => t.Item1);
        Assert.Equal(accepts, nfa.Accepts(input));
        // Assert.Equal(accepts, epsilonlessNfa.Accepts(input));
        Assert.Equal(accepts, dfa.Accepts(input));
    }

    [Fact]
    public void SampleEpsilonElimination()
    {
        var nfa = BuildEpsilonEliminationSampleNfa();

        Assert.True(nfa.EliminateEpsilonTransitions());

        var expectedStates = new[] { "q0", "q1", "q2", "q3", "q4" };
        var gotStates = nfa.States.ToHashSet();

        var expectedAcceptingStates = new[] { "q0", "q2" };
        var gotAcceptingStates = nfa.AcceptingStates.ToHashSet();

        var expectedInitialStates = new[] { "q0", "q2" };
        var gotInitialStates = nfa.InitialStates.ToHashSet();

        Assert.True(gotStates.SetEquals(expectedStates));
        Assert.True(gotAcceptingStates.SetEquals(expectedAcceptingStates));
        Assert.True(gotInitialStates.SetEquals(expectedInitialStates));

        Assert.Equal(8, nfa.Transitions.Count);
        Assert.Equal(0, nfa.EpsilonTransitions.Count);
        AssertTransitions(nfa, "q0", '1', new[] { "q1", "q4" });
        AssertTransitions(nfa, "q1", '1', new[] { "q0" });
        AssertTransitions(nfa, "q0", '0', new[] { "q3" });
        AssertTransitions(nfa, "q2", '0', new[] { "q3" });
        AssertTransitions(nfa, "q3", '0', new[] { "q2" });
        AssertTransitions(nfa, "q2", '1', new[] { "q4" });
        AssertTransitions(nfa, "q4", '0', new[] { "q2" });
    }

    [Fact]
    public void Has101Or11Determinization()
    {
        var dfa = BuildHas101Or11Nfa().Determinize();

        var expectedStates = new[] { "A", "A, B, C", "A, C", "A, B, C, D", "A, C, D", "A, D" }.Select(ParseStateSet);
        var gotStates = dfa.States.ToHashSet();

        var expectedAcceptingStates = new[] { "A, B, C, D", "A, C, D", "A, D" }.Select(ParseStateSet);
        var gotAcceptingStates = dfa.AcceptingStates.ToHashSet();

        Assert.True(gotStates.SetEquals(expectedStates));
        Assert.True(gotAcceptingStates.SetEquals(expectedAcceptingStates));

        Assert.Equal(12, dfa.Transitions.Count);
        AssertTransition(dfa, "A", '0', "A");
        AssertTransition(dfa, "A", '1', "A, B, C");
        AssertTransition(dfa, "A, B, C", '0', "A, C");
        AssertTransition(dfa, "A, B, C", '1', "A, B, C, D");
        AssertTransition(dfa, "A, C", '0', "A");
        AssertTransition(dfa, "A, C", '1', "A, B, C, D");
        AssertTransition(dfa, "A, B, C, D", '0', "A, C, D");
        AssertTransition(dfa, "A, B, C, D", '1', "A, B, C, D");
        AssertTransition(dfa, "A, C, D", '0', "A, D");
        AssertTransition(dfa, "A, C, D", '1', "A, B, C, D");
        AssertTransition(dfa, "A, D", '0', "A, D");
        AssertTransition(dfa, "A, D", '1', "A, B, C, D");
    }

    [Fact]
    public void Has101Or11DeterminizationAndMinimization()
    {
        var dfa = BuildHas101Or11Nfa().Determinize().Minimize(StateCombiner<string>.DefaultSetCombiner);

        var expectedStates = new[] { "A", "A, B, C", "A, C", "A, B, C, D" }.Select(ParseStateSet);
        var gotStates = dfa.States.ToHashSet();

        var expectedAcceptingStates = new[] { "A, B, C, D" }.Select(ParseStateSet);
        var gotAcceptingStates = dfa.AcceptingStates.ToHashSet();

        Assert.True(gotStates.SetEquals(expectedStates));
        Assert.True(gotAcceptingStates.SetEquals(expectedAcceptingStates));

        Assert.Equal(8, dfa.Transitions.Count);
        AssertTransition(dfa, "A", '0', "A");
        AssertTransition(dfa, "A", '1', "A, B, C");
        AssertTransition(dfa, "A, B, C", '0', "A, C");
        AssertTransition(dfa, "A, B, C", '1', "A, B, C, D");
        AssertTransition(dfa, "A, C", '0', "A");
        AssertTransition(dfa, "A, C", '1', "A, B, C, D");
        AssertTransition(dfa, "A, B, C, D", '0', "A, B, C, D");
        AssertTransition(dfa, "A, B, C, D", '1', "A, B, C, D");
    }

    private static Nfa<string, char> BuildHas101Or11Nfa()
    {
        var nfa = new Nfa<string, char>();
        nfa.InitialStates.Add("A");
        nfa.AcceptingStates.Add("D");
        nfa.AddTransition("A", '0', "A");
        nfa.AddTransition("A", '1', "A");
        nfa.AddTransition("A", '1', "B");
        nfa.AddTransition("B", '0', "C");
        nfa.AddEpsilonTransition("B", "C");
        nfa.AddTransition("C", '1', "D");
        nfa.AddTransition("D", '0', "D");
        nfa.AddTransition("D", '1', "D");
        return nfa;
    }

    private static Nfa<string, char> BuildEpsilonEliminationSampleNfa()
    {
        var nfa = new Nfa<string, char>();
        nfa.InitialStates.Add("q0");
        nfa.AcceptingStates.Add("q2");
        nfa.AddTransition("q0", '1', "q1");
        nfa.AddTransition("q1", '1', "q0");
        nfa.AddEpsilonTransition("q0", "q2");
        nfa.AddTransition("q2", '0', "q3");
        nfa.AddTransition("q3", '0', "q2");
        nfa.AddTransition("q2", '1', "q4");
        nfa.AddTransition("q4", '0', "q2");
        return nfa;
    }
}
