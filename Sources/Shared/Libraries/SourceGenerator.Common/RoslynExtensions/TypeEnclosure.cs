// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SourceGenerator.Common.RoslynExtensions;

/// <summary>
/// Represents the complete enclosure of a type.
/// </summary>
/// <param name="Prefix">The prefix part of the enclosing types.</param>
/// <param name="Suffix">The suffix part of the enclosing types.</param>
public record class TypeEnclosure(string Prefix, string Suffix);
