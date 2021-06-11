using System.Collections.Generic;
using Yoakke.Text;

namespace Yoakke.Reporting
{
    /// <summary>
    /// A single diagnostic information with all relevant things.
    /// </summary>
    public class Diagnostic
    {
        /// <summary>
        /// The severity of the diagnostic.
        /// </summary>
        public Severity? Severity { get; set; }
        /// <summary>
        /// A short identifier code of the diagnostic.
        /// </summary>
        public string? Code { get; set; }
        /// <summary>
        /// The summary message of the diagnostic.
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// The relevant information for the diagnostic.
        /// </summary>
        public IList<IDiagnosticInfo> Information { get; set; } = new List<IDiagnosticInfo>();

        public Diagnostic WithSeverity(Severity severity)
        {
            Severity = severity;
            return this;
        }

        public Diagnostic WithCode(string code)
        {
            Code = code;
            return this;
        }

        public Diagnostic WithMessage(string message)
        {
            Message = message;
            return this;
        }

        public Diagnostic WithInfo(IDiagnosticInfo info)
        {
            Information.Add(info);
            return this;
        }

        public Diagnostic WithSourceInfo(Location location, Severity severity, string message) =>
            WithInfo(new SourceDiagnosticInfo { Location = location, Severity = severity, Message = message });

        public Diagnostic WithSourceInfo(Location location, Severity severity) =>
            WithInfo(new SourceDiagnosticInfo { Location = location, Severity = severity });

        public Diagnostic WithSourceInfo(Location location, string message) =>
            WithInfo(new SourceDiagnosticInfo { Location = location, Message = message });

        public Diagnostic WithSourceInfo(Location location) =>
            WithInfo(new SourceDiagnosticInfo { Location = location });

        public Diagnostic WithFootnoteInfo(Severity severity, string footnote) =>
            WithInfo(new FootnoteDiagnosticInfo { Severity = severity, Message = footnote });

        public Diagnostic WithFootnoteInfo(string footnote) =>
            WithInfo(new FootnoteDiagnosticInfo { Message = footnote });
    }
}
