// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Lexer.Attributes;
using Yoakke.SynKit.Parser.Attributes;

namespace Yoakke.SynKit.Parser.Sample;

public enum TokenType
{
    [Error] Error,
    [End] End,
    [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

    [Token("(")] OpenParen,
    [Token(")")] CloseParen,

    [Token("+")] Add,
    [Token("-")] Sub,
    [Token("*")] Mul,
    [Token("/")] Div,
    [Token("%")] Mod,
    [Token("^")] Exp,

    [Token(";")] Semicol,

    [Regex(Regexes.IntLiteral)] IntLit,
}

[Lexer(typeof(TokenType))]
public partial class Lexer
{
}

[Parser(typeof(TokenType))]
public partial class Parser
{
    [Rule("program: expression ';'")]
    public static int Program(int n, IToken _) => n;

    [Right("^")]
    [Left("*", "/", "%")]
    [Left("+", "-")]
    [Rule("expression")]
    public static int BinOp(int a, IToken op, int b) => op.Text switch
    {
        "^" => (int)Math.Pow(a, b),
        "*" => a * b,
        "/" => a / b,
        "%" => a % b,
        "+" => a + b,
        "-" => a - b,
        _ => throw new NotImplementedException(),
    };

    [Rule("expression : '(' expression ')'")]
    public static int Grouping(IToken _1, int n, IToken _2) => n;

    [Rule("expression : IntLit")]
    public static int IntLit(IToken token) => int.Parse(token.Text);

    public void Synchronize()
    {
        for (; this.TokenStream.TryPeek(out var t) && t.Text != ";"; this.TokenStream.Consume(1))
        {
        }
        this.TokenStream.Consume(1);
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var lexer = new Lexer(Console.In);
        var parser = new Parser(lexer);

        while (true)
        {
            var result = parser.ParseProgram();
            if (result.IsOk)
            {
                Console.WriteLine($" => {result.Ok.Value}");
            }
            else
            {
                var err = result.Error;
                foreach (var element in err.Elements.Values)
                {
                    Console.WriteLine($"  expected {string.Join(" or ", element.Expected)} while parsing {element.Context}");
                }
                Console.WriteLine($"  but got {(err.Got == null ? "end of input" : ((IToken<TokenType>)err.Got).Text)}");
                parser.Synchronize();
            }
        }
    }
}
