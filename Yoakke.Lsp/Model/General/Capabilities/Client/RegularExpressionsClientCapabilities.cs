using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.General.Capabilities.Client
{
    /// <summary>
    /// Client capabilities specific to regular expressions.
    /// </summary>
    public class RegularExpressionsClientCapabilities
    {
        /// <summary>
        /// The engine's name.
        /// </summary>
        [JsonProperty("engine")]
        public string Engine { get; set; }
        /// <summary>
        /// The engine's version.
        /// </summary>
        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public string? Version { get; set; }
    }
}
