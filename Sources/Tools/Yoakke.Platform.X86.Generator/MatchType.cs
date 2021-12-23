// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Platform.X86.Generator;

/// <summary>
/// The type of byte-match for the subnode.
/// </summary>
public enum MatchType
{
  /// <summary>
  /// Requires a matching opcode.
  /// </summary>
  Opcode,

  /// <summary>
  /// Requires a matching prefix.
  /// </summary>
  Prefix,

  /// <summary>
  /// The ModRM register extends the opcode.
  /// </summary>
  ModRmReg,

  /// <summary>
  /// Anything else, usually a leaf.
  /// </summary>
  None,
}
