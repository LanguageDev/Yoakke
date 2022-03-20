using System.Linq;
using Xunit;
using Yoakke.Collections;
using Yoakke.SynKit.Automata;

namespace Yoakke.SynKit.Automata.Tests;

public sealed class DfaTests
{
    [InlineData("ab")]
    [InlineData("ba")]
    [InlineData("aba")]
    [InlineData("abab")]
    [InlineData("baba")]
    [InlineData("aaaba")]
    [InlineData("bbbab")]
    [InlineData("bbbababbbaab")]
    [Theory]
    public void LastTwoDifferentCharsAccepts(string input)
    {
        var dfa = BuildLastTwoDifferentCharsDfa();
        Assert.True(dfa.Accepts(input));

        var minDfa = dfa.Minimize();
        Assert.True(minDfa.Accepts(input));
    }

    [InlineData("")]
    [InlineData("a")]
    [InlineData("b")]
    [InlineData("aa")]
    [InlineData("bb")]
    [InlineData("abaa")]
    [InlineData("babb")]
    [Theory]
    public void LastTwoDifferentCharsRejects(string input)
    {
        var dfa = BuildLastTwoDifferentCharsDfa();
        Assert.False(dfa.Accepts(input));

        var minDfa = dfa.Minimize();
        Assert.False(minDfa.Accepts(input));
    }

