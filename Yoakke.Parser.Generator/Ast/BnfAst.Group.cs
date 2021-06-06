namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        /// <summary>
        /// Represents a grouped sub-rule application that won't be unwrapped when transformed.
        /// </summary>
        public class Group : BnfAst
        {
            /// <summary>
            /// The sub-rule that won't be unwrapped when transformed.
            /// </summary>
            public readonly BnfAst Subexpr;

            public Group(BnfAst subexpr)
            {
                Subexpr = subexpr;
            }

            public override bool Equals(BnfAst other) => other is Group group
               && Subexpr.Equals(group.Subexpr);
            public override int GetHashCode() => Subexpr.GetHashCode();

            public override BnfAst Desugar() => Subexpr is Seq
                ? new Group(Subexpr.Desugar())
                : Subexpr.Desugar();

            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens) =>
                Subexpr.GetParsedType(ruleSet, tokens);
        }
    }
}
