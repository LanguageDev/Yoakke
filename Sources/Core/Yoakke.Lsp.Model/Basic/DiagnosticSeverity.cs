// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#diagnosticSeverity.
    /// </summary>
    public enum DiagnosticSeverity
    {
        /// <summary>
        /// Reports an error.
        /// </summary>
        Error = 1,

        /// <summary>
        /// Reports a warning.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Reports an information.
        /// </summary>
        Information = 3,

        /// <summary>
        /// Reports a hint.
        /// </summary>
        Hint = 4,
    }
}
