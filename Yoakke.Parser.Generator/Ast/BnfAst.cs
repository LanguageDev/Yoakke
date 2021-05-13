using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator.Ast
{
    internal abstract partial class BnfAst : IEquatable<BnfAst>
    {
        public override bool Equals(object obj) => obj is BnfAst bnf && Equals(bnf);

        public abstract bool Equals(BnfAst other);
        public abstract override int GetHashCode();

        public abstract BnfAst Desugar();
        public abstract string GetParsedType(RuleSet ruleSet, TokenKindSet tokens);
    }
}
