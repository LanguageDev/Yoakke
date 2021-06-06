namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        /// <summary>
        /// Represents an optional sub-rule application.
        /// </summary>
        public class Opt : BnfAst
        {
            /// <summary>
            /// The optional sub-rule.
            /// </summary>
            public readonly BnfAst Subexpr;

            public Opt(BnfAst subexpr)
            {
                Subexpr = subexpr;
            }

            public override bool Equals(BnfAst other) => other is Opt opt
                && Subexpr.Equals(opt.Subexpr);
            public override int GetHashCode() => Subexpr.GetHashCode();

            public override BnfAst Desugar() => new Opt(Subexpr.Desugar());

            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) => 
                $"{Subexpr.GetParsedType(ruleSet, tokens)}?";
        }
    }
}
