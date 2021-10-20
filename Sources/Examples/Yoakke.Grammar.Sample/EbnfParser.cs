// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections.Values;
using Yoakke.Grammar.Cfg;
using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;

namespace Yoakke.Grammar.Sample
{
    public enum EbnfTokenType
    {
        [Error] Error,
        [End] End,

        [Ignore]
        [Regex(Regexes.Whitespace)] Ignore,

        [Token("::=")] Arrow,
        [Token("|")] Or,
        [Regex(@"'[^']*'")] Literal,

        [Token("[")] OptionOpen,
        [Token("]")] OptionClose,

        [Token("{")] AnyOpen,
        [Token("}")] AnyClose,

        [Regex(Regexes.Identifier)] Identifier,
    }

    [Lexer(typeof(EbnfTokenType))]
    public partial class EbnfLexer
    {
    }

    public static class EbnfParser
    {
        public static ContextFreeGrammar ParseGrammar(string grammar)
        {
            var tokens = Tokenize(grammar);
            var ruleTokens = SplitRules(tokens)
                .Select(SplitRuleAlternatives)
                .ToDictionary(kv => kv.Name, kv => kv.Alternatives);
            return BuildGrammar(ruleTokens);
        }

        private static List<IToken<EbnfTokenType>> Tokenize(string grammar)
        {
            var lexer = new EbnfLexer(grammar);
            var tokens = new List<IToken<EbnfTokenType>>();
            while (true)
            {
                var token = lexer.Next();
                tokens.Add(token);
                if (token.Kind == EbnfTokenType.End) break;
            }
            return tokens;
        }

        private static IEnumerable<List<IToken<EbnfTokenType>>> SplitRules(List<IToken<EbnfTokenType>> tokens)
        {
            var last = 0;
            for (var i = 0; i < tokens.Count; ++i)
            {
                if (tokens[i].Kind == EbnfTokenType.Arrow)
                {
                    var curr = i - 1;
                    var prevRange = tokens.GetRange(last, curr - last);
                    if (prevRange.Count != 0) yield return prevRange;
                    last = curr;
                }
            }
            var prevRange1 = tokens.GetRange(last, tokens.Count - last - 1);
            if (prevRange1.Count != 0) yield return prevRange1;
        }

        private static (string Name, List<List<IToken<EbnfTokenType>>> Alternatives) SplitRuleAlternatives(List<IToken<EbnfTokenType>> tokens)
        {
            var name = tokens[0];
            var remTokens = tokens.Skip(2).ToList();
            var alts = new List<List<IToken<EbnfTokenType>>>();

            var last = 0;
            for (var i = 0; i < remTokens.Count; ++i)
            {
                if (remTokens[i].Kind == EbnfTokenType.Or)
                {
                    alts.Add(remTokens.GetRange(last, i - last));
                    last = i + 1;
                }
            }
            alts.Add(remTokens.GetRange(last, remTokens.Count - last));

            return (name.Text, alts);
        }

        private static List<IToken<EbnfTokenType>> ParseElement(ref int i, List<IToken<EbnfTokenType>> tokens)
        {
            var result = new List<IToken<EbnfTokenType>>() { tokens[i] };
            if (tokens[i].Kind is EbnfTokenType.AnyOpen or EbnfTokenType.OptionOpen)
            {
                ++i;
                for (; tokens[i].Kind != EbnfTokenType.AnyClose && tokens[i].Kind != EbnfTokenType.OptionClose; result.Add(tokens[i++]))
                {
                    // Pass
                }
                result.Add(tokens[i++]);
                return result;
            }
            else
            {
                ++i;
                return result;
            }
        }

        private static ContextFreeGrammar BuildGrammar(Dictionary<string, List<List<IToken<EbnfTokenType>>>> rules)
        {
            var result = new ContextFreeGrammar();
            foreach (var (ruleName, alts) in rules)
            {
                foreach (var alt in alts) BuildRuleAlt(result, rules, ruleName, alt);
            }
            return result;
        }

        private static void BuildRuleAlt(
            ContextFreeGrammar cfg,
            Dictionary<string, List<List<IToken<EbnfTokenType>>>> rules,
            string name,
            List<IToken<EbnfTokenType>> alt)
        {
            var prod = new List<Symbol>();
            var i = 0;
            while (i < alt.Count)
            {
                var oldI = i;
                var element = ParseElement(ref i, alt);
                if (element[0].Kind == EbnfTokenType.AnyOpen)
                {
                    throw new NotImplementedException();
                }
                else if (element[0].Kind == EbnfTokenType.OptionOpen)
                {
                    // We unwrap options
                    // Once include
                    BuildRuleAlt(cfg, rules, name, alt.Take(oldI).Concat(element.Skip(1).SkipLast(1)).Concat(alt.Skip(i)).ToList());
                    // Once skip
                    BuildRuleAlt(cfg, rules, name, alt.Take(oldI).Concat(alt.Skip(i)).ToList());
                    return;
                }
                else
                {
                    Debug.Assert(element.Count == 1);
                    var e = element[0];
                    if (e.Kind == EbnfTokenType.Literal)
                    {
                        // Literal match
                        prod.Add(new Terminal(e.Text[1..^1]));
                    }
                    else if (rules.ContainsKey(e.Text))
                    {
                        prod.Add(new Nonterminal(e.Text));
                    }
                    else
                    {
                        prod.Add(new Terminal(e.Text));
                    }
                }
            }
            cfg[new(name)].Add(prod.ToValue());
        }
    }
}
