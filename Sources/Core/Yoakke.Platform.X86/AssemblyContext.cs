// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Platform.X86;

/// <summary>
/// Context for assembly configuration.
/// </summary>
public class AssemblyContext
{
  /// <summary>
  /// Returns a default instance with some sensible defaults.
  /// </summary>
  public static AssemblyContext Default => new();

  /// <summary>
  /// The address size for the current compilation.
  /// </summary>
  public DataWidth AddressSize { get; set; } = DataWidth.Dword;
}
