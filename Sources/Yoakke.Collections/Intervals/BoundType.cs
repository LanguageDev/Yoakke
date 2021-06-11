// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// The different bound types an interval bound can have.
    /// </summary>
    public enum BoundType
    {
        /// <summary>
        /// The given end is unbounded, meaning it goes to infinity and there is no associated bound value.
        /// </summary>
        Unbounded,

        /// <summary>
        /// The associated bound value is excluded from the interval.
        /// </summary>
        Exclusive,

        /// <summary>
        /// The associated bound value is included in the interval.
        /// </summary>
        Inclusive,
    }
}
