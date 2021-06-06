namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        /// <summary>
        /// Represents a sub-rule that can be repeated 1 or more times.
        /// </summary>
        public class Rep1 : BnfAst
        {
            /// <summary>
            /// The sub-rule to repeat.
            /// </summary>
            public readonly BnfAst Subexpr;

            public Rep1(BnfAst subexpr)
            {
                Subexpr = subexpr;
            }

            public override bool Equals(BnfAst other) => other is Rep1 rep
                && Subexpr.Equals(rep.Subexpr);
            public override int GetHashCode() => Subexpr.GetHashCode();

            public override BnfAst Desugar() => new Rep1(Subexpr.Desugar());

            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
                $"{TypeNames.IReadOnlyList}<{Subexpr.GetParsedType(ruleSet, tokens)}>";
        }
    }
}
