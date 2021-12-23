// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Platform.X86.Operands;

/// <summary>
/// A <see cref="Label"/> reference.
/// </summary>
public readonly struct LabelRef : IOperand
{
  /// <summary>
  /// Utility to create a new <see cref="LabelRef"/>, before the <see cref="Label"/> is ever
  /// added to the code.
  /// </summary>
  /// <param name="name">The name of the <see cref="Label"/> to create.</param>
  /// <param name="label">The created <see cref="Label"/> gets written here, so it can be added
  /// to the <see cref="Assembly"/> at a later point.</param>
  /// <returns>A new <see cref="LabelRef"/>, that references <paramref name="label"/>.</returns>
  public static LabelRef Forward(string name, out Label label)
  {
    label = new Label(name);
    return new(label);
  }

  /// <inheritdoc/>
  public bool IsMemory => true;

  /// <inheritdoc/>
  public DataWidth? GetSize() => null;

  /// <inheritdoc/>
  public DataWidth GetSize(AssemblyContext context) => context.AddressSize;

  /// <summary>
  /// The referenced <see cref="X86.Label"/>.
  /// </summary>
  public readonly Label Label;

  /// <summary>
  /// An offset from the label.
  /// </summary>
  public readonly int Offset;

  /// <summary>
  /// Initializes a new instance of the <see cref="LabelRef"/> struct.
  /// </summary>
  /// <param name="label">The referenced <see cref="Label"/>.</param>
  /// <param name="offset">An optional offset.</param>
  public LabelRef(Label label, int offset = 0)
  {
    this.Label = label;
    this.Offset = offset;
  }
}
