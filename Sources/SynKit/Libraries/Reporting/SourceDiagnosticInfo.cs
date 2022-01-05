// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.SynKit.Text;

namespace Yoakke.SynKit.Reporting;

/// <summary>
/// A <see cref="IDiagnosticInfo"/> that references part of the source code.
/// </summary>
public class SourceDiagnosticInfo : IDiagnosticInfo
{
    /// <inheritdoc/>
    public Severity? Severity { get; set; }

    /// <inheritdoc/>
    public string? Message { get; set; }

    /// <summary>
    /// The location the diagnostic information refers to.
    /// </summary>
    public Location Location { get; set; }
}
