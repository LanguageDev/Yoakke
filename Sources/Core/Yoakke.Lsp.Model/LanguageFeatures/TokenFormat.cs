// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Runtime.Serialization;

namespace Yoakke.Lsp.Model.LanguageFeatures
{
    public enum TokenFormat
    {
        [EnumMember(Value = "relative")]
        Relative,
    }
}
