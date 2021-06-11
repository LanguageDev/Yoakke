// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Yoakke.Text;

namespace Yoakke.Reporting
{
    /// <summary>
    /// A single diagnostic information with all relevant things.
    /// </summary>
    public class Diagnostic
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

        public Diagnostic WithSeverity(Severity severity)
        {
            this.Severity = severity;
            return this;
        }

        public Diagnostic WithCode(string code)
        {
            this.Code = code;
            return this;
        }

        public Diagnostic WithMessage(string message)
        {
            this.Message = message;
            return this;
        }

        public Diagnostic WithInfo(IDiagnosticInfo info)
        {
            this.Information.Add(info);
            return this;
        }

        public Diagnostic WithSourceInfo(Location location, Severity severity, string message) =>
            this.WithInfo(new SourceDiagnosticInfo { Location = location, Severity = severity, Message = message });

        public Diagnostic WithSourceInfo(Location location, Severity severity) =>
            this.WithInfo(new SourceDiagnosticInfo { Location = location, Severity = severity });

        public Diagnostic WithSourceInfo(Location location, string message) =>
            this.WithInfo(new SourceDiagnosticInfo { Location = location, Message = message });

        public Diagnostic WithSourceInfo(Location location) =>
            this.WithInfo(new SourceDiagnosticInfo { Location = location });

        public Diagnostic WithFootnoteInfo(Severity severity, string footnote) =>
            this.WithInfo(new FootnoteDiagnosticInfo { Severity = severity, Message = footnote });

        public Diagnostic WithFootnoteInfo(string footnote) =>
            this.WithInfo(new FootnoteDiagnosticInfo { Message = footnote });
    }
}
