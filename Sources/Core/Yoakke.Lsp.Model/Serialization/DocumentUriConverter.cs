// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Serialization;

/// <summary>
/// A <see cref="JsonConverter"/> for <see cref="DocumentUri"/>s.
/// </summary>
public class DocumentUriConverter : JsonConverter<DocumentUri>
{
  /// <inheritdoc/>
  public override DocumentUri ReadJson(JsonReader reader, Type objectType, DocumentUri existingValue, bool hasExistingValue, JsonSerializer serializer) =>
      new((string?)reader.Value ?? string.Empty);

  /// <inheritdoc/>
  public override void WriteJson(JsonWriter writer, DocumentUri value, JsonSerializer serializer) =>
      writer.WriteValue(value.Value);
}
