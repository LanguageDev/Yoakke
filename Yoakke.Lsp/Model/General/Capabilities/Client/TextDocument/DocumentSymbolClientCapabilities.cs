using Newtonsoft.Json;
using System.Collections.Generic;

namespace Yoakke.Lsp.Model.General.Capabilities.Client.TextDocument
{
    public class DocumentSymbolClientCapabilities
    {
        public class SymbolKindCapabilities
        {
            /// <summary>
            /// The symbol kind values the client supports. When this
            /// property exists the client also guarantees that it will
            /// handle values outside its set gracefully and falls back
            /// to a default value when unknown.
            ///
            /// If this property is not present the client only supports
            /// the symbol kinds from `File` to `Array` as defined in
            /// the initial version of the protocol.
            /// </summary>
            [JsonProperty("valueSet", NullValueHandling = NullValueHandling.Ignore)]
            public IReadOnlyList<SymbolKind>? ValueSet { get; set; }
        }

        public class TagSupportCapabilities
        {
            /// <summary>
            /// The tags supported by the client.
            /// </summary>
            [JsonProperty("valueSet")]
            public IReadOnlyList<SymbolTag> ValueSet { get; set; }
        }

        /// <summary>
        /// Whether document symbol supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }
        /// <summary>
        /// Specific capabilities for the `SymbolKind` in the
        /// `textDocument/documentSymbol` request.
        /// </summary>
        [JsonProperty("symbolKind", NullValueHandling = NullValueHandling.Ignore)]
        public SymbolKindCapabilities? SymbolKind { get; set; }
        /// <summary>
        /// The client supports hierarchical document symbols.
        /// </summary>
        [JsonProperty("hierarchicalDocumentSymbolSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HierarchicalDocumentSymbolSupport { get; set; }
        /// <summary>
        /// The client supports tags on `SymbolInformation`. Tags are supported on
        /// `DocumentSymbol` if `hierarchicalDocumentSymbolSupport` is set to true.
        /// Clients supporting tags have to handle unknown tags gracefully.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("tagSupport", NullValueHandling = NullValueHandling.Ignore)]
        public TagSupportCapabilities? TagSupport { get; set; }
        /// <summary>
        /// The client supports an additional label presented in the UI when
        /// registering a document symbol provider.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("labelSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? LabelSupport { get; set; }
    }
}
