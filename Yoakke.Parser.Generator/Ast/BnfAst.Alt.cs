using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Compatibility;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
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

            public override bool Equals(BnfAst other) => other is Alt alt 
                && First.Equals(alt.First) 
                && Second.Equals(alt.Second);
            public override int GetHashCode() => HashCode.Combine(First, Second);
        }
    }
}
