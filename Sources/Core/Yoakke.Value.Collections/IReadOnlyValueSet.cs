// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Value.Collections
{
    /// <summary>
    /// Represents a generic set of elements that implements value-based equality.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    public interface IReadOnlyValueSet<T> : IReadOnlySet<T>, IEquatable<IReadOnlySet<T>>
    {
    }
}
