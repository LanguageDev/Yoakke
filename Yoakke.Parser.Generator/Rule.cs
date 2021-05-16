using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Parser.Generator.Ast;

namespace Yoakke.Parser.Generator
{
    internal class Rule
    {
        /// <summary>
        /// The name of the rule. This is on the left-hand side of the rule.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// The AST of the grammar to match.
        /// </summary>
        public BnfAst Ast { get; set; }
        /// <summary>
        /// True, if this rule should be part of the public API
        /// </summary>
        public readonly bool PublicApi;
        /// <summary>
        /// The visual name of this rule.
        /// </summary>
        public string VisualName { get; set; }

        public Rule(string name, BnfAst ast, bool publicApi = true)
        {
            Name = name;
            Ast = ast;
            PublicApi = publicApi;
            VisualName = name;
        }
    }
}
