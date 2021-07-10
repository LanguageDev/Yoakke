// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Lsp.Model.LanguageFeatures
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#prepareSupportDefaultBehavior.
    /// </summary>
    public enum PrepareSupportDefaultBehavior
    {
        /// <summary>
        /// The client's default behavior is to select the identifier
        /// according the to language's syntax rule.
        /// </summary>
        Identifier = 1,
    }
}
