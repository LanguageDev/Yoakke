using System.Collections.Generic;
using Yoakke.Text;

namespace Yoakke.Reporting.Present
{
    /// <summary>
    /// An interface to provide custom syntax highlighting for source text.
    /// </summary>
    public interface ISyntaxHighlighter
    {
        /// <summary>
        /// A default syntax highlighter that does nothing.
        /// </summary>
        public static readonly ISyntaxHighlighter Null = NullSyntaxHighlighter.Default;

        /// <summary>
        /// The style to use.
        /// </summary>
        public SyntaxHighlightStyle Style { get; set; }

        /// <summary>
        /// Asks for syntax highlighting for a single source line.
        /// </summary>
        /// <param name="sourceFile">The source that contains the line.</param>
        /// <param name="line">The index of the line in the source.</param>
        /// <returns>A list of <see cref="ColoredToken"/>s. Their order does not matter and not all characters have to
        /// belong to a token.</returns>
        public IReadOnlyList<ColoredToken> GetHighlightingForLine(ISourceFile sourceFile, int line);
    }
}
