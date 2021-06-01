namespace Yoakke.Lsp.Model.LanguageFeatures
{
    /// <summary>
    /// Symbol tags are extra annotations that tweak the rendering of a symbol.
    /// </summary>
    [Since(3, 16)]
    public enum SymbolTag
    {
        /// <summary>
        /// Render a symbol as obsolete, usually using a strike-out.
        /// </summary>
        Deprecated = 1,
    }
}
