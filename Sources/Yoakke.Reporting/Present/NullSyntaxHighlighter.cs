using System.Collections.Generic;
using Yoakke.Text;

namespace Yoakke.Reporting.Present
{
    /// <summary>
    /// A syntax highlighter that does nothing. Serves as a default.
    /// </summary>
    public class NullSyntaxHighlighter : ISyntaxHighlighter
    {
        /// <summary>
        /// A default instance for the null syntax highlighter.
        /// </summary>
        public static readonly NullSyntaxHighlighter Default = new NullSyntaxHighlighter();

        public SyntaxHighlightStyle Style { get; set; } = SyntaxHighlightStyle.Default;

        public IReadOnlyList<ColoredToken> GetHighlightingForLine(ISourceFile sourceFile, int line) =>
            new ColoredToken[] { };
    }
}
