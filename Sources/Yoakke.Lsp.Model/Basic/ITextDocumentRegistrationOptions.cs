// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// General text document registration options.
    /// </summary>
    public interface ITextDocumentRegistrationOptions
    {
        /// <summary>
        /// A document selector to identify the scope of the registration. If set to
        /// null the document selector provided on the client side will be used.
        /// </summary>
        [JsonProperty("documentSelector")]
        public IReadOnlyList<DocumentFilter>? DocumentSelector { get; set; }
    }
}
