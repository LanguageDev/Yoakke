using System.Linq;
using Xunit;
using Yoakke.Collections;
using Yoakke.SynKit.Automata;

namespace Automata.Tests;

public class DfaTests
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
