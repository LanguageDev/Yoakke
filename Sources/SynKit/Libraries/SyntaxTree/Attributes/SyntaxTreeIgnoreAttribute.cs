// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SynKit.SyntaxTree.Attributes;

/// <summary>
/// An attribute to mark a member be ignored by the syntax tree generator.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class SyntaxTreeIgnoreAttribute : Attribute
{
}
