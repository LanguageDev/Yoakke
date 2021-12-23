// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Newtonsoft.Json;
using Yoakke.Lsp.Model.Serialization;

namespace Yoakke.Lsp.Model.Basic;

/// <summary>
/// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#progress.
/// </summary>
[JsonConverter(typeof(ProgressTokenConverter))]
public readonly struct ProgressToken : IEquatable<ProgressToken>
{
  /// <summary>
  /// The underlying token value.
  /// </summary>
  public readonly object Value;

  /// <summary>
  /// True, if <see cref="Value"/> is a <see cref="string"/>.
  /// </summary>
  public bool IsString => this.Value is string;

  /// <summary>
  /// True, if <see cref="Value"/> is an <see cref="int"/>.
  /// </summary>
  public bool IsInt => this.Value is int;

  /// <summary>
  /// Retrieves <see cref="Value"/> as a <see cref="string"/>.
  /// </summary>
  public string AsString => (string)this.Value;

  /// <summary>
  /// Retrieves <see cref="Value"/> as an <see cref="int"/>.
  /// </summary>
  public int AsInt => (int)this.Value;

  private ProgressToken(object value)
  {
    this.Value = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ProgressToken"/> struct.
  /// </summary>
  /// <param name="value">The token value.</param>
  public ProgressToken(string value)
      : this((object)value)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ProgressToken"/> struct.
  /// </summary>
  /// <param name="value">The token value.</param>
  public ProgressToken(int value)
      : this((object)value)
  {
  }

  /// <inheritdoc/>
  public bool Equals(ProgressToken other) => this.Value.Equals(other.Value);

  /// <summary>
  /// Implicitly casts a <see cref="string"/> to a <see cref="ProgressToken"/>.
  /// </summary>
  /// <param name="value">The <see cref="string"/> to cast.</param>
  public static implicit operator ProgressToken(string value) => new(value);

  /// <summary>
  /// Implicitly casts an <see cref="int"/> to a <see cref="ProgressToken"/>.
  /// </summary>
  /// <param name="value">The <see cref="int"/> to cast.</param>
  public static implicit operator ProgressToken(int value) => new(value);
}
