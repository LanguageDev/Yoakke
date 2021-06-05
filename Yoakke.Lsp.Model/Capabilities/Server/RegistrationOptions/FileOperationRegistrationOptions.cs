using Newtonsoft.Json;
using System.Collections.Generic;
using Yoakke.Lsp.Model.Workspace;

namespace Yoakke.Lsp.Model.Capabilities.Server.RegistrationOptions
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
