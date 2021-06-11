using System.Collections.Generic;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    public class ExecuteCommandOptions : IWorkDoneProgressOptions
    {
        [JsonProperty("workDoneProgress", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WorkDoneProgress { get; set; }

        /// <summary>
        /// The commands to be executed on the server
        /// </summary>
        [JsonProperty("commands")]
        public IReadOnlyList<string> Commands { get; set; }
    }
}
