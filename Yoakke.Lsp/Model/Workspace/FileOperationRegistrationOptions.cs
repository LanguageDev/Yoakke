using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lsp.Model.Workspace
{
    /// <summary>
    /// The options to register for file operations.
    /// </summary>
    [Since(3, 16, 0)]
    public class FileOperationRegistrationOptions
    {
        /// <summary>
        /// The actual filters.
        /// </summary>
        [JsonProperty("filters")]
        public IReadOnlyList<FileOperationFilter> Filters { get; set; }
    }
}
