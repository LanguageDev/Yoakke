// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.Reporting;

/// <summary>
/// A single piece of information for a <see cref="Diagnostics"/>.
/// </summary>
public interface IDiagnosticInfo
{
    /// <summary>
    /// The severity of the information.
    /// </summary>
    public Severity? Severity { get; }

    /// <summary>
    /// The information message.
    /// </summary>
    public string? Message { get; }
}
