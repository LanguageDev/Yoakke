using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Yoakke.Lsp.Model.TextSynchronization;

namespace Yoakke.Lsp.Model.Serialization
{
    public class TextDocumentContentChangeEventConverter : JsonConverter<TextDocumentContentChangeEvent>
    {
        public override TextDocumentContentChangeEvent ReadJson(JsonReader reader, Type objectType, TextDocumentContentChangeEvent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            if (obj.ContainsKey("range")) return obj.ToObject<TextDocumentContentChangeEvent.Incremental>(serializer);
            else return obj.ToObject<TextDocumentContentChangeEvent.Full>(serializer);
        }

        public override void WriteJson(JsonWriter writer, TextDocumentContentChangeEvent value, JsonSerializer serializer) =>
            serializer.Serialize(writer, value);
    }
}
