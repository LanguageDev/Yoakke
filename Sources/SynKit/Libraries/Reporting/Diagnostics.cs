// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Yoakke.SynKit.Text;

namespace Yoakke.SynKit.Reporting;

/// <summary>
/// A single diagnostic information with all relevant things.
/// </summary>
public class Diagnostics
{
    /// <summary>
    /// The severity of the diagnostic.
    /// </summary>
    public Severity? Severity { get; set; }

    /// <summary>
    /// A short identifier code of the diagnostic.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// The summary message of the diagnostic.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// The relevant information for the diagnostic.
    /// </summary>
    public IList<IDiagnosticInfo> Information { get; set; } = new List<IDiagnosticInfo>();

    /// <summary>
    /// Sets the <see cref="Severity"/> of this <see cref="Diagnostics"/>.
    /// </summary>
    /// <param name="severity">The <see cref="Reporting.Severity"/> to set.</param>
    /// <returns>This instance to chain calls.</returns>
    public Diagnostics WithSeverity(Severity severity)
    {
        this.Severity = severity;
        return this;
    }

    /// <summary>
    /// Sets the error-code of this <see cref="Diagnostics"/>.
    /// </summary>
    /// <param name="code">The error-code to set.</param>
    /// <returns>This instance to chain calls.</returns>
    public Diagnostics WithCode(string code)
    {
        this.Code = code;
        return this;
    }

    /// <summary>
    /// Sets the message of this <see cref="Diagnostics"/>.
    /// </summary>
    /// <param name="message">The message to set.</param>
    /// <returns>This instance to chain calls.</returns>
    public Diagnostics WithMessage(string message)
    {
        this.Message = message;
        return this;
    }

    /// <summary>
    /// Adds a <see cref="IDiagnosticInfo"/> to this <see cref="Diagnostics"/>.
    /// </summary>
    /// <param name="info">The <see cref="IDiagnosticInfo"/> to add.</param>
    /// <returns>This instance to chain calls.</returns>
    public Diagnostics WithInfo(IDiagnosticInfo info)
    {
        this.Information.Add(info);
        return this;
    }

    /// <summary>
    /// Adds a <see cref="SourceDiagnosticInfo"/> to this <see cref="Diagnostics"/>.
    /// </summary>
    /// <param name="location">The <see cref="Location"/> of the information.</param>
    /// <param name="severity">The <see cref="Reporting.Severity"/> of the information.</param>
    /// <param name="message">The message of the information.</param>
    /// <returns>This instance to chain calls.</returns>
    public Diagnostics WithSourceInfo(Location location, Severity severity, string message) =>
        this.WithInfo(new SourceDiagnosticInfo { Location = location, Severity = severity, Message = message });

    /// <summary>
    /// Adds a <see cref="SourceDiagnosticInfo"/> to this <see cref="Diagnostics"/>.
    /// </summary>
    /// <param name="location">The <see cref="Location"/> of the information.</param>
    /// <param name="severity">The <see cref="Reporting.Severity"/> of the information.</param>
    /// <returns>This instance to chain calls.</returns>
    public Diagnostics WithSourceInfo(Location location, Severity severity) =>
        this.WithInfo(new SourceDiagnosticInfo { Location = location, Severity = severity });

    /// <summary>
    /// Adds a <see cref="SourceDiagnosticInfo"/> to this <see cref="Diagnostics"/>.
    /// </summary>
    /// <param name="location">The <see cref="Location"/> of the information.</param>
    /// <param name="message">The message of the information.</param>
    /// <returns>This instance to chain calls.</returns>
    public Diagnostics WithSourceInfo(Location location, string message) =>
        this.WithInfo(new SourceDiagnosticInfo { Location = location, Message = message });

    /// <summary>
    /// Adds a <see cref="SourceDiagnosticInfo"/> to this <see cref="Diagnostics"/>.
    /// </summary>
    /// <param name="location">The <see cref="Location"/> of the information.</param>
    /// <returns>This instance to chain calls.</returns>
    public Diagnostics WithSourceInfo(Location location) =>
        this.WithInfo(new SourceDiagnosticInfo { Location = location });

    /// <summary>
    /// Adds a <see cref="FootnoteDiagnosticInfo"/> to this <see cref="Diagnostics"/>.
    /// </summary>
    /// <param name="severity">The <see cref="Reporting.Severity"/> of the information.</param>
    /// <param name="footnote">The message of the information.</param>
    /// <returns>This instance to chain calls.</returns>
    public Diagnostics WithFootnoteInfo(Severity severity, string footnote) =>
        this.WithInfo(new FootnoteDiagnosticInfo { Severity = severity, Message = footnote });

    /// <summary>
    /// Adds a <see cref="FootnoteDiagnosticInfo"/> to this <see cref="Diagnostics"/>.
    /// </summary>
    /// <param name="footnote">The message of the information.</param>
    /// <returns>This instance to chain calls.</returns>
    public Diagnostics WithFootnoteInfo(string footnote) =>
        this.WithInfo(new FootnoteDiagnosticInfo { Message = footnote });
}
