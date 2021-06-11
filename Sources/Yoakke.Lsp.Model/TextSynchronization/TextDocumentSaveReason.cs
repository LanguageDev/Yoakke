﻿namespace Yoakke.Lsp.Model.TextSynchronization
{
    /// <summary>
    /// Represents reasons why a text document is saved.
    /// </summary>
    public enum TextDocumentSaveReason
    {
        /// <summary>
        /// Manually triggered, e.g. by the user pressing save, by starting
        /// debugging, or by an API call.
        /// </summary>
        Manual = 1,

        /// <summary>
        /// Automatic after a delay.
        /// </summary>
        AfterDelay = 2,

        /// <summary>
        /// When the editor lost focus.
        /// </summary>
        FocusOut = 3,
    }
}
