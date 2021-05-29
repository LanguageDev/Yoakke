using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Symbols;
using Yoakke.Text;

namespace Yoakke.Sample
{
    public class ConstSymbol : ISymbol
    {
        public IReadOnlyScope Scope { get; }
        public string Name { get; }
        public Location? Definition { get; }

        public readonly object? Value;

        public ConstSymbol(IReadOnlyScope scope, string name, object? value, Location? definition = null)
        {
            Scope = scope;
            Name = name;
            Value = value;
            Definition = definition;
        }
    }
}
