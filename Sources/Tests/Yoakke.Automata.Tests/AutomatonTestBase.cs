// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Yoakke.Automata.Tests
{
    public abstract class AutomatonTestBase
    {
        protected static void AssertTransition<T>(IReadOnlyDfa<T, char> dfa, T from, char on, T to)
        {
            Assert.True(dfa.TryGetTransition(from, on, out var gotTo));
            Assert.Equal(to, gotTo);
        }

        protected static void AssertTransition(IReadOnlyDfa<StateSet<string>, char> dfa, string from, char on, string to)
        {
            var fromSet = ParseStateSet(from);
            var toSet = ParseStateSet(to);

            Assert.True(dfa.TryGetTransition(fromSet, on, out var gotTo));
            AssertEqual(toSet, gotTo);
        }

        protected static void AssertEqual(StateSet<string> expected, StateSet<string> got)
        {
            Assert.Equal(expected.Count, got.Count);
            Assert.All(expected, e => got.Contains(e));
            Assert.All(got, s => expected.Contains(s));
        }

        protected static StateSet<string> ParseStateSet(string text) =>
            new(text.Split(',').Select(t => t.Trim()).ToList());

        protected static (char, string) ParseTransition(string text)
        {
            var parts = text.Split("->");
            return (parts[0].Trim()[0], parts[1].Trim());
        }
    }
}
