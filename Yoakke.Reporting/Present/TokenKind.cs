using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Reporting.Present
{
    /// <summary>
    /// Different token categories for presenting.
    /// </summary>
    public enum TokenKind
    {
        Comment,
        Keyword,
        Literal,
        Name,
        Punctuation,
        Operator,
        Other,
    }
}
