using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lsp.Model.General.Capabilities.Client.TextDocument
{
    public class SemanticTokensClientCapabilities
    {
        public class RequestsCapabilities
        {
            public class RangeCapabilities
            {
            }

            public class FullCapabilities
            {
                /// <summary>
                /// The client will send the `textDocument/semanticTokens/full/delta`
                /// request if the server provides a corresponding handler.
                /// </summary>
                [JsonProperty("delta", NullValueHandling = NullValueHandling.Ignore)]
                public bool? Delta { get; set; }
            }

            /// <summary>
            /// The client will send the `textDocument/semanticTokens/range` request
            /// if the server provides a corresponding handler.
            /// </summary>
            [JsonProperty("range", NullValueHandling = NullValueHandling.Ignore)]
            public BoolOrObject<RangeCapabilities>? Range { get; set; }
            /// <summary>
            /// The client will send the `textDocument/semanticTokens/full` request
            /// if the server provides a corresponding handler.
            /// </summary>
            [JsonProperty("full", NullValueHandling = NullValueHandling.Ignore)]
            public BoolOrObject<FullCapabilities>? Full { get; set; }
        }

        /// <summary>
        /// Whether implementation supports dynamic registration. If this is set to
        /// `true` the client supports the new `(TextDocumentRegistrationOptions &
        /// StaticRegistrationOptions)` return value for the corresponding server
        /// capability as well.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
        /// <summary>
        /// Which requests the client supports and might send to the server
        /// depending on the server's capability. Please note that clients might not
        /// show semantic tokens or degrade some of the user experience if a range
        /// or full request is advertised by the client but not provided by the
        /// server. If for example the client capability `requests.full` and
        /// `request.range` are both set to true but the server only provides a
        /// range provider the client might not render a minimap correctly or might
        /// even decide to not show any semantic tokens at all.
        /// </summary>
        [JsonProperty("requests")]
        public RequestsCapabilities Requests { get; set; }
        /// <summary>
        /// The token types that the client supports.
        /// </summary>
        [JsonProperty("tokenTypes")]
        public IReadOnlyList<string> TokenTypes { get; set; }
        /// <summary>
        /// The token modifiers that the client supports.
        /// </summary>
        [JsonProperty("tokenModifiers")]
        public IReadOnlyList<string> TokenModifiers { get; set; }
        /// <summary>
        /// The formats the clients supports.
        /// </summary>
        [JsonProperty("formats")]
        public IReadOnlyList<TokenFormat> Formats { get; set; }
        /// <summary>
        /// Whether the client supports tokens that can overlap each other.
        /// </summary>
        [JsonProperty("overlappingTokenSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? OverlappingTokenSupport { get; set; }
        /// <summary>
        /// Whether the client supports tokens that can span multiple lines.
        /// </summary>
        [JsonProperty("multilineTokenSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? MultilineTokenSupport { get; set; }
    }
}
