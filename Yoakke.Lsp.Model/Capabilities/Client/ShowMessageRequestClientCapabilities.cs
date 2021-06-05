using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client
{
    /// <summary>
    /// Show message request client capabilities
    /// </summary>
    public class ShowMessageRequestClientCapabilities
    {
        public class MessageActionItemCapabilities
        {
            /// <summary>
            /// Whether the client supports additional attributes which
            /// are preserved and sent back to the server in the
            /// request's response.
            /// </summary>
            [JsonProperty("additionalPropertiesSupport", NullValueHandling = NullValueHandling.Ignore)]
            public bool? AdditionalPropertiesSupport { get; set; }
        }

        /// <summary>
        /// Capabilities specific to the `MessageActionItem` type.
        /// </summary>
        [JsonProperty("messageActionItem", NullValueHandling = NullValueHandling.Ignore)]
        public MessageActionItemCapabilities? MessageActionItem { get; set; }
    }
}
