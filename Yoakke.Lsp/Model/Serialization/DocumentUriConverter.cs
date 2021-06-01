using Newtonsoft.Json;
using System;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Serialization
{
    public class DocumentUriConverter : JsonConverter<DocumentUri>
    {
        public override DocumentUri ReadJson(JsonReader reader, Type objectType, DocumentUri existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            new DocumentUri(reader.ReadAsString());

        public override void WriteJson(JsonWriter writer, DocumentUri value, JsonSerializer serializer) =>
            writer.WriteValue(value.Value);
    }
}
