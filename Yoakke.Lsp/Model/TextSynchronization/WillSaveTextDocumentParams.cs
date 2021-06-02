using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.TextSynchronization
{
    /// <summary>
    /// The parameters send in a will save text document notification.
    /// </summary>
    public class WillSaveTextDocumentParams
    {
        /// <summary>
        /// The document that will be saved.
        /// </summary>
        [JsonProperty("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }
        /// <summary>
        /// The 'TextDocumentSaveReason'.
        /// </summary>
        [JsonProperty("reason")]
        public TextDocumentSaveReason Reason { get; set; }
    }
}
