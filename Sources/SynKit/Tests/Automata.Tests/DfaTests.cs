using Xunit;
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
}
