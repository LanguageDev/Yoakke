using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using VerifyXunit;
using Xunit;
using Yoakke.SynKit.Parser.Tests;

namespace Yoakke.SynKit.Parser.Generator.Tests;

[UsesVerify]
public class ExpressionParserTests : CodeGenerationTestBase
{
    [Fact]
    public async Task SimpleMathExpression()
    {
        string source = @"
namespace Foo
{
    using Yoakke.SynKit.Parser.Attributes;

    internal enum TokenType
    {
        End,
        Error,
        Whitespace,

        Add,
        Sub,
        Mul,
        Div,
        Exp,
        Number,
        Lparen,
        Rparen,
    }

    [Parser(typeof(TokenType))]
    partial class C
    {
        [Rule(""top_expression : expression End"")]
        private static int TopLevel(int n, IToken _) => n;

        [Right(""^"")]
        [Left(""*"", ""/"")]
        [Left(""+"", ""-"")]
        [Rule(""expression"")]
        private static int Op(int left, IToken op, int right) => op.Text switch
        {
            ""+"" => left + right,
            ""-"" => left - right,
            ""*"" => left * right,
            ""/"" => left / right,
            ""^"" => (int)Math.Pow(left, right),
            _ => throw new InvalidOperationException(),
        };

        [Rule(""expression : '(' expression ')'"")]
        private static int Grouping(IToken _1, int n, IToken _2) => n;

        [Rule(""expression : Number"")]
        private static int Number(IToken tok) => int.Parse(tok.Text);
    }
}";
        await VerifyCSharp(source, NullableContextOptions.Disable);
    }

    [Fact]
    public void InconsistentVisibility()
    {
        string source = @"
namespace Foo
{
    using Yoakke.SynKit.Parser.Attributes;

    internal enum TokenType
    {
        End,
        Error,
        Whitespace,

        Number,
    }

    [Parser(typeof(TokenType))]
    public partial class C
    {
        [Rule(""expression : Number"")]
        private static int Number(IToken tok) => int.Parse(tok.Text);
    }
}";
        var (_, diagnostics) = GenerateCSharpOutput(source, NullableContextOptions.Disable);
        Assert.NotEmpty(diagnostics);
        Assert.All(diagnostics, d => Assert.Equal("YKPARSERGEN003", d.Id));
    }
}
