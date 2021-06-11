// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Reporting
{
    /// <summary>
    /// A footnote diagnostic information.
    /// </summary>
    public class FootnoteDiagnosticInfo : IDiagnosticInfo
    {
        public Severity? Severity { get; set; }

        public string? Message { get; set; }
    }
}
