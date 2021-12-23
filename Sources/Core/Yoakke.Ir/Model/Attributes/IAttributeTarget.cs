// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Ir.Model.Attributes;

/// <summary>
/// An attribute target that can be read from and written to.
/// </summary>
public interface IAttributeTarget : IReadOnlyAttributeTarget
{
  /// <summary>
  /// Adds the given <see cref="IAttribute"/> to this <see cref="IAttributeTarget"/>.
  /// </summary>
  /// <param name="attribute">The <see cref="IAttribute"/> to add.</param>
  public void AddAttribute(IAttribute attribute);
}
