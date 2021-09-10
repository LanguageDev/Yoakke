// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Parser.Attributes
{
    /// <summary>
    /// An attribute to mark a field or property as the thing holding onto the token source.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TokenSourceAttribute : Attribute
    {
    }
}
