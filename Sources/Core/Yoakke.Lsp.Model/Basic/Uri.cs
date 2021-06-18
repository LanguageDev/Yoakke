// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Serialization;

namespace Yoakke.Lsp.Model.Basic
{
    [JsonConverter(typeof(UriConverter))]
    public readonly struct Uri : IEquatable<Uri>
    {
        public readonly string Value;

        public Uri(string value)
        {
            this.Value = value;
        }

        public bool Equals(Uri other) => this.Value.Equals(other.Value);

        public static implicit operator Uri(string value) => new(value);
    }
}