    [Fact]
    public void Last2DifferentMinimize()
    {
        var dfa = BuildLastTwoDifferentCharsDfa().Minimize();

        Assert.True(dfa.States.ToHashSet().SetEquals(new[]
        {
            SetOf("S"),
            SetOf("A", "AA"),
            SetOf("B", "BB"),
            SetOf("AB"),
            SetOf("BA"),
        }));

        Assert.True(dfa.AcceptingStates.ToHashSet().SetEquals(new[]
        {
            SetOf("AB"),
            SetOf("BA"),
        }));

        Assert.Equal(SetOf("S"), dfa.InitialState);

        Assert.Equal(10, dfa.Transitions.Count);
        Assert.Contains(Tran(SetOf("S"), 'a', SetOf("A", "AA")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("S"), 'b', SetOf("B", "BB")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "AA"), 'a', SetOf("A", "AA")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("B", "BB"), 'b', SetOf("B", "BB")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("A", "AA"), 'b', SetOf("AB")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("B", "BB"), 'a', SetOf("BA")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("AB"), 'a', SetOf("BA")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("AB"), 'b', SetOf("B", "BB")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("BA"), 'a', SetOf("A", "AA")), dfa.Transitions);
        Assert.Contains(Tran(SetOf("BA"), 'b', SetOf("AB")), dfa.Transitions);
    }

    [Fact]
    public void CompleteSimple()
    {
        // We build a DFA that accepts a*
        // And we complete it over { a, b }
        var dfa = new Dfa<char, char>();
        dfa.InitialState = 'q';
        dfa.AcceptingStates.Add('q');
        dfa.Transitions.Add('q', 'a', 'q');
        dfa.Alphabet.Add('b');

        Assert.True(dfa.Complete('t'));
        Assert.True(SetOf('q', 't').SetEquals(dfa.States));
        Assert.Equal(4, dfa.Transitions.Count);
        Assert.Contains(Tran('q', 'a', 'q'), dfa.Transitions);
        Assert.Contains(Tran('q', 'b', 't'), dfa.Transitions);
        Assert.Contains(Tran('t', 'a', 't'), dfa.Transitions);
        Assert.Contains(Tran('t', 'b', 't'), dfa.Transitions);
    }

    [Fact]
    public void MinimizeIncomplete()
    {
        // We build a simple, incomplete DFA
        var dfa = new Dfa<string, char>();
        dfa.InitialState = "A";
        dfa.AcceptingStates.Add("D");
        dfa.Transitions.Add("A", '0', "B");
        dfa.Transitions.Add("A", '1', "C");
        dfa.Transitions.Add("B", '0', "C");
        dfa.Transitions.Add("C", '0', "B");
        dfa.Transitions.Add("C", '1', "D");

        // Minimize it
        var minDfa = dfa.Minimize();

        Assert.True(SetOf(SetOf("A"), SetOf("B"), SetOf("C"), SetOf("D")).SetEquals(minDfa.States));
        Assert.True(SetOf(SetOf("D")).SetEquals(minDfa.AcceptingStates));

        Assert.Equal(5, minDfa.Transitions.Count);
        Assert.Contains(Tran(SetOf("A"), '0', SetOf("B")), minDfa.Transitions);
        Assert.Contains(Tran(SetOf("A"), '1', SetOf("C")), minDfa.Transitions);
        Assert.Contains(Tran(SetOf("B"), '0', SetOf("C")), minDfa.Transitions);
        Assert.Contains(Tran(SetOf("C"), '0', SetOf("B")), minDfa.Transitions);
        Assert.Contains(Tran(SetOf("C"), '1', SetOf("D")), minDfa.Transitions);
    }

    [Fact]
    public void MinimizeIncompleteIssue()
    {
        var dfa = new Dfa<string, char>();
        dfa.InitialState = "q0";
        dfa.AcceptingStates.Add("q3");
        dfa.AcceptingStates.Add("q4");
        dfa.Transitions.Add("q0", '0', "q1");
        dfa.Transitions.Add("q1", 'x', "q2");
        dfa.Transitions.Add("q2", '0', "q3");
        dfa.Transitions.Add("q3", '0', "q4");
        dfa.Transitions.Add("q4", '0', "q4");

        var minDfa = dfa.Minimize();

        Assert.True(SetOf(SetOf("q0"), SetOf("q1"), SetOf("q2"), SetOf("q3", "q4")).SetEquals(minDfa.States));
        Assert.True(SetOf(SetOf("q3", "q4")).SetEquals(minDfa.AcceptingStates));

        Assert.Equal(4, minDfa.Transitions.Count);
        Assert.Contains(Tran(SetOf("q0"), '0', SetOf("q1")), minDfa.Transitions);
        Assert.Contains(Tran(SetOf("q1"), 'x', SetOf("q2")), minDfa.Transitions);
        Assert.Contains(Tran(SetOf("q2"), '0', SetOf("q3", "q4")), minDfa.Transitions);
        Assert.Contains(Tran(SetOf("q3", "q4"), '0', SetOf("q3", "q4")), minDfa.Transitions);
    }

    private static Dfa<string, char> BuildLastTwoDifferentCharsDfa()
    {
        var dfa = new Dfa<string, char>();
        dfa.InitialState = "S";
        dfa.AcceptingStates.Add("AB");
        dfa.AcceptingStates.Add("BA");
        dfa.Transitions.Add("S", 'a', "A");
        dfa.Transitions.Add("S", 'b', "B");
        dfa.Transitions.Add("A", 'a', "AA");
        dfa.Transitions.Add("A", 'b', "AB");
        dfa.Transitions.Add("AA", 'a', "AA");
        dfa.Transitions.Add("AA", 'b', "AB");
        dfa.Transitions.Add("B", 'a', "BA");
        dfa.Transitions.Add("B", 'b', "BB");
        dfa.Transitions.Add("BB", 'a', "BA");
        dfa.Transitions.Add("BB", 'b', "BB");
        dfa.Transitions.Add("AB", 'a', "BA");
        dfa.Transitions.Add("AB", 'b', "BB");
        dfa.Transitions.Add("BA", 'a', "AA");
        dfa.Transitions.Add("BA", 'b', "AB");
        return dfa;
    }

    private static ByValueSet<T> SetOf<T>(params T[] vs) => new(vs, comparer: null);

    private static Transition<TState, TSymbol> Tran<TState, TSymbol>(TState from, TSymbol on, TState to) =>
        new(from, on, to);
}
