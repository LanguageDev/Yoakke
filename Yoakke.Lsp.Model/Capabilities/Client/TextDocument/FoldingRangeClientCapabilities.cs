using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument
{
    public class FoldingRangeClientCapabilities
    {
        /// <summary>
        /// Whether implementation supports dynamic registration for folding range
        /// providers. If this is set to `true` the client supports the new
        /// `FoldingRangeRegistrationOptions` return value for the corresponding
        /// server capability as well.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }

        /// <summary>
        /// The maximum number of folding ranges that the client prefers to receive
        /// per document. The value serves as a hint, servers are free to follow the
        /// limit.
        /// </summary>
        [JsonProperty("rangeLimit", NullValueHandling = NullValueHandling.Ignore)]
        public uint? RangeLimit { get; set; }

        /// <summary>
        /// If set, the client signals that it only supports folding complete lines.
        /// If set, client will ignore specified `startCharacter` and `endCharacter`
        /// properties in a FoldingRange.
        /// </summary>
        [JsonProperty("lineFoldingOnly", NullValueHandling = NullValueHandling.Ignore)]
        public bool? LineFoldingOnly { get; set; }
    }
}
