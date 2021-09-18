using System.Linq;
using Xunit;
using Yoakke.Automata.Discrete;

namespace Yoakke.Automata.Tests
{
    public class DfaTests
    {
        [Theory]
        [InlineData(new string[] { }, false)]
        [InlineData(new string[] { "a -> A" }, false)]
        [InlineData(new string[] { "b -> B" }, false)]
        [InlineData(new string[] { "a -> A", "a -> AA" }, false)]
        [InlineData(new string[] { "a -> A", "b -> AB" }, true)]
        [InlineData(new string[] { "a -> A", "b -> AB", "a -> BA" }, true)]
        public void Last2DifferentAcceptsTests(string[] transitionTexts, bool accepts)
        {
            // We use an automata that accepts at least 2-long characters where the last 2 characters differ
            // The alphabet is { a, b }
            var dfa = new Dfa<string, char>();
            dfa.InitialState = "S";
            dfa.AcceptingStates.Add("AB");
            dfa.AcceptingStates.Add("BA");
            dfa.AddTransition("S", 'a', "A");
            dfa.AddTransition("S", 'b', "B");
            dfa.AddTransition("A", 'a', "AA");
            dfa.AddTransition("A", 'b', "AB");
            dfa.AddTransition("AA", 'a', "AA");
            dfa.AddTransition("AA", 'b', "AB");
            dfa.AddTransition("B", 'a', "BA");
            dfa.AddTransition("B", 'b', "BB");
            dfa.AddTransition("BB", 'a', "BA");
            dfa.AddTransition("BB", 'b', "BB");
            dfa.AddTransition("AB", 'a', "BA");
            dfa.AddTransition("AB", 'b', "BB");
            dfa.AddTransition("BA", 'a', "AA");
            dfa.AddTransition("BA", 'b', "AB");

            var transitions = transitionTexts.Select(ParseTransition).ToList();

            var state = dfa.InitialState;
            foreach (var (inputChar, expectedNextState) in transitions)
            {
                Assert.True(dfa.TryGetTransition(state!, inputChar, out var nextState));
                Assert.Equal(expectedNextState, nextState);
                state = nextState;
            }

            var input = transitions.Select(t => t.Item1);
            Assert.Equal(accepts, dfa.Accepts(input));
        }

        private static (char, string) ParseTransition(string text)
        {
            var parts = text.Split("->");
            return (parts[0].Trim()[0], parts[1].Trim());
        }
    }
}
