using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.General.Capabilities.Client
{
    public class DidChangeConfigurationClientCapabilities
    {
        /// <summary>
        /// Did change configuration notification supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
    }
}
