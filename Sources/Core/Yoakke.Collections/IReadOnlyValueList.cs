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
    /// Represents a generic read-only list of elements that implements value-based equality.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public interface IReadOnlyValueList<T> : IReadOnlyList<T>, IEquatable<IReadOnlyList<T>>
        where T : IEquatable<T>
    {
    }
}
