using System;
using System.Collections.Generic;

namespace Yoakke.Reporting.Present
{
    /// <summary>
    /// The style descriptor used unside the presenter.
    /// </summary>
    public class DiagnosticStyle
    {
        /// <summary>
        /// The default diagnostic style.
        /// </summary>
        public static readonly DiagnosticStyle Default = new DiagnosticStyle();

        /// <summary>
        /// The color of different severities.
        /// </summary>
        public IDictionary<Severity, ConsoleColor> SeverityColors { get; set; } = new Dictionary<Severity, ConsoleColor>
        {
            { Severity.Note         , ConsoleColor.White   },
            { Severity.Help         , ConsoleColor.Blue    },
            { Severity.Warning      , ConsoleColor.Yellow  },
            { Severity.Error        , ConsoleColor.Red     },
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

        public ConsoleColor GetSeverityColor(Severity severity) =>
            SeverityColors.TryGetValue(severity, out var col) ? col : DefaultColor;
    }
}
