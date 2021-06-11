using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument
{
    public class DefinitionClientCapabilities
    {
        /// <summary>
        /// Whether definition supports dynamic registration.
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
