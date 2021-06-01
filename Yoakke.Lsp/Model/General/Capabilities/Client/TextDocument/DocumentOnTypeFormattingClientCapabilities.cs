using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.General.Capabilities.Client.TextDocument
{
    public class DocumentOnTypeFormattingClientCapabilities
    {
        /// <summary>
        /// Whether on type formatting supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
    }
}
