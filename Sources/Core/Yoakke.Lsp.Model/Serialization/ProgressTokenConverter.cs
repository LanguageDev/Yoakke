// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Serialization
{
    /// <summary>
    /// A <see cref="JsonConverter"/> for <see cref="ProgressToken"/>s.
    /// </summary>
    public class ProgressTokenConverter : JsonConverter<ProgressToken>
    {
        /// <inheritdoc/>
        public override ProgressToken ReadJson(JsonReader reader, Type objectType, ProgressToken existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            reader.TokenType == JsonToken.String
                ? new ProgressToken((string?)reader.Value ?? string.Empty)
                : new ProgressToken((int?)reader.Value ?? 0);

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, ProgressToken value, JsonSerializer serializer) =>
            writer.WriteValue(value.Value);
    }
}
