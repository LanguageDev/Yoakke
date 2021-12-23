// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Linq;

namespace Yoakke.Ir.Model.Builders;

/// <summary>
/// A builder for <see cref="Procedure"/>s.
/// </summary>
public class ProcedureBuilder : Procedure
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ProcedureBuilder"/> class.
  /// </summary>
  /// <param name="name">The name of the procedure.</param>
  public ProcedureBuilder(string name)
      : base(name)
  {
  }

  /// <summary>
  /// Builds a copy <see cref="Procedure"/> of this builder.
  /// </summary>
  /// <returns>The built <see cref="Procedure"/>.</returns>
  public Procedure Build()
  {
    if (ReferenceEquals(this.Entry, BasicBlock.Invalid)) throw new InvalidOperationException("Entry not set for procedure");

    return new(this.Name)
    {
      Entry = this.Entry,
      BasicBlocks = this.BasicBlocks.ToList(),
    };
  }

  /// <summary>
  /// Adds a <see cref="BasicBlock"/> as an entry point to this builder.
  /// </summary>
  /// <param name="basicBlock">The <see cref="BasicBlock"/> to add.</param>
  /// <returns>This instance, to be able to chain calls.</returns>
  public ProcedureBuilder WithEntryAt(BasicBlock basicBlock)
  {
    this.Entry = basicBlock;
    if (!this.BasicBlocks.Contains(basicBlock)) this.BasicBlocks.Add(basicBlock);
    return this;
  }

  /// <summary>
  /// Adds a <see cref="BasicBlock"/> to this builder.
  /// </summary>
  /// <param name="basicBlock">The <see cref="BasicBlock"/> to add.</param>
  /// <returns>This instance, to be able to chain calls.</returns>
  public ProcedureBuilder WithBasicBlock(BasicBlock basicBlock)
  {
    this.BasicBlocks.Add(basicBlock);
    return this;
  }
}
