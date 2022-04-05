using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Yoakke.SynKit.RegExes.Tests;

public sealed class PcreParserTests
{
    [InlineData(@"", "Seq()")]
    [InlineData(@" ", "' '")]
    [InlineData(@"a", "'a'")]
    [InlineData(@"ab", "Seq('a', 'b')")]
    [InlineData(@"a b", "Seq('a', ' ', 'b')")]
    [InlineData(@"abc", "Seq('a', 'b', 'c')")]
    [InlineData(@"a|b", "Alt('a', 'b')")]
    [InlineData(@"a|b|c", "Alt('a', 'b', 'c')")]
    [InlineData(@"ab|bcd|cdd", "Alt(Seq('a', 'b'), Seq('b', 'c', 'd'), Seq('c', 'd', 'd'))")]
    [InlineData(@"(a|b)c", "Seq(Alt('a', 'b'), 'c')")]
    [InlineData(@"w(a|b)c", "Seq('w', Alt('a', 'b'), 'c')")]
    [InlineData(@"(a)", "'a'")]
    [InlineData(@"\(a\)", "Seq('(', 'a', ')')")]
    [InlineData(@"a*", "Quant[*]('a')")]
    [InlineData(@"a+", "Quant[+]('a')")]
    [InlineData(@"a?", "Quant[?]('a')")]
    [InlineData(@"a{3}", "Quant[3]('a')")]
    [InlineData(@"a{123}", "Quant[123]('a')")]
    [InlineData(@"a{123,}", "Quant[123,]('a')")]
    [InlineData(@"a{,123}", "Quant[,123]('a')")]
    [InlineData(@"a{34,123}", "Quant[34,123]('a')")]
    [InlineData(@"ba{34,123}", "Seq('b', Quant[34,123]('a'))")]
    [InlineData(@"ba{34,123}c", "Seq('b', Quant[34,123]('a'), 'c')")]
    [InlineData(@"(ba){34,123}", "Quant[34,123](Seq('b', 'a'))")]
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
        PcreAst.Quantified quant => $"Quant[{Stringify(quant.Quantifier)}]({Stringify(quant.Element)})",
        PcreAst.Literal l => $"'{l.Char}'",
        _ => throw new InvalidOperationException(),
    };

    private static string Stringify(Quantifier quantifier) => quantifier switch
    {
        Quantifier.ZeroOrMore => "*",
        Quantifier.OneOrMore => "+",
        Quantifier.Optional => "?",
        Quantifier.AtLeast l => $"{l.Amount},",
        Quantifier.AtMost m => $",{m.Amount}",
        Quantifier.Exactly e => $"{e.Amount}",
        Quantifier.Between b => $"{b.Min},{b.Max}",
        _ => throw new InvalidOperationException(),
    };
}
