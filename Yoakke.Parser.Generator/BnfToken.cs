using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator
{
    internal class BnfToken
    {
        public readonly int Index;
        public readonly string Value;
        public readonly BnfTokenType Type;

        public BnfToken(int index, string value, BnfTokenType type)
        {
            Index = index;
            Value = value;
            Type = type;
        }
    }
}
