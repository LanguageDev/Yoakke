using Newtonsoft.Json;
using System.Collections.Generic;
using Yoakke.Lsp.Model.Basic;
using Yoakke.Lsp.Model.TextSynchronization;

namespace Yoakke.Lsp.Model.Capabilities.Server.RegistrationOptions
{
    /// <summary>
    /// Describe options to be used when registering for text document change events.
    /// </summary>
    public class TextDocumentChangeRegistrationOptions : ITextDocumentRegistrationOptions
    {
        [JsonProperty("documentSelector")]
        public IReadOnlyList<DocumentFilter>? DocumentSelector { get; set; }

        /// <summary>
        /// How documents are synced to the server. See TextDocumentSyncKind.Full
        /// and TextDocumentSyncKind.Incremental.
        /// </summary>
        [JsonProperty("syncKind")]
        public TextDocumentSyncKind SyncKind { get; set; }
    }
}
