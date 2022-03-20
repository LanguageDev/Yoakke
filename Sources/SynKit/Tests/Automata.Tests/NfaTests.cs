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

public sealed class NfaTests
{
    [InlineData("101")]
    [InlineData("1011")]
    [InlineData("1010")]
    [InlineData("10100")]
    [InlineData("10101")]
    [InlineData("101000")]
    [InlineData("101001")]
    [InlineData("1111111001011100")]
    [InlineData("000001111100001111000101")]
    [Theory]
    public void Has101Accepts(string input)
    {
        // NOTE: We test the determinized DFA alongside
        var nfa = BuildHas101Nfa();
        var dfa = nfa.Determinize();

        Assert.True(nfa.Accepts(input));
        Assert.True(dfa.Accepts(input));
    }

    [InlineData("")]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("01")]
    [InlineData("10")]
    [InlineData("11")]
    [InlineData("100")]
    [InlineData("1001")]
    [InlineData("1000001")]
    [InlineData("1111111000001")]
    [InlineData("11111110000011111111")]
    [InlineData("111111100100011111111")]
    [InlineData("1111111001111100011111111")]
    [Theory]
    public void Has101AcceptsRejects(string input)
    {
        // NOTE: We test the determinized DFA alongside
        var nfa = BuildHas101Nfa();
        var dfa = nfa.Determinize();

        Assert.False(nfa.Accepts(input));
        Assert.False(dfa.Accepts(input));
    }

    [Fact]
    public void Has101Determinization()
    {
        var dfa = BuildHas101Nfa().Determinize();

        Assert.True(dfa.States.ToHashSet().SetEquals(new[]
        {
            SetOf("A"),
            SetOf("A", "B"),
            SetOf("A", "C"),
            SetOf("A", "B", "D"),
            SetOf("A", "D"),
            SetOf("A", "C", "D"),
        }));

        Assert.True(dfa.AcceptingStates.ToHashSet().SetEquals(new[]
        {
            SetOf("A", "B", "D"),
            SetOf("A", "D"),
            SetOf("A", "C", "D"),
        }));

        Assert.Equal(12, dfa.Transitions.Count);
        Assert.Contains(Tran(SetOf("A"), '0', SetOf("A")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A"), '1', SetOf("A", "B")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B"), '0', SetOf("A", "C")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B"), '1', SetOf("A", "B")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "C"), '0', SetOf("A")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "C"), '1', SetOf("A", "B", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B", "D"), '0', SetOf("A", "C", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B", "D"), '1', SetOf("A", "B", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "C", "D"), '0', SetOf("A", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "C", "D"), '1', SetOf("A", "B", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "D"), '0', SetOf("A", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "D"), '1', SetOf("A", "B", "D")), dfa.Transitions);
    }

    [Fact]
    public void Has101DeterminizationAndMinimization()
    {
        var dfa = BuildHas101Nfa().Determinize().Minimize();

        Assert.True(dfa.States.ToHashSet().SetEquals(new[]
        {
            SetOf("A"),
            SetOf("A", "B"),
            SetOf("A", "C"),
            SetOf("A", "B", "C", "D"),
        }));

        Assert.True(dfa.AcceptingStates.ToHashSet().SetEquals(new[]
        {
            SetOf("A", "B", "C", "D"),
        }));
        
        Assert.Equal(8, dfa.Transitions.Count);
        Assert.Contains(Tran(SetOf("A"), '0', SetOf("A")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A"), '1', SetOf("A", "B")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B"), '0', SetOf("A", "C")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B"), '1', SetOf("A", "B")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "C"), '0', SetOf("A")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "C"), '1', SetOf("A", "B", "C", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B", "C", "D"), '0', SetOf("A", "B", "C", "D")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "B", "C", "D"), '1', SetOf("A", "B", "C", "D")), dfa.Transitions);
    }

    private static Nfa<string, char> BuildHas101Nfa()
    {
        var nfa = new Nfa<string, char>();
        nfa.InitialStates.Add("A");
        nfa.AcceptingStates.Add("D");
        nfa.Transitions.Add("A", '0', "A");
        nfa.Transitions.Add("A", '1', "A");
        nfa.Transitions.Add("A", '1', "B");
        nfa.Transitions.Add("B", '0', "C");
        nfa.Transitions.Add("C", '1', "D");
        nfa.Transitions.Add("D", '0', "D");
        nfa.Transitions.Add("D", '1', "D");
        return nfa;
    }

    private static ByValueSet<T> SetOf<T>(params T[] vs) => new(vs, comparer: null);

    private static Transition<TState, TSymbol> Tran<TState, TSymbol>(TState from, TSymbol on, TState to) =>
        new(from, on, to);
}
