// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SynKit.SyntaxTree.Attributes;

/// <summary>
/// An attribute to annotate that the given node is part of a syntax tree.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public class SyntaxTreeAttribute : Attribute
{
}
