// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Serialization
{
    public class DocumentUriConverter : JsonConverter<DocumentUri>
    {
        public override DocumentUri ReadJson(JsonReader reader, Type objectType, DocumentUri existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            new DocumentUri((string)reader.Value);

        public override void WriteJson(JsonWriter writer, DocumentUri value, JsonSerializer serializer) =>
            writer.WriteValue(value.Value);
    }
}
