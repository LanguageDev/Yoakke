using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument
{
    public class DocumentColorClientCapabilities
    {
        /// <summary>
        /// Whether document color supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
    }
}
