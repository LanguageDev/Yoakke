using Newtonsoft.Json;
using System.Collections.Generic;
using Yoakke.Lsp.Model.Diagnostics;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument
{
    public class PublishDiagnosticsClientCapabilities
    {
        public class TagSupportCapabilities
        {
            /// <summary>
            /// The tags supported by the client.
            /// </summary>
            [JsonProperty("valueSet")]
            public IReadOnlyList<DiagnosticTag> ValueSet { get; set; }
        }

        /// <summary>
        /// Whether the clients accepts diagnostics with related information.
        /// </summary>
        [JsonProperty("relatedInformation", NullValueHandling = NullValueHandling.Ignore)]
        public bool? RelatedInformation { get; set; }
        /// <summary>
        /// Client supports the tag property to provide meta data about a diagnostic.
        /// Clients supporting tags have to handle unknown tags gracefully.
        /// </summary>
        [Since(3, 15, 0)]
        [JsonProperty("tagSupport", NullValueHandling = NullValueHandling.Ignore)]
        public TagSupportCapabilities? TagSupport { get; set; }
        /// <summary>
        /// Whether the client interprets the version property of the
        /// `textDocument/publishDiagnostics` notification's parameter.
        /// </summary>
        [Since(3, 15, 0)]
        [JsonProperty("versionSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? VersionSupport { get; set; }
        /// <summary>
        /// Client supports a codeDescription property
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("codeDescriptionSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? CodeDescriptionSupport { get; set; }
        /// <summary>
        /// Whether code action supports the `data` property which is
        /// preserved between a `textDocument/publishDiagnostics` and
        /// `textDocument/codeAction` request.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("dataSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DataSupport { get; set; }
    }
}
