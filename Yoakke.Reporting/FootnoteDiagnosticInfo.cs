using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
