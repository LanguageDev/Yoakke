// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Capabilities.Client.TextDocument
{
    /// <summary>
    /// Text document specific client capabilities.
    /// </summary>
    public class TextDocumentClientCapabilities
    {
        /// <summary>
        /// Text synchronization capabilities.
        /// </summary>
        [JsonProperty("synchronization", NullValueHandling = NullValueHandling.Ignore)]
        public TextDocumentSyncClientCapabilities? Synchronization { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/completion` request.
        /// </summary>
        [JsonProperty("completion", NullValueHandling = NullValueHandling.Ignore)]
        public CompletionClientCapabilities? Completion { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/hover` request.
        /// </summary>
        [JsonProperty("hover", NullValueHandling = NullValueHandling.Ignore)]
        public HoverClientCapabilities? Hover { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/signatureHelp` request.
        /// </summary>
        [JsonProperty("signatureHelp", NullValueHandling = NullValueHandling.Ignore)]
        public SignatureHelpClientCapabilities? SignatureHelp { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/declaration` request.
        /// </summary>
        [Since(3, 14, 0)]
        [JsonProperty("declaration", NullValueHandling = NullValueHandling.Ignore)]
        public DeclarationClientCapabilities? Declaration { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/definition` request.
        /// </summary>
        [JsonProperty("definition", NullValueHandling = NullValueHandling.Ignore)]
        public DefinitionClientCapabilities? Definition { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/typeDefinition` request.
        /// </summary>
        [Since(3, 6, 0)]
        [JsonProperty("typeDefinition", NullValueHandling = NullValueHandling.Ignore)]
        public TypeDefinitionClientCapabilities? TypeDefinition { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/implementation` request.
        /// </summary>
        [Since(3, 6, 0)]
        [JsonProperty("implementation", NullValueHandling = NullValueHandling.Ignore)]
        public ImplementationClientCapabilities? Implementation { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/references` request.
        /// </summary>
        [JsonProperty("references", NullValueHandling = NullValueHandling.Ignore)]
        public ReferenceClientCapabilities? References { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentHighlight` request.
        /// </summary>
        [JsonProperty("documentHighlight", NullValueHandling = NullValueHandling.Ignore)]
        public DocumentHighlightClientCapabilities? DocumentHighlight { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentSymbol` request.
        /// </summary>
        [JsonProperty("documentSymbol", NullValueHandling = NullValueHandling.Ignore)]
        public DocumentSymbolClientCapabilities? DocumentSymbol { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/codeAction` request.
        /// </summary>
        [JsonProperty("codeAction", NullValueHandling = NullValueHandling.Ignore)]
        public CodeActionClientCapabilities? CodeAction { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/codeLens` request.
        /// </summary>
        [JsonProperty("codeLens", NullValueHandling = NullValueHandling.Ignore)]
        public CodeLensClientCapabilities? CodeLens { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentLink` request.
        /// </summary>
        [JsonProperty("documentLink", NullValueHandling = NullValueHandling.Ignore)]
        public DocumentLinkClientCapabilities? DocumentLink { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentColor` and the
        /// `textDocument/colorPresentation` request.
        /// </summary>
        [Since(3, 6, 0)]
        [JsonProperty("colorProvider", NullValueHandling = NullValueHandling.Ignore)]
        public DocumentColorClientCapabilities? ColorProvider { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/formatting` request.
        /// </summary>
        [JsonProperty("formatting", NullValueHandling = NullValueHandling.Ignore)]
        public DocumentFormattingClientCapabilities? Formatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/rangeFormatting` request.
        /// </summary>
        [JsonProperty("rangeFormatting", NullValueHandling = NullValueHandling.Ignore)]
        public DocumentRangeFormattingClientCapabilities? RangeFormatting { get; set; }

        /// <summary>
        /// request.
        /// Capabilities specific to the `textDocument/onTypeFormatting` request.
        /// </summary>
        [JsonProperty("onTypeFormatting", NullValueHandling = NullValueHandling.Ignore)]
        public DocumentOnTypeFormattingClientCapabilities? OnTypeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/rename` request.
        /// </summary>
        [JsonProperty("rename", NullValueHandling = NullValueHandling.Ignore)]
        public RenameClientCapabilities? Rename { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/publishDiagnostics`
        /// notification.
        /// </summary>
        [JsonProperty("publishDiagnostics", NullValueHandling = NullValueHandling.Ignore)]
        public PublishDiagnosticsClientCapabilities? PublishDiagnostics { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/foldingRange` request.
        /// </summary>
        [Since(3, 10, 0)]
        [JsonProperty("foldingRange", NullValueHandling = NullValueHandling.Ignore)]
        public FoldingRangeClientCapabilities? FoldingRange { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/selectionRange` request.
        /// </summary>
        [Since(3, 15, 0)]
        [JsonProperty("selectionRange", NullValueHandling = NullValueHandling.Ignore)]
        public SelectionRangeClientCapabilities? SelectionRange { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/linkedEditingRange` request.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("linkedEditingRange", NullValueHandling = NullValueHandling.Ignore)]
        public LinkedEditingRangeClientCapabilities? LinkedEditingRange { get; set; }

        /// <summary>
        /// Capabilities specific to the various call hierarchy requests.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("callHierarchy", NullValueHandling = NullValueHandling.Ignore)]
        public CallHierarchyClientCapabilities? CallHierarchy { get; set; }

        /// <summary>
        /// Capabilities specific to the various semantic token requests.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("semanticTokens", NullValueHandling = NullValueHandling.Ignore)]
        public SemanticTokensClientCapabilities? SemanticTokens { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/moniker` request.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("moniker", NullValueHandling = NullValueHandling.Ignore)]
        public MonikerClientCapabilities? Moniker { get; set; }
    }
}
