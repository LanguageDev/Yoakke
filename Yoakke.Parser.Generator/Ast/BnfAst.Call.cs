using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        public class Call : BnfAst
        {
            public readonly string Name;

            public Call(string name)
            {
                Name = name;
            }

            public override bool Equals(BnfAst other) => other is Call call
                && Name.Equals(call.Name);
            public override int GetHashCode() => Name.GetHashCode();
        }
    }
}
