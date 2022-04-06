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
    [InlineData(@"(ba){34,123}c", "Seq(Quant[34,123](Seq('b', 'a')), 'c')")]
    [InlineData(@"]", "']'")]
    [InlineData(@"[a]", "Cc[+]('a')")]
    [InlineData(@"[abc]", "Cc[+]('a', 'b', 'c')")]
    [InlineData(@"[a-c]", "Cc[+]('a' to 'c')")]
    [InlineData(@"[a-cA-Z]", "Cc[+]('a' to 'c', 'A' to 'Z')")]
    [InlineData(@"[1a-c2A-Z3]", "Cc[+]('1', 'a' to 'c', '2', 'A' to 'Z', '3')")]
    [InlineData(@"[^a]", "Cc[-]('a')")]
    [InlineData(@"[^abc]", "Cc[-]('a', 'b', 'c')")]
    [InlineData(@"[^a-c]", "Cc[-]('a' to 'c')")]
    [InlineData(@"[^a-cA-Z]", "Cc[-]('a' to 'c', 'A' to 'Z')")]
    [InlineData(@"[^1a-c2A-Z3]", "Cc[-]('1', 'a' to 'c', '2', 'A' to 'Z', '3')")]
    [InlineData(@"[-ab]", "Cc[+]('-', 'a', 'b')")]
    [InlineData(@"[-a-b]", "Cc[+]('-', 'a' to 'b')")]
    [InlineData(@"[^-ab]", "Cc[-]('-', 'a', 'b')")]
    [InlineData(@"[^-a-b]", "Cc[-]('-', 'a' to 'b')")]
    [InlineData(@"[]]", "Cc[+](']')")]
    [InlineData(@"[][]", "Cc[+](']', '[')")]
    [InlineData(@"[]-a]", "Cc[+](']' to 'a')")]
    [InlineData(@"[^]]", "Cc[-](']')")]
    [InlineData(@"[^]-b]", "Cc[-](']' to 'b')")]
    [InlineData(@"[[:ascii:]]", "NamedCc[+](ascii)")]
    [InlineData(@"[[:alnum:]]", "NamedCc[+](alnum)")]
    [InlineData(@"[[:^xdigit:]]", "NamedCc[-](xdigit)")]
    [InlineData(@"[[:]]", "Seq(Cc[+]('[', ':'), ']')")]
    [InlineData(@"\12", "'\n'")]
    [InlineData(@"\41", "'!'")]
    [InlineData(@"\165", "'u'")]
    [InlineData(@"\x2f", "'/'")]
    [InlineData(@"\x2F", "'/'")]
    [InlineData(@"\x{3c9}", "'ω'")]
    [InlineData(@"\x{3C9}", "'ω'")]
    [InlineData(@"\.", "'.'")]
    [InlineData(@"\Q.(] {+?\E", "Q'.(] {+?'")]
    [InlineData(@"[a-\Qqwe\E]", "Cc[+]('a' to 'q')")]
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
        PcreAst.CharacterClass cc => $"Cc[{(cc.Invert ? '-' : '+')}]({string.Join(", ", cc.Elements.Select(Stringify))})",
        PcreAst.CharacterClassRange r => $"'{r.From}' to '{r.To}'",
        PcreAst.NamedCharacterClass ncc => $"NamedCc[{(ncc.Invert ? '-' : '+')}]({ncc.Name})",
        PcreAst.Literal l => $"'{l.Char}'",
        PcreAst.Quoted q => $"Q'{q.Text}'",
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
