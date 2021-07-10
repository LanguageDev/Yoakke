// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Serialization;

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#uri.
    /// </summary>
    [JsonConverter(typeof(UriConverter))]
    public readonly struct Uri : IEquatable<Uri>
    {
        /// <summary>
        /// The raw URI string.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Uri"/> struct.
        /// </summary>
        /// <param name="value">The raw URI string.</param>
        public Uri(string value)
        {
            this.Value = value;
        }

        /// <inheritdoc/>
        public bool Equals(Uri other) => this.Value.Equals(other.Value);

        /// <summary>
        /// Implicitly converts a <see cref="string"/> to a <see cref="Uri"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to convert.</param>
        public static implicit operator Uri(string value) => new(value);
    }
}
