using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    public class WorkDoneProgressParams
    {
        /// <summary>
        /// An optional token that a server can use to report work done progress.
        /// </summary>
        [JsonProperty("workDoneToken", NullValueHandling = NullValueHandling.Ignore)]
        public ProgressToken? WorkDoneToken { get; set; }
    }
}
