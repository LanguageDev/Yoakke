using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        public class Literal : BnfAst
        {
            public readonly string Value;

            public Literal(string value)
            {
                Value = value;
            }

            public override bool Equals(BnfAst other) => other is Literal lit
                && Value.Equals(lit.Value);
            public override int GetHashCode() => Value.GetHashCode();

            public override string GetParsedType(RuleSet ruleSet) => TypeNames.IToken;
        }
    }
}
