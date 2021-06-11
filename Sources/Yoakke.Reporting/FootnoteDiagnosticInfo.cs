namespace Yoakke.Reporting
{
    /// <summary>
    /// A footnote diagnostic information.
    /// </summary>
    public class FootnoteDiagnosticInfo : IDiagnosticInfo
    {
        public Severity? Severity { get; set; }

        public string? Message { get; set; }
    }
}
