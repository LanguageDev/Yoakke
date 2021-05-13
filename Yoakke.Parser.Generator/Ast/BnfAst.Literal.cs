using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        public class Literal : BnfAst
        {
            public readonly object Value;

            public Literal(object value)
            {
                Value = value;
            }

            public override bool Equals(BnfAst other) => other is Literal lit
                && Value.Equals(lit.Value);
            public override int GetHashCode() => Value.GetHashCode();

            public override BnfAst Desugar() => this;

            public override string GetParsedType(RuleSet ruleSet) => TypeNames.IToken;
        }
    }
}
