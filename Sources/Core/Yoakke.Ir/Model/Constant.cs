// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Numerics;

namespace Yoakke.Ir.Model;

/// <summary>
/// Represents some compile-time constant value.
/// </summary>
public abstract record Constant
{
  /// <summary>
  /// The <see cref="Model.Type"/> of this constant.
  /// </summary>
  public abstract Type Type { get; }

  /* Variants */

  public record Int : Constant
  {
    /// <inheritdoc/>
    public override Type Type { get; }

    /// <summary>
    /// The value of this integer.
    /// </summary>
    public BigInteger Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Int"/> class.
    /// </summary>
    /// <param name="type">The exact integer type.</param>
    /// <param name="value">The numeric value.</param>
    public Int(Type.Int type, BigInteger value)
    {
      this.Type = type;
      this.Value = value;
    }
  }
}
