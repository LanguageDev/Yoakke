using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Yoakke.SynKit.RegExes.Tests;

public sealed class PcreParserTests
{
    [InlineData("", "Seq()")]
    [InlineData("a", "'a'")]
    [InlineData("ab", "Seq('a', 'b')")]
    [InlineData("abc", "Seq('a', 'b', 'c')")]
    [InlineData("a|b", "Alt('a', 'b')")]
    [InlineData("a|b|c", "Alt('a', 'b', 'c')")]
    [InlineData("ab|bcd|cdd", "Alt(Seq('a', 'b'), Seq('b', 'c', 'd'), Seq('c', 'd', 'd'))")]
    [Theory]
    public void Simple(string inputText, string astText)
    {
        var ast = PcreParser.Parse(inputText);
        var gotAstText = Stringify(ast);
        Assert.Equal(astText, gotAstText);
    }

    private static string Stringify(PcreAst ast) => ast switch
    {
        PcreAst.Sequence seq => $"Seq({string.Join(", ", seq.Elements.Select(Stringify))})",
        PcreAst.Alternation alt => $"Alt({string.Join(", ", alt.Elements.Select(Stringify))})",
        PcreAst.Literal l => $"'{l.Char}'",
        _ => throw new InvalidOperationException(),
    };
}
