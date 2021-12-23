// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic;

/// <summary>
/// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#range.
/// </summary>
public readonly struct Range
{
  /// <summary>
  /// The range's start position.
  /// </summary>
  [JsonProperty("start")]
  public Position Start { get; init; }

  /// <summary>
  /// The range's end position.
  /// </summary>
  [JsonProperty("end")]
  public Position End { get; init; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Range"/> struct.
  /// </summary>
  /// <param name="start">The starting <see cref="Position"/>.</param>
  /// <param name="end">The ending <see cref="Position"/>.</param>
  public Range(Position start, Position end)
  {
    this.Start = start;
    this.End = end;
  }
}
