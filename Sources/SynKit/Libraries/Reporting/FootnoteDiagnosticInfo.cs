// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.Reporting;

/// <summary>
/// A footnote diagnostic information.
/// </summary>
public class FootnoteDiagnosticInfo : IDiagnosticInfo
{
    /// <inheritdoc/>
    public Severity? Severity { get; set; }

    /// <inheritdoc/>
    public string? Message { get; set; }
}
