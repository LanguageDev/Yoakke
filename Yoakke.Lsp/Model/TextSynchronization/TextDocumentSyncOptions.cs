using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lsp.Model.TextSynchronization
{
    public class TextDocumentSyncOptions
    {
        /// <summary>
        /// Open and close notifications are sent to the server. If omitted open
        /// close notification should not be sent.
        /// </summary>
        [JsonProperty("openClose", NullValueHandling = NullValueHandling.Ignore)]
        public bool? OpenClose { get; set; }
        /// <summary>
        /// Change notifications are sent to the server. See
        /// TextDocumentSyncKind.None, TextDocumentSyncKind.Full and
        /// TextDocumentSyncKind.Incremental. If omitted it defaults to
        /// TextDocumentSyncKind.None.
        /// </summary>
        [JsonProperty("change", NullValueHandling = NullValueHandling.Ignore)]
        public TextDocumentSyncKind? Change { get; set; }
    }
}
