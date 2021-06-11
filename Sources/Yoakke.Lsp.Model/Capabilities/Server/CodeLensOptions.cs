using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    public class CodeLensOptions : IWorkDoneProgressOptions
    {
        [JsonProperty("workDoneProgress", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WorkDoneProgress { get; set; }

        /// <summary>
        /// Code lens has a resolve provider as well.
        /// </summary>
        [JsonProperty("resolveProvider", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ResolveProvider { get; set; }
    }
}
