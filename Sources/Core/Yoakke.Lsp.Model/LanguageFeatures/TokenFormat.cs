// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Runtime.Serialization;

namespace Yoakke.Lsp.Model.LanguageFeatures
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#tokenFormat.
    /// </summary>
    public enum TokenFormat
    {
        /// <summary>
        /// Relative token format.
        /// </summary>
        [EnumMember(Value = "relative")]
        Relative,
    }
}
