// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.LexerUtils.FiniteAutomata
{
    /// <summary>
    /// A type to represent the epsilon symbol for epsilon tranisitions.
    /// </summary>
    public struct Epsilon
    {
        /// <summary>
        /// A constant singleton value to not have to instantiate it all the time.
        /// </summary>
        public static readonly Epsilon Instance = new();
    }
}
