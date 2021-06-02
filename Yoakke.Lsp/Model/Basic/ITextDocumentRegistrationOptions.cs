using Newtonsoft.Json;
using System.Collections.Generic;

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// General text document registration options.
    /// </summary>
    public interface ITextDocumentRegistrationOptions
    {
        /// <summary>
        /// A document selector to identify the scope of the registration. If set to
        /// null the document selector provided on the client side will be used.
        /// </summary>
        [JsonProperty("documentSelector")]
        public IReadOnlyList<DocumentFilter>? DocumentSelector { get; set; }
    }
}
