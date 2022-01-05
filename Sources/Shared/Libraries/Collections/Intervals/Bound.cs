// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Intervals;

/// <summary>
/// A common base for interval endpoints.
/// </summary>
/// <typeparam name="T">The type of the endpoint value.</typeparam>
public abstract record Bound<T> : IComparable<Bound<T>>
{
    /// <inheritdoc/>
    public int CompareTo(Bound<T> other) => BoundComparer<T>.Default.Compare(this, other);

    /// <inheritdoc/>
    public override int GetHashCode() => BoundComparer<T>.Default.GetHashCode(this);
}
