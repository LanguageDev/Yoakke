// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.Text;

/// <summary>
/// Represents a range in some source file.
/// </summary>
/// <param name="File">The source file.</param>
/// <param name="Range">The range.</param>
public readonly record struct Location(ISourceFile? File, Range Range);
