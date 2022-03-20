// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Yoakke.Collections;

namespace Yoakke.SynKit.Automata.Tests;

public sealed class EpsilonNfaTests
{
    [InlineData("11")]
    [InlineData("101")]
    [InlineData("1010")]
    [InlineData("1011")]
    [InlineData("10100")]
    [InlineData("10101")]
    [InlineData("101000")]
    [InlineData("101001")]
    [InlineData("00011")]
    [InlineData("000101")]
    [InlineData("000010000101")]
    [InlineData("00001000011")]
    [Theory]
    public void Has101Or11NfaAccepts(string input)
    {
        // NOTE: We test the epsilonless NFA and the determinized DFA alongside
        var nfa = BuildHas101Or11Nfa();
        var dfa = nfa.Determinize();
        var epsilonlessNfa = BuildHas101Or11Nfa();
        epsilonlessNfa.EliminateEpsilonTransitions();
        var minDfa = dfa.Minimize();

        Assert.True(nfa.Accepts(input));
        Assert.True(epsilonlessNfa.Accepts(input));
        Assert.True(dfa.Accepts(input));
        Assert.True(minDfa.Accepts(input));
    }

    [InlineData("")]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("00")]
    [InlineData("10")]
    [InlineData("100")]
    [InlineData("00100")]
    [InlineData("00100100000")]
    [InlineData("0010010000010000")]
    [Theory]
    public void Has101Or11NfaRejects(string input)
    {
        // NOTE: We test the epsilonless NFA and the determinized DFA alongside
        var nfa = BuildHas101Or11Nfa();
        var dfa = nfa.Determinize();
        var epsilonlessNfa = BuildHas101Or11Nfa();
        epsilonlessNfa.EliminateEpsilonTransitions();
        var minDfa = dfa.Minimize();

        Assert.False(nfa.Accepts(input));
        Assert.False(epsilonlessNfa.Accepts(input));
        Assert.False(dfa.Accepts(input));
        Assert.False(minDfa.Accepts(input));
    }

    [Fact]
    public void SampleEpsilonElimination()
    {
        var nfa = BuildEpsilonEliminationSampleNfa();

        Assert.True(nfa.EliminateEpsilonTransitions());

        Assert.True(nfa.InitialStates.ToHashSet().SetEquals(new[]
        {
            "q0", "q2",
        }));

        Assert.True(nfa.AcceptingStates.ToHashSet().SetEquals(new[]
        {
            "q0", "q2",
        }));

        Assert.True(nfa.States.ToHashSet().SetEquals(new[]
        {
            "q0", "q1", "q2", "q3", "q4"
        }));

        Assert.Equal(8, nfa.Transitions.Count);
        Assert.Equal(0, nfa.EpsilonTransitions.Count);
        Assert.Contains(Tran("q0", '1', "q1"), nfa.Transitions);
        Assert.Contains(Tran("q0", '1', "q4"), nfa.Transitions);
        Assert.Contains(Tran("q1", '1', "q0"), nfa.Transitions);
        Assert.Contains(Tran("q0", '0', "q3"), nfa.Transitions);
        Assert.Contains(Tran("q2", '0', "q3"), nfa.Transitions);
        Assert.Contains(Tran("q3", '0', "q2"), nfa.Transitions);
        Assert.Contains(Tran("q2", '1', "q4"), nfa.Transitions);
        Assert.Contains(Tran("q4", '0', "q2"), nfa.Transitions);
    }

    [Fact]
    public void Has101Or11Determinization()
    {
        var dfa = BuildHas101Or11Nfa().Determinize();

        Assert.True(dfa.AcceptingStates.ToHashSet().SetEquals(new[]
        {
            SetOf("A", "B", "C", "D"),
            SetOf("A", "C", "D"),
            SetOf("A", "D"),
        }));

        Assert.True(dfa.States.ToHashSet().SetEquals(new[]
        {
            SetOf("A"),
            SetOf("A", "B", "C"),
            SetOf("A", "C"),
            SetOf("A", "B", "C", "D"),
            SetOf("A", "C", "D"),
            SetOf("A", "D"),
        }));

        Assert.Equal(12, dfa.Transitions.Count);
        Assert.Contains(Tran(SetOf("A"), '0', SetOf("A")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A"), '1', SetOf("A", "B", "C")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B", "C"), '0', SetOf("A", "C")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B", "C"), '1', SetOf("A", "B", "C", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "C"), '0', SetOf("A")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "C"), '1', SetOf("A", "B", "C", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B", "C", "D"), '0', SetOf("A", "C", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B", "C", "D"), '1', SetOf("A", "B", "C", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "C", "D"), '0', SetOf("A", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "C", "D"), '1', SetOf("A", "B", "C", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "D"), '0', SetOf("A", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "D"), '1', SetOf("A", "B", "C", "D")), dfa.Transitions);
    }

    [Fact]
    public void Has101Or11DeterminizationAndMinimization()
    {
        var dfa = BuildHas101Or11Nfa().Determinize().Minimize();

        Assert.True(dfa.AcceptingStates.ToHashSet().SetEquals(new[]
        {
            SetOf("A", "B", "C", "D"),
        }));

        Assert.True(dfa.States.ToHashSet().SetEquals(new[]
        {
            SetOf("A"),
            SetOf("A", "B", "C"),
            SetOf("A", "C"),
            SetOf("A", "B", "C", "D"),
        }));

        Assert.Equal(8, dfa.Transitions.Count);
        Assert.Contains(Tran(SetOf("A"), '0', SetOf("A")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A"), '1', SetOf("A", "B", "C")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B", "C"), '0', SetOf("A", "C")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B", "C"), '1', SetOf("A", "B", "C", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "C"), '0', SetOf("A")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "C"), '1', SetOf("A", "B", "C", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B", "C", "D"), '0', SetOf("A", "B", "C", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B", "C", "D"), '1', SetOf("A", "B", "C", "D")), dfa.Transitions);
    }

    private static Nfa<string, char> BuildHas101Or11Nfa()
    {
        var nfa = new Nfa<string, char>();
        nfa.InitialStates.Add("A");
        nfa.AcceptingStates.Add("D");
        nfa.Transitions.Add("A", '0', "A");
        nfa.Transitions.Add("A", '1', "A");
        nfa.Transitions.Add("A", '1', "B");
        nfa.Transitions.Add("B", '0', "C");
        nfa.EpsilonTransitions.Add("B", "C");
        nfa.Transitions.Add("C", '1', "D");
        nfa.Transitions.Add("D", '0', "D");
        nfa.Transitions.Add("D", '1', "D");
        return nfa;
    }

    private static Nfa<string, char> BuildEpsilonEliminationSampleNfa()
    {
        var nfa = new Nfa<string, char>();
        nfa.InitialStates.Add("q0");
        nfa.AcceptingStates.Add("q2");
        nfa.Transitions.Add("q0", '1', "q1");
        nfa.Transitions.Add("q1", '1', "q0");
        nfa.EpsilonTransitions.Add("q0", "q2");
        nfa.Transitions.Add("q2", '0', "q3");
        nfa.Transitions.Add("q3", '0', "q2");
        nfa.Transitions.Add("q2", '1', "q4");
        nfa.Transitions.Add("q4", '0', "q2");
        return nfa;
    }

    private static ByValueSet<T> SetOf<T>(params T[] vs) => new(vs, comparer: null);

    private static Transition<TState, TSymbol> Tran<TState, TSymbol>(TState from, TSymbol on, TState to) =>
        new(from, on, to);
}
