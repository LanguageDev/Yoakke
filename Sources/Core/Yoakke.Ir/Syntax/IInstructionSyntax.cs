// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.Ir.Model;
using Type = System.Type;

namespace Yoakke.Ir.Syntax;

/// <summary>
/// An object to parse and unparse (print) an instruction.
/// </summary>
public interface IInstructionSyntax
{
  /// <summary>
  /// The instruction name that triggers this parser.
  /// </summary>
  public string Name { get; }

  /// <summary>
  /// The implementation <see cref="System.Type"/> of the handled <see cref="Instruction"/>.
  /// </summary>
  public Type Type { get; }

  /// <summary>
  /// Parses an <see cref="Instruction"/>. The identifier is already consumed. The attributes should not be consumed.
  /// </summary>
  /// <param name="parser">The parser to parse from.</param>
  /// <returns>The parsed <see cref="Instruction"/>.</returns>
  public Instruction Parse(IrParser parser);

  /// <summary>
  /// Prints the <see cref="Instruction"/> as text form. The attributes should not be printed.
  /// </summary>
  /// <param name="instruction">The <see cref="Instruction"/> to print, that is guaranteed to be handled by this syntax
  /// handler.</param>
  /// <param name="writer">The <see cref="IrWriter"/> to write to.</param>
  public void Print(Instruction instruction, IrWriter writer);
}
