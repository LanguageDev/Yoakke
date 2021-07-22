// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.SyntaxTree.Attributes
{
    /// <summary>
    /// An attribute to mark a member be ignored by the syntax tree generator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SyntaxTreeIgnoreAttribute : Attribute
    {
    }
}
