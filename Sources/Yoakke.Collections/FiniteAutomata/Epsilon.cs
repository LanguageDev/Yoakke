// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Collections.FiniteAutomata
{
    /// <summary>
    /// A type to represent the epsilon symbol for epsilon tranisitions.
    /// </summary>
    public struct Epsilon
    {
        public static readonly Epsilon Default = new Epsilon();
    }
}
