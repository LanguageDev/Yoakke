// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Serialization;

namespace Yoakke.Lsp.Model.Basic
{
    [JsonConverter(typeof(ProgressTokenConverter))]
    public readonly struct ProgressToken : IEquatable<ProgressToken>
    {
        public readonly object Value;

        public bool IsString => this.Value is string;

        public bool IsInt => this.Value is int;

        public string AsString => (string)this.Value;

        public int AsInt => (int)this.Value;

        private ProgressToken(object value)
        {
            this.Value = value;
        }

        public ProgressToken(string value)
            : this((object)value)
        {
        }

        public ProgressToken(int value)
            : this((object)value)
        {
        }

        /// <inheritdoc/>
        public bool Equals(ProgressToken other) => this.Value.Equals(other.Value);

        public static implicit operator ProgressToken(string value) => new(value);

        public static implicit operator ProgressToken(int value) => new(value);
    }
}
