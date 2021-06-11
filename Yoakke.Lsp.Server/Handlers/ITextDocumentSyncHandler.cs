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
        /// True, if open and close events should be sent.
        /// </summary>
        public bool SendOpenClose { get; }

        /// <summary>
        /// The synchronization kind.
        /// </summary>
        public TextDocumentSyncKind SyncKind { get; }

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

        /// <summary>
        /// Signals that a text document is about to be saved.
        /// </summary>
        /// <param name="saveParams">The save parameters.</param>
        public void WillSave(WillSaveTextDocumentParams saveParams);

        /// <summary>
        /// Signals that a text document is about to be saved, but allows modifications to be made
        /// before the save.
        /// </summary>
        /// <param name="saveParams">The save parameters.</param>
        /// <returns>An array of text edits, or null, if none is needed.</returns>
        public IReadOnlyList<TextEdit>? WillSaveWaitUntil(WillSaveTextDocumentParams saveParams);

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
