using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument
{
    public class CodeLensClientCapabilities
    {
        /// <summary>
        /// Whether code lens supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
    }
}
