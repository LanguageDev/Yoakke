// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Yoakke.Ir.Model.Attributes;

namespace Yoakke.Ir.Model;

/// <summary>
/// Base of all IR value types.
/// </summary>
public abstract record Type : Constant, IAttributeTarget
{
  #region AttributeTarget

  /// <inheritdoc/>
  public Attributes.AttributeTargets Flag => this.attributeTarget.Flag;

  private readonly AttributeTarget attributeTarget = new(Attributes.AttributeTargets.TypeDefinition);

  /// <inheritdoc/>
  public IEnumerable<IAttribute> GetAttributes() => this.attributeTarget.GetAttributes();

  /// <inheritdoc/>
  public IEnumerable<IAttribute> GetAttributes(string name) => this.attributeTarget.GetAttributes(name);

  /// <inheritdoc/>
  public IEnumerable<TAttrib> GetAttributes<TAttrib>()
      where TAttrib : IAttribute => this.attributeTarget.GetAttributes<TAttrib>();

  /// <inheritdoc/>
  public bool TryGetAttribute(string name, [MaybeNullWhen(false)] out IAttribute attribute) =>
      this.attributeTarget.TryGetAttribute(name, out attribute);

  /// <inheritdoc/>
  public bool TryGetAttribute<TAttrib>([MaybeNullWhen(false)] out TAttrib attribute)
      where TAttrib : IAttribute => this.attributeTarget.TryGetAttribute(out attribute);

  /// <inheritdoc/>
  public void AddAttribute(IAttribute attribute) => this.attributeTarget.AddAttribute(attribute);

  #endregion AttributeTarget

  /* Variants */

  /// <summary>
  /// A type of any type.
  /// </summary>
  public record Type_ : Type
  {
    /// <summary>
    /// A default instance to use.
    /// </summary>
    public static readonly Type_ Instance = new();

    /// <inheritdoc/>
    public override Type Type => this;

    /// <inheritdoc/>
    public override string ToString() => "type";
  }

  /// <summary>
  /// A void-type, representing the empty/nothing type.
  /// </summary>
  public record Void : Type
  {
    /// <summary>
    /// A default instance to use.
    /// </summary>
    public static readonly Void Instance = new();

    /// <inheritdoc/>
    public override Type Type => Type_.Instance;

    /// <inheritdoc/>
    public override string ToString() => "void";
  }

  /// <summary>
  /// A signed integer type with a given bit-width.
  /// </summary>
  public new record Int(int Bits) : Type
  {
    /// <inheritdoc/>
    public override Type Type => Type_.Instance;

    /// <inheritdoc/>
    public override string ToString() => $"i{this.Bits}";
  }
}
