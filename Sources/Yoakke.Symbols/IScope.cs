namespace Yoakke.Symbols
{
    /// <summary>
    /// A scope that can be modified by defining symbols inside them.
    /// </summary>
    public interface IScope : IReadOnlyScope
    {
        /// <summary>
        /// Defines a symbol in this scope.
        /// </summary>
        /// <param name="symbol">The symbol to define.</param>
        public void DefineSymbol(ISymbol symbol);
    }
}
