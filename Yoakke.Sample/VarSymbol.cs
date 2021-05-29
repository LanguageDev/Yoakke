using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Symbols;
using Yoakke.Text;

namespace Yoakke.Sample
{
    public class VarSymbol : ISymbol
    {
        public IReadOnlyScope Scope { get; }
        public string Name { get; }
        public Location? Definition { get; }

        public VarSymbol(IReadOnlyScope scope, string name, Location? definition = null)
        {
            Scope = scope;
            Name = name;
            Definition = definition;
        }
    }
}
