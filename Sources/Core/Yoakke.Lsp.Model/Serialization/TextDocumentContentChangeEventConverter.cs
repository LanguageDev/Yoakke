// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Yoakke.Lsp.Model.TextSynchronization;

namespace Yoakke.Lsp.Model.Serialization
{
    public class TextDocumentContentChangeEventConverter : JsonConverter<TextDocumentContentChangeEvent>
    {
        /// <inheritdoc/>
        public override TextDocumentContentChangeEvent ReadJson(JsonReader reader, Type objectType, TextDocumentContentChangeEvent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            if (obj.ContainsKey("range")) return obj.ToObject<TextDocumentContentChangeEvent.Incremental>(serializer);
            else return obj.ToObject<TextDocumentContentChangeEvent.Full>(serializer);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, TextDocumentContentChangeEvent value, JsonSerializer serializer) =>
            serializer.Serialize(writer, value);
    }
}
