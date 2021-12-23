// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Linq;

namespace Yoakke.Ir.Model.Builders;

/// <summary>
/// A builder for <see cref="Assembly"/>s.
/// </summary>
public class AssemblyBuilder : Assembly
{
  /// <summary>
  /// Builds a copy <see cref="Assembly"/> of this builder.
  /// </summary>
  /// <returns>The built <see cref="Assembly"/>.</returns>
  public Assembly Build() => new()
  {
    Procedures = this.Procedures.Values.ToDictionary(p => p.Name),
  };

  /// <summary>
  /// Adds a <see cref="Procedure"/> to this builder.
  /// </summary>
  /// <param name="procedure">The <see cref="Procedure"/> to add.</param>
  /// <returns>This instance, to be able to chain calls.</returns>
  public AssemblyBuilder WithProcedure(Procedure procedure)
  {
    this.Procedures.Add(procedure.Name, procedure);
    return this;
  }
}
