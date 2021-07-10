// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Yoakke.Lsp.Model.Basic;
using Yoakke.Lsp.Model.TextSynchronization;

namespace Yoakke.Lsp.Server.Handlers
{
    // TODO: Async?

    /// <summary>
    /// Handler type for text synchronization.
    /// </summary>
    public interface ITextDocumentSyncHandler : IHandler
    {
        /// <summary>
        /// The synchronization kind.
        /// </summary>
        public TextDocumentSyncKind SyncKind { get; }

        /// <summary>
        /// The list of <see cref="DocumentFilter"/>s to filter for relevant documents, if needed.
        /// </summary>
        public IReadOnlyList<DocumentFilter>? DocumentSelector { get; }

        /// <summary>
        /// Signals a newly opened document.
        /// </summary>
        /// <param name="openParams">The open parameters.</param>
        public void DidOpen(DidOpenTextDocumentParams openParams);

        /// <summary>
        /// Signals that a change occurred in a document.
        /// </summary>
        /// <param name="changeParams">The change parameters.</param>
        public void DidChange(DidChangeTextDocumentParams changeParams);

#if false
        // TODO: Support these?
        /// <summary>
        /// Signals that a text document is about to be saved.
        /// </summary>
        /// <param name="saveParams">The save parameters.</param>
        // public void WillSave(WillSaveTextDocumentParams saveParams);

        // TODO: Support these?
        /// <summary>
        /// Signals that a text document is about to be saved, but allows modifications to be made
        /// before the save.
        /// </summary>
        /// <param name="saveParams">The save parameters.</param>
        /// <returns>An array of text edits, or null, if none is needed.</returns>
        // public IReadOnlyList<TextEdit>? WillSaveWaitUntil(WillSaveTextDocumentParams saveParams);
#endif

        /// <summary>
        /// Signals that a text document is saved.
        /// </summary>
        /// <param name="saveParams">The save parameters.</param>
        public void DidSave(DidSaveTextDocumentParams saveParams);

        /// <summary>
        /// Signals that a text document is closed.
        /// </summary>
        /// <param name="closeParams">The close parameters.</param>
        public void DidClose(DidCloseTextDocumentParams closeParams);
    }
}
