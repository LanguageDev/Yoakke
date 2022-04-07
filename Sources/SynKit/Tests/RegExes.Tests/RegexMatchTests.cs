// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Yoakke.SynKit.Automata;

namespace Yoakke.SynKit.RegExes.Tests;

public sealed class RegexMatchTests
{
    [InlineData(@"a", "a")]
    [InlineData(@"abc", "abc")]
    [InlineData(@"a*", "")]
    [InlineData(@"a*", "a")]
    [InlineData(@"a*", "aa")]
    [InlineData(@"a*", "aaaaaa")]
    [InlineData(@"a+", "a")]
    [InlineData(@"a+", "aaaa")]
    [InlineData(@"(ab)*", "")]
    [InlineData(@"(ab)*", "ab")]
    [InlineData(@"(ab)*", "abababab")]
    [InlineData(@"[[:xdigit:]]*", "123ABCDEFabcdef0123456789")]
    [InlineData(@"[[:^xdigit:]]*", "ghijkl")]
    [Theory]
    public void MatchesNfaDfaMindfa(string regex, string text)
    {
        var settings = new RegExSettings();
        var pcreAst = PcreParser.Parse(settings, regex);
        var rawRegexAst = pcreAst.ToPlainRegex(settings);
        var nfa = RegexToNfa(rawRegexAst);
        var epsilonlessNfa = RegexToNfa(rawRegexAst);
        epsilonlessNfa.EliminateEpsilonTransitions();
        var dfa = nfa.Determinize();
        var minDfa = dfa.Minimize();

        Assert.True(nfa.Accepts(text));
        Assert.True(epsilonlessNfa.Accepts(text));
        Assert.True(dfa.Accepts(text));
        Assert.True(minDfa.Accepts(text));
    }

    [InlineData(@"a", "b")]
    [InlineData(@"abc", "abd")]
    [InlineData(@"abc", "abcd")]
    [InlineData(@"a*", "b")]
    [InlineData(@"a*", "ab")]
    [InlineData(@"a*", "aab")]
    [InlineData(@"a*", "aaaabaa")]
    [InlineData(@"a+", "")]
    [InlineData(@"a+", "b")]
    [InlineData(@"(ab)*", "aaaa")]
    [InlineData(@"(ab)*", "aba")]
    [InlineData(@"(ab)*", "ababaabab")]
    [InlineData(@"[[:xdigit:]]*", "abcdghx9y9")]
    [InlineData(@"[[:^xdigit:]]*", "gh1ijklab")]
    [Theory]
    public void DoesNotMatchNfaDfaMindfa(string regex, string text)
    {
        var settings = new RegExSettings();
        var pcreAst = PcreParser.Parse(settings, regex);
        var rawRegexAst = pcreAst.ToPlainRegex(settings);
        var nfa = RegexToNfa(rawRegexAst);
        var epsilonlessNfa = RegexToNfa(rawRegexAst);
        epsilonlessNfa.EliminateEpsilonTransitions();
        var dfa = nfa.Determinize();
        var minDfa = dfa.Minimize();

        Assert.False(nfa.Accepts(text));
        Assert.False(epsilonlessNfa.Accepts(text));
        Assert.False(dfa.Accepts(text));
        Assert.False(minDfa.Accepts(text));
    }

    private static Nfa<int, char> RegexToNfa(RegExAst<char> regexAst)
    {
        var nfa = new Nfa<int, char>();
        var randomState = StateCreator.Random();
        // TODO: Ease this with utilities maybe? Have a method that auto-adds start and end?
        var (start, end) = regexAst.ThompsonsConstruct(nfa, randomState);
        nfa.InitialStates.Add(start);
        nfa.AcceptingStates.Add(end);
        return nfa;
    }
}
