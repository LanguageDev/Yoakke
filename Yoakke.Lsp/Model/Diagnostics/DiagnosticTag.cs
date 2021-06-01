namespace Yoakke.Lsp.Model.Diagnostics
{
    /// <summary>
    /// The diagnostic tags.
    /// </summary>
    [Since(3, 15, 0)]
    public enum DiagnosticTag
    {
        /// <summary>
        /// Unused or unnecessary code.
        ///
        /// Clients are allowed to render diagnostics with this tag faded out
        /// instead of having an error squiggle.
        /// </summary>
        Unnecessary = 1,
        /// <summary>
        /// Deprecated or obsolete code.
        ///
        /// Clients are allowed to rendered diagnostics with this tag strike through.
        /// </summary>
        Deprecated = 2,
    }
}
