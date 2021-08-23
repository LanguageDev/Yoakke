// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Ir.Model
{
    /// <summary>
    /// The different kinds of comparisons.
    /// </summary>
    public enum Comparison
    {
        /// <summary>
        /// Compare for less.
        /// </summary>
        Less,

        /// <summary>
        /// Compare for less or equal.
        /// </summary>
        LessEqual,

        /// <summary>
        /// Compare for equality.
        /// </summary>
        Equal,

        /// <summary>
        /// Compare for inequality.
        /// </summary>
        NotEqual,

        /// <summary>
        /// Compare for greater.
        /// </summary>
        Greater,

        /// <summary>
        /// Compare for greater or equal.
        /// </summary>
        GreaterEqual,
    }
}
