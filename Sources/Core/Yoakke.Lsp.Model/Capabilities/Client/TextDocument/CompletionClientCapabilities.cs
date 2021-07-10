// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.General;
using Yoakke.Lsp.Model.LanguageFeatures;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#completionClientCapabilities.
    /// </summary>
    public class CompletionClientCapabilities
    {
        /// <summary>
        /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#completionClientCapabilities.
        /// </summary>
        public class CompletionItemCapabilities
        {
            /// <summary>
            /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#completionClientCapabilities.
            /// </summary>
            public class TagSupportCapabilities
            {
                /// <summary>
                /// The tags supported by the client.
                /// </summary>
                [JsonProperty("valueSet")]
                public IReadOnlyList<CompletionItemTag> ValueSet { get; set; }
            }

            /// <summary>
            /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#completionClientCapabilities.
            /// </summary>
            public class ResolveSupportCapabilities
            {
                /// <summary>
                /// The properties that a client can resolve lazily.
                /// </summary>
                [JsonProperty("properties")]
                public IReadOnlyList<string> Properties { get; set; }
            }

            /// <summary>
            /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#completionClientCapabilities.
            /// </summary>
            public class InsertTextModeSupportCapabilities
            {
                /// <summary>
                /// The supported <see cref="InsertTextMode"/>s.
                /// </summary>
                [JsonProperty("valueSet")]
                public IReadOnlyList<InsertTextMode> ValueSet { get; set; }
            }

            /// <summary>
            /// Client supports snippets as insert text.
            ///
            /// A snippet can define tab stops and placeholders with `$1`, `$2`
            /// and `${3:foo}`. `$0` defines the final tab stop, it defaults to
            /// the end of the snippet. Placeholders with equal identifiers are
            /// linked, that is typing in one will update others too.
            /// </summary>
            [JsonProperty("snippetSupport", NullValueHandling = NullValueHandling.Ignore)]
            public bool? SnippetSupport { get; set; }

            /// <summary>
            /// Client supports commit characters on a completion item.
            /// </summary>
            [JsonProperty("commitCharactersSupport", NullValueHandling = NullValueHandling.Ignore)]
            public bool? CommitCharactersSupport { get; set; }

            /// <summary>
            /// Client supports the follow content formats for the documentation
            /// property. The order describes the preferred format of the client.
            /// </summary>
            [JsonProperty("documentationFormat", NullValueHandling = NullValueHandling.Ignore)]
            public IReadOnlyList<MarkupKind>? DocumentationFormat { get; set; }

            /// <summary>
            /// Client supports the deprecated property on a completion item.
            /// </summary>
            [JsonProperty("deprecatedSupport", NullValueHandling = NullValueHandling.Ignore)]
            public bool? DeprecatedSupport { get; set; }

            /// <summary>
            /// Client supports the preselect property on a completion item.
            /// </summary>
            [JsonProperty("preselectSupport", NullValueHandling = NullValueHandling.Ignore)]
            public bool? PreselectSupport { get; set; }

            /// <summary>
            /// Client supports the tag property on a completion item. Clients
            /// supporting tags have to handle unknown tags gracefully. Clients
            /// especially need to preserve unknown tags when sending a completion
            /// item back to the server in a resolve call.
            /// </summary>
            [Since(3, 15, 0)]
            [JsonProperty("tagSupport", NullValueHandling = NullValueHandling.Ignore)]
            public TagSupportCapabilities? TagSupport { get; set; }

            /// <summary>
            /// Client supports insert replace edit to control different behavior if
            /// a completion item is inserted in the text or should replace text.
            /// </summary>
            [Since(3, 16, 0)]
            [JsonProperty("insertReplaceSupport", NullValueHandling = NullValueHandling.Ignore)]
            public bool? InsertReplaceSupport { get; set; }

            /// <summary>
            /// Indicates which properties a client can resolve lazily on a
            /// completion item. Before version 3.16.0 only the predefined properties
            /// `documentation` and `detail` could be resolved lazily.
            /// </summary>
            [Since(3, 16, 0)]
            [JsonProperty("resolveSupport", NullValueHandling = NullValueHandling.Ignore)]
            public ResolveSupportCapabilities? ResolveSupport { get; set; }

            /// <summary>
            /// The client supports the `insertTextMode` property on
            /// a completion item to override the whitespace handling mode
            /// as defined by the client (see `insertTextMode`).
            /// </summary>
            [Since(3, 16, 0)]
            [JsonProperty("insertTextModeSupport", NullValueHandling = NullValueHandling.Ignore)]
            public InsertTextModeSupportCapabilities? InsertTextModeSupport { get; set; }

            /// <summary>
            /// The client has support for completion item label
            /// details (see also `CompletionItemLabelDetails`).
            /// </summary>
            [Since(3, 17, 0)]
            [JsonProperty("labelDetailsSupport", NullValueHandling = NullValueHandling.Ignore)]
            public bool? LabelDetailsSupport { get; set; }
        }

        /// <summary>
        /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#completionClientCapabilities.
        /// </summary>
        public class CompletionItemKindCapabilities
        {
            /// <summary>
            /// The completion item kind values the client supports. When this
            /// property exists the client also guarantees that it will
            /// handle values outside its set gracefully and falls back
            /// to a default value when unknown.
            ///
            /// If this property is not present the client only supports
            /// the completion items kinds from `Text` to `Reference` as defined in
            /// the initial version of the protocol.
            /// </summary>
            [JsonProperty("valueSet", NullValueHandling = NullValueHandling.Ignore)]
            public IReadOnlyList<CompletionItemKind>? ValueSet { get; set; }
        }

        /// <summary>
        /// Whether completion supports dynamic registration.
        /// </summary>
        [JsonProperty("dynamicRegistration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DynamicRegistration { get; set; }

        /// <summary>
        /// The client supports the following `CompletionItem` specific
        /// capabilities.
        /// </summary>
        [JsonProperty("completionItem", NullValueHandling = NullValueHandling.Ignore)]
        public CompletionItemCapabilities? CompletionItem { get; set; }

        /// <summary>
        /// The completion kind capabilities.
        /// </summary>
        [JsonProperty("completionItemKind", NullValueHandling = NullValueHandling.Ignore)]
        public CompletionItemKindCapabilities? CompletionItemKind { get; set; }

        /// <summary>
        /// The client supports to send additional context information for a
        /// `textDocument/completion` request.
        /// </summary>
        [JsonProperty("contextSupport", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ContextSupport { get; set; }

        /// <summary>
        /// The client's default when the completion item doesn't provide a
        /// `insertTextMode` property.
        /// </summary>
        [Since(3, 17, 0)]
        [JsonProperty("insertTextMode", NullValueHandling = NullValueHandling.Ignore)]
        public InsertTextMode? InsertTextMode { get; set; }
    }
}
