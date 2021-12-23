// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Xml.Serialization;

namespace Yoakke.Platform.X86.Generator.Model;

/// <summary>
/// The ModRM byte for <see cref="Encoding"/> in an <see cref="Instruction"/>.
/// </summary>
public class ModRM
{
  /// <summary>
  /// Mode bits.
  /// </summary>
  [XmlAttribute(AttributeName = "mode")]
  public string Mode { get; set; } = string.Empty;

  /// <summary>
  /// Register bits.
  /// </summary>
  [XmlAttribute(AttributeName = "reg")]
  public string Reg { get; set; } = string.Empty;

  /// <summary>
  /// R/M bits.
  /// </summary>
  [XmlAttribute(AttributeName = "rm")]
  public string Rm { get; set; } = string.Empty;
}
