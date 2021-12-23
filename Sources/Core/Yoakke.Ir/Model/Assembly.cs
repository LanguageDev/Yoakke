// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Yoakke.Ir.Model.Attributes;

namespace Yoakke.Ir.Model;

/// <summary>
/// A full compilation unit, containing procedures.
/// </summary>
public class Assembly : IAttributeTarget
{
  /// <summary>
  /// The procedures defined inside the assembly.
  /// </summary>
  public IDictionary<string, Procedure> Procedures { get; init; } = new Dictionary<string, Procedure>();

  #region AttributeTarget

  /// <inheritdoc/>
  public Attributes.AttributeTargets Flag => this.attributeTarget.Flag;

  private readonly AttributeTarget attributeTarget = new(Attributes.AttributeTargets.Assembly);

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
}
