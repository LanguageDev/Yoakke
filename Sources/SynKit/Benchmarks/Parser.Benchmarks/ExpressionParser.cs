// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Parser;
using Yoakke.SynKit.Lexer.Attributes;
using Yoakke.SynKit.Parser.Attributes;

namespace Yoakke.SynKit.Parser.Benchmarks;

public enum TokenType
{
    [Error] Error,
    [End] End,
    [Ignore][Regex(Regexes.Whitespace)] Whitespace,

    [Token("(")] OpenParen,
    [Token(")")] CloseParen,

    [Token("+")] Add,
    [Token("-")] Sub,
    [Token("*")] Mul,
    [Token("/")] Div,
    [Token("%")] Mod,
    [Token("^")] Exp,

    [Token("?")] Conditional,
    [Token(":")] Colon,
    [Token(",")] Comma,
    [Token("=")] Assignment,

    [Token(";")] Semicol,

    [Regex(Regexes.IntLiteral)] IntLit,
}

[Lexer(typeof(TokenType))]
public partial class Lexer
{
    public List<Token<TokenType>> LexAll()
    {
        var list = new List<Token<TokenType>>();
        while (true)
        {
            var token = Next();
            list.Add(token);
            if (token.Kind == TokenType.End) break;
        }
        return list;
    }
}

[Parser(typeof(TokenType))]
public partial class ExpressionParser
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
}

[Parser(typeof(TokenType))]
public partial class ManualExpressionParser
{
    [Rule("program: expression ';'")]
    public static int Program(int n, IToken _) => n;

    [Rule("expression_level1_operator: ('+' | '-')")]
    [Rule("expression_level2_operator: ('*' | '/' | '%')")]
    [Rule("expression_level3_operator: ('^')")]
    public static IToken Level1Operator(IToken op) => op;

    [Rule("expression: (expression_level1 (expression_level1_operator expression_level1)*)")]
    [Rule("expression_level1: (expression_level2 (expression_level2_operator expression_level2)*)")]
    [Rule("expression_level2: (expression_atomic (expression_level3_operator expression_atomic)*)")]
    public static int Pop(Punctuated<int, IToken> p)
    {
        int result = 0;
        IToken? lastOp = null;
        foreach (var (n, op) in p)
        {
            if (lastOp is null)
            {
                result = n;
                lastOp = op;
            }
            else
            {
                if (op is null)
                {
                    result = BinOp(result, lastOp, n);
                }
                else
                {
                    result = BinOp(result, lastOp, n);
                    lastOp = op;
                }
            }
        }

        return result;
    }

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

    [Rule("expression_atomic : '(' expression ')'")]
    public static int Grouping(IToken _1, int n, IToken _2) => n;

    [Rule("expression_atomic : IntLit")]
    public static int IntLit(IToken token) => int.Parse(token.Text);
}

[Parser(typeof(TokenType))]
public partial class WorstManualExpressionParser
{
    [Rule("program: expression ';'")]
    public static int Program(int n, IToken _) => n;

    [Rule("expression_level1_operator: ('+' | '-')")]
    [Rule("expression_level2_operator: ('*' | '/' | '%')")]
    [Rule("expression_level3_operator: ('^')")]
    public static IToken Level1Operator(IToken op) => op;

    [Rule("expression: expression_level1")]
    [Rule("expression_level1: expression_level2")]
    [Rule("expression_level2: expression_atomic")]
    public static int TrivialRules(int n) => n;

    [Rule("expression: expression_level1 expression_level1_operator expression_level1")]
    [Rule("expression_level1: expression_level1 expression_level2_operator expression_level2")]
    [Rule("expression_level2: expression_atomic expression_level3_operator expression_level2")]
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

    [Rule("expression_atomic : '(' expression ')'")]
    public static int Grouping(IToken _1, int n, IToken _2) => n;

    [Rule("expression_atomic : IntLit")]
    public static int IntLit(IToken token) => int.Parse(token.Text);
}

[Parser(typeof(TokenType))]
public partial class ComplexExpressionParser
{
    [Rule("program: expression ';'")]
    public static int Program(int n, IToken _) => n;

    [Rule("conditional_expression: primary_expression")]
    [Rule("assignment_expression: conditional_expression")]
    [Rule("constant_expression: conditional_expression")]
    [Rule("expression: assignment_expression")]
    [Rule("expression: constant_expression")]
    public static int TrivialRules(int n) => n;

    [Rule("conditional_expression: primary_expression '?' expression ':' conditional_expression")]
    public static int MakeConditionalExpression(int condition, IToken _1, int trueExpression, IToken _2, int falseExpression) => condition == 0 ? falseExpression : trueExpression;

    [Rule("assignment_expression: primary_expression '=' assignment_expression")]
    public static int MakeConditionalExpression(int condition, IToken _1, int trueExpression) => trueExpression;

    [Rule("primary_expression : '(' expression ')'")]
    public static int Grouping(IToken _1, int n, IToken _2) => n;

    [Rule("primary_expression : IntLit")]
    public static int IntLit(IToken token) => int.Parse(token.Text);
}

[Parser(typeof(TokenType))]
public partial class ManualComplexExpressionParser
{
    [Rule("program: expression ';'")]
    public static int Program(int n, IToken _) => n;

    /**
     * This is optimized parse tree.
     * expression - assignment_expression
     *   
     * assignment_expression -- primary_expression
     *                     \--- primary_expression '?' expression ':' conditional_expression
     *                     \--- primary_expression '=' assignment_expression
     *
     * conditional_expression - primary_expression
     *                     \--- primary_expression '?' expression ':' conditional_expression
     *            
     */
    [Rule("conditional_expression: primary_expression")]
    [Rule("assignment_expression: primary_expression")]
    [Rule("constant_expression: conditional_expression")]
    [Rule("expression: assignment_expression")]
    public static int TrivialRules(int n) => n;

    [Rule("conditional_expression: primary_expression '?' expression ':' conditional_expression")]
    [Rule("assignment_expression: primary_expression '?' expression ':' conditional_expression")]
    public static int MakeConditionalExpression(int condition, IToken _1, int trueExpression, IToken _2, int falseExpression) => condition == 0 ? falseExpression : trueExpression;

    [Rule("assignment_expression: primary_expression '=' assignment_expression")]
    public static int MakeConditionalExpression(int condition, IToken _1, int trueExpression) => trueExpression;

    [Rule("primary_expression : '(' expression ')'")]
    public static int Grouping(IToken _1, int n, IToken _2) => n;

    [Rule("primary_expression : IntLit")]
    public static int IntLit(IToken token) => int.Parse(token.Text);
}

