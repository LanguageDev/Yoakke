﻿// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Yoakke.Lsp.Model.Serialization
{
    public class EitherConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsAssignableTo(typeof(IEither));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var typeArgs = objectType.GenericTypeArguments;
            var obj = JToken.Load(reader);
            foreach (var typeArg in typeArgs)
            {
                try
                {
                    var ctorValue = obj.ToObject(typeArg, serializer);
                    return Activator.CreateInstance(objectType, ctorValue)!;
                }
                catch (Exception) { }
            }
            throw new NotSupportedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            serializer.Serialize(writer, ((IEither)value).Value);
    }
}
