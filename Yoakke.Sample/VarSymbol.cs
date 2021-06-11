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
            this.Scope = scope;
            this.Name = name;
            this.Definition = definition;
        }
    }
}
