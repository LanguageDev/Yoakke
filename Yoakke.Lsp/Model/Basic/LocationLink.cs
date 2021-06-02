using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    public class LocationLink
    {
        /// <summary>
        /// Span of the origin of this link.
        ///
        /// Used as the underlined span for mouse interaction. Defaults to the word
        /// range at the mouse position.
        /// </summary>
        [JsonProperty("originSelectionRange", NullValueHandling = NullValueHandling.Ignore)]
        public Range? OriginSelectionRange { get; set; }
        /// <summary>
        /// The target resource identifier of this link.
        /// </summary>
        [JsonProperty("targetUri")]
        public DocumentUri TargetUri { get; set; }
        /// <summary>
        /// The full target range of this link. If the target for example is a symbol
        /// then target range is the range enclosing this symbol not including
        /// leading/trailing whitespace but everything else like comments. This
        /// information is typically used to highlight the range in the editor.
        /// </summary>
        [JsonProperty("targetRange")]
        public Range TargetRange { get; set; }
        /// <summary>
        /// The range that should be selected and revealed when this link is being
        /// followed, e.g the name of a function. Must be contained by the the
        /// `targetRange`. See also `DocumentSymbol#range`
        /// </summary>
        [JsonProperty("targetSelectionRange")]
        public Range TargetSelectionRange { get; set; }
    }
}
