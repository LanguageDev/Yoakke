// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Runtime.Serialization;

namespace Yoakke.Lsp.Model.Basic;

/// <summary>
/// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#traceValue.
/// </summary>
public enum TraceValue
{
  /// <summary>
  /// No trace.
  /// </summary>
  [EnumMember(Value = "off")]
  Off,

  /// <summary>
  /// Trace messages.
  /// </summary>
  [EnumMember(Value = "messages")]
  Messages,

  /// <summary>
  /// Verbose trace.
  /// </summary>
  [EnumMember(Value = "verbose")]
  Verbose,
}
