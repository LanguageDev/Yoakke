// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lsp.Server.Handlers
{
    /// <summary>
    /// Represents the language client that the server is communicating with.
    /// </summary>
    public interface ILanguageClient
    {
        /// <summary>
        /// Dynamically registers a <see cref="ITextDocumentSyncHandler"/>.
        /// </summary>
        /// <param name="handler">The <see cref="ITextDocumentSyncHandler"/> to register.</param>
        /// <returns>The id to refer to this registration to unregister it later.</returns>
        public string RegisterHandler(ITextDocumentSyncHandler handler);
    }
}
