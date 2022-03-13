// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Intervals;

/// <summary>
/// The types an interval endpoint can be.
/// </summary>
public enum EndpointType
{
    /// <summary>
    /// Goes to one of the infinities.
    /// </summary>
    Unbounded,

    /// <summary>
    /// Includes the specified endpoint value.
    /// </summary>
    Inclusive,

    /// <summary>
    /// Excludes the specified endpoint value.
    /// </summary>
    Exclusive,
}
