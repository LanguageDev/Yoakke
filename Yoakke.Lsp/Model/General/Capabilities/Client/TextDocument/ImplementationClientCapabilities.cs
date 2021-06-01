using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.General.Capabilities.Client.TextDocument
{
    public class ImplementationClientCapabilities
    {
        /// <summary>
        /// Whether implementation supports dynamic registration. If this is set to
        /// `true` the client supports the new `ImplementationRegistrationOptions`
        /// return value for the corresponding server capability as well.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
        /// <summary>
        /// The client supports additional metadata in the form of definition links.
        /// </summary>
        [Since(3, 14, 0)]
        [JsonProperty("linkSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? LinkSupport { get; set; }
    }
}
