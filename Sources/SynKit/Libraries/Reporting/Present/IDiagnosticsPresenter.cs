// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.Reporting.Present;

/// <summary>
/// Interface for objects that can present a <see cref="Diagnostics"/> for the user in some way.
/// </summary>
public interface IDiagnosticsPresenter
{
    /// <summary>
    /// The style to use when presenting.
    /// </summary>
    public DiagnosticsStyle Style { get; set; }

    /// <summary>
    /// The syntax highlighter to use for source code.
    /// </summary>
    public ISyntaxHighlighter SyntaxHighlighter { get; set; }

    /// <summary>
    /// Presents the given <see cref="Diagnostics"/> to the user.
    /// </summary>
    /// <param name="diagnostic">The diagnostic to present.</param>
    public void Present(Diagnostics diagnostic);
}
