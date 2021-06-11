using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument
{
    public class DocumentLinkClientCapabilities
    {
        /// <summary>
        /// Whether document link supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }

        /// <summary>
        /// Whether the client supports the `tooltip` property on `DocumentLink`.
        /// </summary>
        [Since(3, 15, 0)]
        [JsonProperty("tooltipSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? TooltipSupport { get; set; }
    }
}
