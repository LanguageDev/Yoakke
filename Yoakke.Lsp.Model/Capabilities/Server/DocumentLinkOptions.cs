using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    public class DocumentLinkOptions : IWorkDoneProgressOptions
    {
        [JsonProperty("workDoneProgress", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WorkDoneProgress { get; set; }
        /// <summary>
        /// Document links have a resolve provider as well.
        /// </summary>
        [JsonProperty("resolveProvider", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ResolveProvider { get; set; }
    }
}
