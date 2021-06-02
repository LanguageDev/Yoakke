using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    public class RenameOptions : IWorkDoneProgressOptions
    {
        [JsonProperty("workDoneProgress", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WorkDoneProgress { get; set; }
        /// <summary>
        /// Renames should be checked and tested before being executed.
        /// </summary>
        [JsonProperty("prepareProvider", NullValueHandling = NullValueHandling.Ignore)]
        public bool? PrepareProvider { get; set; }
    }
}
