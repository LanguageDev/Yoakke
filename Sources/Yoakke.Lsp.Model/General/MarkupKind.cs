using System.Runtime.Serialization;

namespace Yoakke.Lsp.Model.General
{
    /// <summary>
    /// Describes the content type that a client supports in various
    /// result literals like `Hover`, `ParameterInfo` or `CompletionItem`.
    ///
    /// Please note that `MarkupKinds` must not start with a `$`. This kinds
    /// are reserved for internal usage.
    /// </summary>
    public enum MarkupKind
    {
        /// <summary>
        /// Plain text is supported as a content format
        /// </summary>
        [EnumMember(Value = "plaintext")]
        PlainText,

        /// <summary>
        /// Markdown is supported as a content format
        /// </summary>
        [EnumMember(Value = "markdown")]
        Markdown,
    }
}
