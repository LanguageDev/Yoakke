// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Yoakke.SynKit.Parser.Generator.Ast;

namespace Yoakke.SynKit.Parser.Generator.Model;

/// <summary>
/// Represents a set of BNF <see cref="Rule"/> definitions.
/// </summary>
internal class RuleSet
{
    /// <summary>
    /// The rule name (left of grammar) mapping to the different possible rules (basically alternatives).
    /// </summary>
    public IDictionary<string, Rule> Rules { get; private set; } = new Dictionary<string, Rule>();

    private readonly Dictionary<string, IList<PrecedenceEntry>> precedences = new();

    /// <summary>
    /// Tries to retrieve a <see cref="Rule"/> from this <see cref="RuleSet"/> with a given name.
    /// </summary>
    /// <param name="name">The name of the <see cref="Rule"/> to retrieve.</param>
    /// <param name="rule">The <see cref="Rule"/> gets written here, if found.</param>
    /// <returns>True, if a <see cref="Rule"/> with the name <paramref name="name"/> was found.</returns>
    public bool TryGetRule(string name, out Rule rule) => this.Rules.TryGetValue(name, out rule);

    /// <summary>
    /// Retrieves a <see cref="Rule"/> from this <see cref="RuleSet"/> with a given name.
    /// </summary>
    /// <param name="name">The name of the <see cref="Rule"/> to retrieve.</param>
    /// <returns>The <see cref="Rule"/> with the name <paramref name="name"/>.</returns>
    public Rule GetRule(string name) => this.Rules[name];

    /// <summary>
    /// Adds a <see cref="Rule"/> to this <see cref="RuleSet"/>.
    /// </summary>
    /// <param name="rule">The <see cref="Rule"/> to add.</param>
    public void Add(Rule rule)
    {
        if (this.Rules.TryGetValue(rule.Name, out var existingRule))
        {
            // Combine them with an alternation
            existingRule.Ast = new BnfAst.Alt(existingRule.Ast, rule.Ast);
        }
        else
        {
            this.Rules.Add(rule.Name, rule);
        }
    }

    /// <summary>
    /// Adds a precedence table to this <see cref="RuleSet"/>.
    /// </summary>
    /// <param name="ruleName">The name of the rule that will parse this precedence hierarchy.</param>
    /// <param name="precs">The list of precedence descriptions.</param>
    /// <param name="method">The method that transforms each parse into the primitive.</param>
    public void AddPrecedence(string ruleName, IList<(bool Left, HashSet<object> Operators)> precs, IMethodSymbol method)
    {
        if (!this.precedences.TryGetValue(ruleName, out var precList))
        {
            precList = new List<PrecedenceEntry>();
            this.precedences.Add(ruleName, precList);
        }
        foreach (var (l, p) in precs) precList.Add(new PrecedenceEntry(l, p, method));
    }

    /// <summary>
    /// Desugars everything into their simplest form.
    /// This means transforming precedence tables into <see cref="Rule"/>s and normalizing all <see cref="Rule"/>.
    /// </summary>
    public void Desugar()
    {
        // Generate precedence rules
        foreach (var kv in this.precedences)
        {
            if (!this.Rules.TryGetValue(kv.Key, out var rule)) continue;
            this.Rules.Remove(kv.Key);
            var newRules = BnfDesugar.GeneratePrecedenceParser(rule, kv.Value.Reverse().ToList());
            foreach (var r in newRules) this.Add(r);
        }

        // Desugar left-recursion, includes native desugaring steps
        BnfDesugar.EliminateLeftRecursion(this.Rules);
    }
}
