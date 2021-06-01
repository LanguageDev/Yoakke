using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Lsp.Model.TextSynchronization;

namespace Yoakke.Lsp.Model.Capabilities.Server
{
    public class ServerCapabilities
    {
        /// <summary>
        /// Defines how text documents are synced. Is either a detailed structure
        /// defining each notification or for backwards compatibility the
        /// TextDocumentSyncKind number. If omitted it defaults to
        /// `TextDocumentSyncKind.None`.
        /// </summary>
        [JsonProperty("textDocumentSync", NullValueHandling = NullValueHandling.Ignore)]
        public Either<TextDocumentSyncOptions, TextDocumentSyncKind>? TextDocumentSync { get; set; }
        /// <summary>
        /// The server provides completion support.
        /// </summary>
        [JsonProperty("completionProvider", NullValueHandling = NullValueHandling.Ignore)]
        public CompletionOptions? CompletionProvider { get; set; }
        /// <summary>
        /// The server provides hover support.
        /// </summary>
        [JsonProperty("hoverProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, HoverOptions>? HoverProvider { get; set; }
        /// <summary>
        /// The server provides signature help support.
        /// </summary>
        [JsonProperty("signatureHelpProvider", NullValueHandling = NullValueHandling.Ignore)]
        public SignatureHelpOptions? SignatureHelpProvider { get; set; }
#if false
        /// <summary>
        /// The server provides go to declaration support.
        /// </summary>
        [Since(3, 14, 0)]
        [JsonProperty("declarationProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, DeclarationOptions, DeclarationRegistrationOptions>? DeclarationProvider { get; set; }
        /// <summary>
        /// The server provides goto definition support.
        /// </summary>
        [JsonProperty("definitionProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, DefinitionOptions>? DefinitionProvider { get; set; }
        /// <summary>
        /// The server provides goto type definition support.
        /// </summary>
        [Since(3, 6, 0)]
        [JsonProperty("typeDefinitionProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, TypeDefinitionOptions, TypeDefinitionRegistrationOptions>? TypeDefinitionProvider { get; set; }
        /// <summary>
        /// The server provides goto implementation support.
        /// </summary>
        [Since(3, 6, 0)]
        [JsonProperty("implementationProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, ImplementationOptions, ImplementationRegistrationOptions>? ImplementationProvider { get; set; }
        /// <summary>
        /// The server provides find references support.
        /// </summary>
        [JsonProperty("referencesProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, ReferenceOptions>? ReferencesProvider { get; set; }
        /// <summary>
        /// The server provides document highlight support.
        /// </summary>
        [JsonProperty("documentHighlightProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, DocumentHighlightOptions>? DocumentHighlightProvider { get; set; }
        /// <summary>
        /// The server provides document symbol support.
        /// </summary>
        [JsonProperty("documentSymbolProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, DocumentSymbolOptions>? DocumentSymbolProvider { get; set; }
        /// <summary>
        /// The server provides code actions. The `CodeActionOptions` return type is
        /// only valid if the client signals code action literal support via the
        /// property `textDocument.codeAction.codeActionLiteralSupport`.
        /// </summary>
        [JsonProperty("codeActionProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, CodeActionOptions>? CodeActionProvider { get; set; }
        /// <summary>
        /// The server provides code lens.
        /// </summary>
        [JsonProperty("codeLensProvider", NullValueHandling = NullValueHandling.Ignore)]
        public CodeLensOptions? CodeLensProvider { get; set; }
        /// <summary>
        /// The server provides document link support.
        /// </summary>
        [JsonProperty("documentLinkProvider", NullValueHandling = NullValueHandling.Ignore)]
        public DocumentLinkOptions? DocumentLinkProvider { get; set; }
        /// <summary>
        /// The server provides color provider support.
        /// </summary>
        [Since(3, 6, 0)]
        [JsonProperty("colorProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, DocumentColorOptions, DocumentColorRegistrationOptions>? ColorProvider { get; set; }
        /// <summary>
        /// The server provides document formatting.
        /// </summary>
        [JsonProperty("documentFormattingProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, DocumentFormattingOptions>? DocumentFormattingProvider { get; set; }
        /// <summary>
        /// The server provides document range formatting.
        /// </summary>
        [JsonProperty("documentRangeFormattingProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, DocumentRangeFormattingOptions>? DocumentRangeFormattingProvider { get; set; }
        /// <summary>
        /// The server provides document formatting on typing.
        /// </summary>
        [JsonProperty("documentOnTypeFormattingProvider", NullValueHandling = NullValueHandling.Ignore)]
        public DocumentOnTypeFormattingOptions? DocumentOnTypeFormattingProvider { get; set; }
        /// <summary>
        /// The server provides rename support. RenameOptions may only be
        /// specified if the client states that it supports
        /// `prepareSupport` in its initial `initialize` request.
        /// </summary>
        [JsonProperty("renameProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, RenameOptions>? RenameProvider { get; set; }
        /// <summary>
        /// The server provides folding provider support.
        /// </summary>
        [Since(3, 10, 0)]
        [JsonProperty("foldingRangeProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, FoldingRangeOptions, FoldingRangeRegistrationOptions>? FoldingRangeProvider { get; set; }
        /// <summary>
        /// The server provides execute command support.
        /// </summary>
        [JsonProperty("executeCommandProvider", NullValueHandling = NullValueHandling.Ignore)]
        public ExecuteCommandOptions? ExecuteCommandProvider { get; set; }
        /// <summary>
        /// The server provides selection range support.
        /// </summary>
        [Since(3, 15, 0)]
        [JsonProperty("selectionRangeProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, SelectionRangeOptions, SelectionRangeRegistrationOptions>? SelectionRangeProvider { get; set; }
        /// <summary>
        /// The server provides linked editing range support.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("linkedEditingRangeProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, LinkedEditingRangeOptions, LinkedEditingRangeRegistrationOptions>? LinkedEditingRangeProvider { get; set; }
        /// <summary>
        /// The server provides call hierarchy support.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("callHierarchyProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, CallHierarchyOptions, CallHierarchyRegistrationOptions>? CallHierarchyProvider { get; set; }
        /// <summary>
        /// The server provides semantic tokens support.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("semanticTokensProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<SemanticTokensOptions, SemanticTokensRegistrationOptions>? SemanticTokensProvider { get; set; }
        /// <summary>
        /// Whether server provides moniker support.
        /// </summary>
        [Since(3, 16, 0)]
        [JsonProperty("monikerProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, MonikerOptions, MonikerRegistrationOptions>? MonikerProvider { get; set; }
        /// <summary>
        /// The server provides workspace symbol support.
        /// </summary>
        [JsonProperty("workspaceSymbolProvider", NullValueHandling = NullValueHandling.Ignore)]
        public Either<bool, WorkspaceSymbolOptions>? WorkspaceSymbolProvider { get; set; }
        /// <summary>
        /// Workspace specific server capabilities
        /// </summary>
        [JsonProperty("workspace", NullValueHandling = NullValueHandling.Ignore)]
        public Workspace? Workspace { get; set; }
        /// <summary>
        /// Experimental server capabilities.
        /// </summary>
        [JsonProperty("experimental", NullValueHandling = NullValueHandling.Ignore)]
        public object? Experimental { get; set; }
#endif
    }
}
