// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Platform.X86.Operands;

/// <summary>
/// An x86 address specification.
/// </summary>
public readonly struct Address : IOperand
{
  /// <inheritdoc/>
  public bool IsMemory => true;

  /// <inheritdoc/>
  public DataWidth? GetSize() => null;

  /// <inheritdoc/>
  public DataWidth GetSize(AssemblyContext context) => context.AddressSize;

  /// <summary>
  /// The optional <see cref="Operands.Segment"/> override.
  /// </summary>
  public Segment? Segment { get; init; }

  /// <summary>
  /// The base address <see cref="Register"/>.
  /// </summary>
  public Register? Base { get; init; }

  /// <summary>
  /// A scaled offset.
  /// </summary>
  public ScaledIndex? ScaledIndex { get; init; }

  /// <summary>
  /// A <see cref="LabelRef"/> displacement value.
  /// </summary>
  public LabelRef? DisplacementLabel { get; init; }

  /// <summary>
  /// A displacement constant.
  /// </summary>
  public int Displacement { get; init; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="segment">The optional <see cref="Operands.Segment"/> override.</param>
  /// <param name="base">The base address <see cref="Register"/>.</param>
  /// <param name="scaledIndex">A scaled offset.</param>
  /// <param name="displacementLabel">A <see cref="LabelRef"/> displacement value.</param>
  /// <param name="displacement">A displacement constant.</param>
  public Address(
      Segment? segment = null,
      Register? @base = null,
      ScaledIndex? scaledIndex = null,
      LabelRef? displacementLabel = null,
      int displacement = 0)
  {
    this.Segment = segment;
    this.Base = @base;
    this.ScaledIndex = scaledIndex;
    this.DisplacementLabel = displacementLabel;
    this.Displacement = displacement;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="base">The base address <see cref="Register"/>.</param>
  /// <param name="scaledIndex">A scaled offset.</param>
  /// <param name="displacementLabel">A <see cref="LabelRef"/> displacement value.</param>
  /// <param name="displacement">A displacement constant.</param>
  public Address(
      Register? @base,
      ScaledIndex? scaledIndex = null,
      LabelRef? displacementLabel = null,
      int displacement = 0)
      : this(null, @base, scaledIndex, displacementLabel, displacement)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="scaledIndex">A scaled offset.</param>
  /// <param name="displacementLabel">A <see cref="LabelRef"/> displacement value.</param>
  /// <param name="displacement">A displacement constant.</param>
  public Address(
      ScaledIndex? scaledIndex,
      LabelRef? displacementLabel = null,
      int displacement = 0)
      : this(null, null, scaledIndex, displacementLabel, displacement)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="displacementLabel">A <see cref="LabelRef"/> displacement value.</param>
  /// <param name="displacement">A displacement constant.</param>
  public Address(
      LabelRef? displacementLabel,
      int displacement = 0)
      : this(null, null, null, displacementLabel, displacement)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="displacement">A displacement constant.</param>
  public Address(int displacement)
      : this(null, null, null, null, displacement)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="scaledIndex">A scaled offset.</param>
  /// <param name="displacement">A displacement constant.</param>
  public Address(ScaledIndex? scaledIndex, int displacement)
      : this(null, null, scaledIndex, null, displacement)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="segment">The optional <see cref="Operands.Segment"/> override.</param>
  /// <param name="scaledIndex">A scaled offset.</param>
  /// <param name="displacementLabel">A <see cref="LabelRef"/> displacement value.</param>
  /// <param name="displacement">A displacement constant.</param>
  public Address(
      Segment? segment,
      ScaledIndex? scaledIndex,
      LabelRef? displacementLabel = null,
      int displacement = 0)
      : this(segment, null, scaledIndex, displacementLabel, displacement)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="base">The base address <see cref="Register"/>.</param>
  /// <param name="displacementLabel">A <see cref="LabelRef"/> displacement value.</param>
  /// <param name="displacement">A displacement constant.</param>
  public Address(
      Register? @base,
      LabelRef? displacementLabel,
      int displacement = 0)
      : this(null, @base, null, displacementLabel, displacement)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="segment">The optional <see cref="Operands.Segment"/> override.</param>
  /// <param name="base">The base address <see cref="Register"/>.</param>
  /// <param name="displacementLabel">A <see cref="LabelRef"/> displacement value.</param>
  /// <param name="displacement">A displacement constant.</param>
  public Address(
      Segment? segment,
      Register? @base,
      LabelRef? displacementLabel,
      int displacement = 0)
      : this(segment, @base, null, displacementLabel, displacement)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="base">The base address <see cref="Register"/>.</param>
  /// <param name="displacement">A displacement constant.</param>
  public Address(Register? @base, int displacement)
      : this(null, @base, null, null, displacement)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="base">The base address <see cref="Register"/>.</param>
  /// <param name="scaledIndex">A scaled offset.</param>
  /// <param name="displacement">A displacement constant.</param>
  public Address(Register? @base, ScaledIndex? scaledIndex, int displacement)
      : this(null, @base, scaledIndex, null, displacement)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="segment">The optional <see cref="Operands.Segment"/> override.</param>
  /// <param name="displacementLabel">A <see cref="LabelRef"/> displacement value.</param>
  /// <param name="displacement">A displacement constant.</param>
  public Address(Segment? segment, LabelRef? displacementLabel, int displacement = 0)
      : this(segment, null, null, displacementLabel, displacement)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="segment">The optional <see cref="Operands.Segment"/> override.</param>
  /// <param name="displacement">A displacement constant.</param>
  public Address(Segment? segment, int displacement)
      : this(segment, null, null, null, displacement)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="segment">The optional <see cref="Operands.Segment"/> override.</param>
  /// <param name="scaledIndex">A scaled offset.</param>
  /// <param name="displacement">A displacement constant.</param>
  public Address(Segment? segment, ScaledIndex? scaledIndex, int displacement)
      : this(segment, null, scaledIndex, null, displacement)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="segment">The optional <see cref="Operands.Segment"/> override.</param>
  /// <param name="base">The base address <see cref="Register"/>.</param>
  /// <param name="displacement">A displacement constant.</param>
  public Address(Segment? segment, Register? @base, int displacement)
      : this(segment, @base, null, null, displacement)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Address"/> struct.
  /// </summary>
  /// <param name="segment">The optional <see cref="Operands.Segment"/> override.</param>
  /// <param name="base">The base address <see cref="Register"/>.</param>
  /// <param name="scaledIndex">A scaled offset.</param>
  /// <param name="displacement">A displacement constant.</param>
  public Address(
      Segment? segment,
      Register? @base,
      ScaledIndex? scaledIndex,
      int displacement)
      : this(segment, @base, scaledIndex, null, displacement)
  {
  }
}

/// <summary>
/// Represents a scaled index for X86 addressing.
/// </summary>
public readonly struct ScaledIndex
{
  /// <summary>
  /// The index <see cref="Register"/>.
  /// </summary>
  public readonly Register Index;

  /// <summary>
  /// The index scaling constant. Must be 1, 2, 4 or 8.
  /// </summary>
  public readonly int Scale;

  /// <summary>
  /// Initializes a new instance of the <see cref="ScaledIndex"/> struct.
  /// </summary>
  /// <param name="index">The index <see cref="Register"/>.</param>
  /// <param name="scale">The index stacke.</param>
  public ScaledIndex(Register index, int scale = 1)
  {
    if (scale != 1 && scale != 2 && scale != 4 && scale != 8)
    {
      throw new ArgumentOutOfRangeException(nameof(scale), "scale must be 1, 2, 4 or 8");
    }
    this.Index = index;
    this.Scale = scale;
  }

  /// <summary>
  /// Deconstructs this <see cref="ScaledIndex"/>.
  /// </summary>
  /// <param name="index">The <see cref="Index"/> gets written here.</param>
  /// <param name="scale">The <see cref="Scale"/> gets written here.</param>
  public void Deconstruct(out Register index, out int scale)
  {
    index = this.Index;
    scale = this.Scale;
  }
}
