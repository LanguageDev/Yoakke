// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.CodeAnalysis;

namespace Yoakke.SourceGenerator.Common
{
    /// <summary>
    /// Extension functionality for <see cref="AttributeData"/>.
    /// </summary>
    public static class AttributeDataExtensions
    {
        /// <summary>
        /// Retrieves a constructor value for a given <see cref="AttributeData"/>.
        /// </summary>
        /// <param name="data">The <see cref="AttributeData"/>.</param>
        /// <param name="idx">The index of the value in the constructor to retrieve.</param>
        /// <returns>The constructor value of <paramref name="data"/> at the given <paramref name="idx"/>.</returns>
        public static object? GetCtorValue(this AttributeData data, int idx = 0) => data.ConstructorArguments[idx].Value;
    }
}
