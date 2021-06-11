using Yoakke.Text;

namespace Yoakke.Symbols
{
    /// <summary>
    /// Represents a single symbol.
    /// </summary>
    public interface ISymbol
    {
        /// <summary>
        /// The scope that contains this symbol.
        /// </summary>
        public IReadOnlyScope Scope { get; }

        /// <summary>
        /// The name of this symbol.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The definition location of the symbol, if any.
        /// </summary>
        public Location? Definition { get; }
    }
}
