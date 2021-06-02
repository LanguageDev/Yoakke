using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lsp.Model.Basic
{
    public class VersionedTextDocumentIdentifier : TextDocumentIdentifier
    {
        /// <summary>
        /// The version number of this document.
        ///
        /// The version number of a document will increase after each change,
        /// including undo/redo. The number doesn't need to be consecutive.
        /// </summary>
        [JsonProperty("version")]
        public int Version { get; set; }
    }
}
