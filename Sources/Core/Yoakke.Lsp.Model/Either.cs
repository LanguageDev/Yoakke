// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;
using Yoakke.Lsp.Model.Serialization;

namespace Yoakke.Lsp.Model;

/// <summary>
/// Interface for simple sum-types.
/// </summary>
#pragma warning disable SA1649 // File name should match first type name
public interface IEither
#pragma warning restore SA1649 // File name should match first type name
{
  /// <summary>
  /// The currently held value.
  /// </summary>
  public object Value { get; }
}

/// <summary>
/// A sum-type of two possibilities.
/// </summary>
/// <typeparam name="T1">The type of the first alternative.</typeparam>
/// <typeparam name="T2">The type of the second alternative.</typeparam>
[JsonConverter(typeof(EitherConverter))]
public class Either<T1, T2> : IEither
{
  /// <inheritdoc/>
  public object Value { get; }

  /// <summary>
  /// True, if <see cref="Value"/> is <typeparamref name="T1"/>.
  /// </summary>
  public bool IsFirst => this.Value is T1;

  /// <summary>
  /// True, if <see cref="Value"/> is <typeparamref name="T2"/>.
  /// </summary>
  public bool IsSecond => this.Value is T2;

  /// <summary>
  /// Retrieves <see cref="Value"/> as <typeparamref name="T1"/>.
  /// </summary>
  public T1 AsFirst => (T1)this.Value;

  /// <summary>
  /// Retrieves <see cref="Value"/> as <typeparamref name="T2"/>.
  /// </summary>
  public T2 AsSecond => (T2)this.Value;

  /// <summary>
  /// Initializes a new instance of the <see cref="Either{T1, T2}"/> class.
  /// </summary>
  /// <param name="value">The stored value.</param>
  protected Either(object value)
  {
    this.Value = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Either{T1, T2}"/> class.
  /// </summary>
  /// <param name="value">The value.</param>
  public Either(T1 value)
      : this((object)value!)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Either{T1, T2}"/> class.
  /// </summary>
  /// <param name="value">The value.</param>
  public Either(T2 value)
      : this((object)value!)
  {
  }

  /// <summary>
  /// Implicitly casts a <typeparamref name="T1"/> value to an <see cref="Either{T1, T2}"/>.
  /// </summary>
  /// <param name="value">The value to cast.</param>
  public static implicit operator Either<T1, T2>(T1 value) => new(value);

  /// <summary>
  /// Implicitly casts a <typeparamref name="T2"/> value to an <see cref="Either{T1, T2}"/>.
  /// </summary>
  /// <param name="value">The value to cast.</param>
  public static implicit operator Either<T1, T2>(T2 value) => new(value);
}

/// <summary>
/// A sum-type of three possibilities.
/// </summary>
/// <typeparam name="T1">The type of the first alternative.</typeparam>
/// <typeparam name="T2">The type of the second alternative.</typeparam>
/// <typeparam name="T3">The type of the third alternative.</typeparam>
[JsonConverter(typeof(EitherConverter))]
public class Either<T1, T2, T3> : IEither
{
  /// <inheritdoc/>
  public object Value { get; }

  /// <summary>
  /// True, if <see cref="Value"/> is <typeparamref name="T1"/>.
  /// </summary>
  public bool IsFirst => this.Value is T1;

  /// <summary>
  /// True, if <see cref="Value"/> is <typeparamref name="T2"/>.
  /// </summary>
  public bool IsSecond => this.Value is T2;

  /// <summary>
  /// True, if <see cref="Value"/> is <typeparamref name="T3"/>.
  /// </summary>
  public bool IsThird => this.Value is T3;

  /// <summary>
  /// Retrieves <see cref="Value"/> as <typeparamref name="T1"/>.
  /// </summary>
  public T1 AsFirst => (T1)this.Value;

  /// <summary>
  /// Retrieves <see cref="Value"/> as <typeparamref name="T2"/>.
  /// </summary>
  public T2 AsSecond => (T2)this.Value;

  /// <summary>
  /// Retrieves <see cref="Value"/> as <typeparamref name="T3"/>.
  /// </summary>
  public T3 AsThird => (T3)this.Value;

  /// <summary>
  /// Initializes a new instance of the <see cref="Either{T1, T2, T3}"/> class.
  /// </summary>
  /// <param name="value">The stored value.</param>
  protected Either(object value)
  {
    this.Value = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Either{T1, T2, T3}"/> class.
  /// </summary>
  /// <param name="value">The value.</param>
  public Either(T1 value)
      : this((object)value!)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Either{T1, T2, T3}"/> class.
  /// </summary>
  /// <param name="value">The value.</param>
  public Either(T2 value)
      : this((object)value!)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Either{T1, T2, T3}"/> class.
  /// </summary>
  /// <param name="value">The value.</param>
  public Either(T3 value)
      : this((object)value!)
  {
  }

  /// <summary>
  /// Implicitly casts a <typeparamref name="T1"/> value to an <see cref="Either{T1, T2, T3}"/>.
  /// </summary>
  /// <param name="value">The value to cast.</param>
  public static implicit operator Either<T1, T2, T3>(T1 value) => new(value);

  /// <summary>
  /// Implicitly casts a <typeparamref name="T2"/> value to an <see cref="Either{T1, T2, T3}"/>.
  /// </summary>
  /// <param name="value">The value to cast.</param>
  public static implicit operator Either<T1, T2, T3>(T2 value) => new(value);

  /// <summary>
  /// Implicitly casts a <typeparamref name="T3"/> value to an <see cref="Either{T1, T2, T3}"/>.
  /// </summary>
  /// <param name="value">The value to cast.</param>
  public static implicit operator Either<T1, T2, T3>(T3 value) => new(value);
}
