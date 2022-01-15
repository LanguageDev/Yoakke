// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SynKit.Lexer.Attributes;

/// <summary>
/// An attribute to mark an enum value as an error token type.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
internal class ErrorAttribute : Attribute
{
}