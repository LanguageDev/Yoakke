using Newtonsoft.Json;
using System;
using Yoakke.Lsp.Model.Basic;
using Yoakke.Lsp.Model.Serialization;
using Range = Yoakke.Lsp.Model.Basic.Range;

namespace Yoakke.Lsp.Model.TextSynchronization
{
    /// <summary>
    /// An event describing a change to a text document. If range and rangeLength are
    /// omitted the new text is considered to be the full content of the document.
    /// </summary>
    [JsonConverter(typeof(TextDocumentContentChangeEventConverter))]
    public abstract class TextDocumentContentChangeEvent
    {
        public class Incremental : TextDocumentContentChangeEvent
        {
            /// <summary>
            /// The range of the document that changed.
            /// </summary>
            [JsonProperty("range")]
            public Range Range { get; set; }
            /// <summary>
            /// The optional length of the range that got replaced.
            /// </summary>
            [JsonProperty("rangeLength", NullValueHandling = NullValueHandling.Ignore)]
            [Obsolete("use range instead.")]
            public uint RangeLength { get; set; }
            /// <summary>
            /// The new text for the provided range.
            /// </summary>
            [JsonProperty("text")]
            public string Text { get; set; }
        }
        
        public class Full : TextDocumentContentChangeEvent
        {
            /// <summary>
            /// The new text of the whole document.
            /// </summary>
            [JsonProperty("text")]
            public string Text { get; set; }
        }
    }
}
