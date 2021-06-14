// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Ast.Attributes
{
    /// <summary>
    /// An attribute to annotate that the given node is part of an AST.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AstAttribute : Attribute
    {
    }
}
