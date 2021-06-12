// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.CodeAnalysis;

namespace Yoakke.SourceGenerator.Common
{
    public static class AttributeDataExtensions
    {
        public static object? GetCtorValue(this AttributeData data, int idx = 0) => data.ConstructorArguments[idx].Value;
    }
}
