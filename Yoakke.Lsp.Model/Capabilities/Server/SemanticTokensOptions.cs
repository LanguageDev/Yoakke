using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    public class SemanticTokensOptions : IWorkDoneProgressOptions
    {
        public class RangeOptions
        {
        }

        public class FullOptions
        {
            /// <summary>
            /// The server supports deltas for full documents.
            /// </summary>
            [JsonProperty("delta", NullValueHandling = NullValueHandling.Ignore)]
            public bool? Delta { get; set; }
        }

        [JsonProperty("workDoneProgress", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WorkDoneProgress { get; set; }

        /// <summary>
        /// The legend used by the server
        /// </summary>
        [JsonProperty("legend")]
        public SemanticTokensLegend Legend { get; set; }

        /// <summary>
        /// Server supports providing semantic tokens for a specific range
        /// of a document.
        /// </summary>
        [JsonProperty("range", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, RangeOptions>? Range { get; set; }

        /// <summary>
        /// Server supports providing semantic tokens for a full document.
        /// </summary>
        [JsonProperty("full", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, FullOptions>? Full { get; set; }
    }
}
