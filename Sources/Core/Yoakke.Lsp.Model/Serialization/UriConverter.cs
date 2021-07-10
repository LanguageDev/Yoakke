// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Serialization
{
    public class UriConverter : JsonConverter<Basic.Uri>
    {
        public override Basic.Uri ReadJson(JsonReader reader, Type objectType, Basic.Uri existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            new((string)reader.Value);

        public override void WriteJson(JsonWriter writer, Basic.Uri value, JsonSerializer serializer) =>
            writer.WriteValue(value.Value);
    }
}
