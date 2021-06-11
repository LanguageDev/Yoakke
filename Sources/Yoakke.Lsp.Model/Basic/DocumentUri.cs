// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Serialization;

namespace Yoakke.Lsp.Model.Basic
{
    [JsonConverter(typeof(DocumentUriConverter))]
    public readonly struct DocumentUri : IEquatable<DocumentUri>
    {
        public readonly string Value;

        public DocumentUri(string value)
        {
            this.Value = value;
        }

        public bool Equals(DocumentUri other) => this.Value.Equals(other.Value);

        public static implicit operator DocumentUri(string value) => new DocumentUri(value);
    }
}
