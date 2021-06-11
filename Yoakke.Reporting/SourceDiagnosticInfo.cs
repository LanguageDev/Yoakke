using Yoakke.Text;

namespace Yoakke.Reporting
{
    /// <summary>
    /// A <see cref="IDiagnosticInfo"/> that references part of the source code.
    /// </summary>
    public class SourceDiagnosticInfo : IDiagnosticInfo
    {
        public Severity? Severity { get; set; }

        public string? Message { get; set; }

        /// <summary>
        /// The location the diagnostic information refers to.
        /// </summary>
        public Location Location { get; set; }
    }
}
