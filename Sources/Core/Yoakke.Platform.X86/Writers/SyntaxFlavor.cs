// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Platform.X86.Writers
{
    /// <summary>
    /// The different x86 syntax flavors.
    /// </summary>
    public enum SyntaxFlavor
    {
        /// <summary>
        /// Intel syntax with destination, source ordering.
        /// </summary>
        Intel,

        /// <summary>
        /// AT&amp;T syntax with source, destination ordering.
        /// </summary>
        ATnT,
    }
}
