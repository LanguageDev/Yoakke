using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.General.Capabilities.Client.TextDocument
{
    public class SelectionRangeClientCapabilities
    {
        /// <summary>
        /// Whether implementation supports dynamic registration for selection range
        /// providers. If this is set to `true` the client supports the new
        /// `SelectionRangeRegistrationOptions` return value for the corresponding
        /// server capability as well.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
    }
}
