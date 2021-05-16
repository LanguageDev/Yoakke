using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Text;

namespace Yoakke.Lexer.Tests
{
    public abstract class TestBase<TKind> where TKind : notnull
    {
        protected static Token<TKind> Tok(string value, TKind tt, Range r) =>
            new Token<TKind>(r, value, tt);

        protected static Range Rn((int Line, int Column) p1, (int Line, int Column) p2) =>
            new Range(new Position(p1.Line, p1.Column), new Position(p2.Line, p2.Column));

        protected static Range Rn((int Line, int Column) p1, int len) =>
            new Range(new Position(p1.Line, p1.Column), len);
    }
}
