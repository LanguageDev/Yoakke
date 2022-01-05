// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;

namespace Yoakke.SynKit.Reporting.Present;

/// <summary>
/// The style descriptor used unside the presenter.
/// </summary>
public class DiagnosticsStyle
{
    /// <summary>
    /// The default diagnostic style.
    /// </summary>
    public static readonly DiagnosticsStyle Default = new();

    /// <summary>
    /// The color of different severities.
    /// </summary>
    public IDictionary<Severity, ConsoleColor> SeverityColors { get; set; } = new Dictionary<Severity, ConsoleColor>
        {
            { Severity.Note, ConsoleColor.White },
            { Severity.Help, ConsoleColor.Blue },
            { Severity.Warning, ConsoleColor.Yellow },
            { Severity.Error, ConsoleColor.Red },
            { Severity.InternalError, ConsoleColor.Magenta },
        };

    /// <summary>
    /// The color of line numbers.
    /// </summary>
    public ConsoleColor LineNumberColor { get; set; } = ConsoleColor.White;

    /// <summary>
    /// The default color for anything not specified.
    /// </summary>
    public ConsoleColor DefaultColor { get; set; } = ConsoleColor.White;

    /// <summary>
    /// The padding character for line numbers.
    /// </summary>
    public char LineNumberPadding { get; set; } = ' ';

    /// <summary>
    /// The tab size to use in spaces.
    /// </summary>
    public int TabSize { get; set; } = 4;

    /// <summary>
    /// How many lines to print before and after the relevant lines.
    /// </summary>
    public int SurroundingLines { get; set; } = 1;

    /// <summary>
    /// How big of a gap can we connect up between annotated lines.
    /// </summary>
    public int ConnectUpLines { get; set; } = 1;

    /// <summary>
    /// If <see langword="true"/>, source lines that only contain whitespace at the beginning or end of blocks will not be printed.
    /// </summary>
    public bool TrimEmptySourceLinesAtEdges { get; set; } = true;

    /// <summary>
    /// Retrieves the appropriate color for the given <see cref="Severity"/>.
    /// </summary>
    /// <param name="severity">The <see cref="Severity"/> to get the color for.</param>
    /// <returns>The <see cref="ConsoleColor"/> associated with <paramref name="severity"/>, or
    /// <see cref="DefaultColor"/>, if none is associated with it.</returns>
    public ConsoleColor GetSeverityColor(Severity severity) =>
        this.SeverityColors.TryGetValue(severity, out var col) ? col : this.DefaultColor;
}
