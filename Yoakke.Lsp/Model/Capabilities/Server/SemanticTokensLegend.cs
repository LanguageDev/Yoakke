using Newtonsoft.Json;
using System.Collections.Generic;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    public class SemanticTokensLegend
    {
        /// <summary>
        /// The token types a server uses.
        /// </summary>
        [JsonProperty("tokenTypes")]
        public IReadOnlyList<string> TokenTypes { get; set; }
        /// <summary>
        /// The token modifiers a server uses.
        /// </summary>
        [JsonProperty("tokenModifiers")]
        public IReadOnlyList<string> TokenModifiers { get; set; }
    }
}
