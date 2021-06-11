namespace Yoakke.Reporting.Present
{
    /// <summary>
    /// Interface for objects that can present a <see cref="Diagnostic"/> for the user in some way.
    /// </summary>
    public interface IDiagnosticPresenter
    {
        /// <summary>
        /// The style to use when presenting.
        /// </summary>
        public DiagnosticStyle Style { get; set; }

        /// <summary>
        /// The syntax highlighter to use for source code.
        /// </summary>
        public ISyntaxHighlighter SyntaxHighlighter { get; set; }

        /// <summary>
        /// Presents the given <see cref="Diagnostic"/> to the user.
        /// </summary>
        /// <param name="diagnostic">The diagnostic to present.</param>
        public void Present(Diagnostic diagnostic);
    }
}
