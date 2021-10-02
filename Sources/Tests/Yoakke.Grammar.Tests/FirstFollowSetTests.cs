using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Yoakke.Grammar.Cfg;

namespace Yoakke.Grammar.Tests
{
    public class FirstFollowSetTests
    {
        [Theory]
        [InlineData(@"
            E -> T E'
            E' -> + T E' | ε
            T -> F T'
            T' -> * F T' | ε
            F -> ( E ) | id",
        new[] {
            "E : (, id",
            "T : (, id",
            "F : (, id",
            "E' : +, ε",
            "T' : *, ε",
        })]
        public void FirstSetTests(string grammarText, string[] firstSets)
        {
            var cfg = TestUtils.ParseGrammar(grammarText);
            cfg.StartSymbol = "E";

            foreach (var term in cfg.Terminals)
            {
                var firstSet = cfg.First(term);

                Assert.False(firstSet.HasEmpty);
                Assert.True(firstSet.Terminals.SequenceEqual(new[] { term }));
            }

            foreach (var firstSetText in firstSets)
            {
                var (rule, expectedTerms, expectedEps) = ParseSet(firstSetText);
                var firstSet = cfg.First(new Symbol.Nonterminal(rule));

                Assert.True(expectedTerms.SetEquals(firstSet.Terminals));
                Assert.Equal(expectedEps, firstSet.HasEmpty);
            }
        }

        [Theory]
        [InlineData(@"
            E -> T E'
            E' -> + T E' | ε
            T -> F T'
            T' -> * F T' | ε
            F -> ( E ) | id",
        new[] {
            "E : ), $",
            "E' : ), $",
            "T : +, ), $",
            "T' : +, ), $",
            "F : +, *, ), $",
        })]
        public void FollowSetTests(string grammarText, string[] followSets)
        {
            var cfg = TestUtils.ParseGrammar(grammarText);
            cfg.StartSymbol = "E";

            foreach (var followSetText in followSets)
            {
                var (rule, expectedTerms, _) = ParseSet(followSetText);
                var followSet = cfg.Follow(new Symbol.Nonterminal(rule));

                Assert.True(expectedTerms.SetEquals(followSet.Terminals));
            }
        }

        private static (string Rule, HashSet<Symbol.Terminal> Terminals, bool Epsilon) ParseSet(string text)
        {
            var parts = text.Split(':');
            var terminals = parts[1].Trim().Split(',').Select(t => t.Trim());
            var hasEpsilon = terminals.Any(t => t == "ε");
            var termSymbols = terminals
                .Where(t => t != "ε")
                .Select(t => t == "$" ? Symbol.EndOfInput : new Symbol.Terminal(t))
                .ToHashSet();
            return (parts[0].Trim(), termSymbols, hasEpsilon);
        }
    }
}
