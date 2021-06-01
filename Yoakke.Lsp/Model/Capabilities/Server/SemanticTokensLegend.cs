using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
