using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
