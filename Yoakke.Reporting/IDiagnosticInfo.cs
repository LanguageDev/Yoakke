namespace Yoakke.Reporting
{
    /// <summary>
    /// A single piece of information for a <see cref="Diagnostic"/>.
    /// </summary>
    public interface IDiagnosticInfo
    {
        /// <summary>
        /// The severity of the information.
        /// </summary>
        public Severity? Severity { get; }
        /// <summary>
        /// The information message.
        /// </summary>
        public string? Message { get; }
    }
}
