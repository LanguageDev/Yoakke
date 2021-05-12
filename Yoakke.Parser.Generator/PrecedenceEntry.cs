using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator
{
    internal class PrecedenceEntry
    {
        public readonly bool Left;
        public readonly ISet<object> Operators;
        public readonly IMethodSymbol Method;

        public PrecedenceEntry(bool left, ISet<object> operators, IMethodSymbol method)
        {
            Left = left;
            Operators = operators;
            Method = method;
        }
    }
}
