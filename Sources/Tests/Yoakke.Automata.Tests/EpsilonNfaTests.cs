// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Yoakke.Automata.Sparse;

namespace Yoakke.Automata.Tests
{
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

            var state = new StateSet<string>(new[] { nfa.InitialState });
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

        /*[Fact]
        public void Has101Or11NfaEpsilonElimination()
        {
            var nfa = BuildHas101Or11Nfa().EliminateEpsilonTransitions();

            var expectedStates = new[] { "A", "B, C", "C", "D" }.Select(ParseStateSet);
            var gotStates = nfa.States.ToHashSet();

            var expectedAcceptingStates = new[] { "D" }.Select(ParseStateSet);
            var gotAcceptingStates = nfa.AcceptingStates.ToHashSet();

            Assert.True(gotStates.SetEquals(expectedStates));
            Assert.True(gotAcceptingStates.SetEquals(expectedAcceptingStates));

            AssertTransitions(nfa, "A", '0', new[] { "A" });
            AssertTransitions(nfa, "A", '1', new[] { "A", "B" });
        }*/

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
            nfa.InitialState = "A";
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
    }
}
