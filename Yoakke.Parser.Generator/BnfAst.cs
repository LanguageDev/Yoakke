using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator
{
    internal abstract class BnfAst
    {
        public class Alt : BnfAst
        {
            public readonly BnfAst First;
            public readonly BnfAst Second;

            public Alt(BnfAst first, BnfAst second)
            {
                First = first;
                Second = second;
            }
        }

        public class Seq : BnfAst
        {
            public readonly BnfAst First;
            public readonly BnfAst Second;

            public Seq(BnfAst first, BnfAst second)
            {
                First = first;
                Second = second;
            }
        }

        public class Opt : BnfAst
        {
            public readonly BnfAst Subexpr;

            public Opt(BnfAst subexpr)
            {
                Subexpr = subexpr;
            }
        }

        public class Rep0 : BnfAst
        {
            public readonly BnfAst Subexpr;

            public Rep0(BnfAst subexpr)
            {
                Subexpr = subexpr;
            }
        }

        public class Rep1 : BnfAst
        {
            public readonly BnfAst Subexpr;

            public Rep1(BnfAst subexpr)
            {
                Subexpr = subexpr;
            }
        }

        public class RuleCall : BnfAst
        {
            public readonly string Name;

            public RuleCall(string name)
            {
                Name = name;
            }
        }

        public class LiteralValue : BnfAst
        {
            public readonly string Value;

            public LiteralValue(string value)
            {
                Value = value;
            }
        }

        public class LiteralKind : BnfAst
        {
            public readonly string Kind;

            public LiteralKind(string kind)
            {
                Kind = kind;
            }
        }
    }
}
