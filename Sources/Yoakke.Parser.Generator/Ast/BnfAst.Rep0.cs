namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        /// <summary>
        /// Represents a sub-rule that can be repeated 0 or more times.
        /// </summary>
        public class Rep0 : BnfAst
        {
            /// <summary>
            /// The sub-rule to repeat.
            /// </summary>
            public readonly BnfAst Subexpr;

            public Rep0(BnfAst subexpr)
            {
                this.Subexpr = subexpr;
            }

            public override bool Equals(BnfAst other) => other is Rep0 rep
                && this.Subexpr.Equals(rep.Subexpr);

            public override int GetHashCode() => this.Subexpr.GetHashCode();

            public override BnfAst Desugar() => new Rep0(this.Subexpr.Desugar());

            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
                $"{TypeNames.IReadOnlyList}<{this.Subexpr.GetParsedType(ruleSet, tokens)}>";
        }
    }
}
