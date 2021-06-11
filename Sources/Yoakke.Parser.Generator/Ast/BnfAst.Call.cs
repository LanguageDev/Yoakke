// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        /// <summary>
        /// Represents parsing a sub-rule that is referenced by name.
        /// </summary>
        public class Call : BnfAst
        {
            /// <summary>
            /// The name of the sub-rule.
            /// </summary>
            public readonly string Name;

            public Call(string name)
            {
                this.Name = name;
            }

            public override bool Equals(BnfAst other) => other is Call call
                && this.Name.Equals(call.Name);

            public override int GetHashCode() => this.Name.GetHashCode();

            public override BnfAst Desugar() => this;

            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens)
            {
                var called = ruleSet.GetRule(this.Name);
                return called.Ast.GetParsedType(ruleSet, tokens);
            }
        }
    }
}
