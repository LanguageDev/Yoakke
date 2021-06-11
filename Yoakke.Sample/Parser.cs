using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Lexer;
using Yoakke.Parser;
using Yoakke.Parser.Attributes;
using Token = Yoakke.Lexer.Token<Yoakke.Sample.TokenType>;

namespace Yoakke.Sample
{
    [Parser(typeof(TokenType))]
    public partial class Parser
    {
        // Statements

        [Rule("program : stmt*")]
        private static Statement Program(IReadOnlyList<Statement> statements) => new Statement.Program(statements);

        [Rule("stmt : 'func' Ident '(' (Ident (',' Ident)*)? ')' block_stmt")]
        private static Statement Func(
            Token _1, Token name,
            Token _2, Punctuated<Token, Token> paramList, Token _3,
            Statement body) =>
            new Statement.Func(name.Text, paramList.Values.Select(t => t.Text).ToArray(), body);

        [Rule("stmt : 'if' expr block_stmt ('else' block_stmt)?")]
        private static Statement IfElse(Token _1, Expression cond, Statement then, (Token Kw, Statement Body)? els) =>
            new Statement.If(cond, then, els?.Body);

        [Rule("stmt : 'while' expr block_stmt")]
        private static Statement While(Token _1, Expression cond, Statement body) =>
            new Statement.While(cond, body);

        [Rule("stmt : block_stmt")]
        private static Statement Identity(Statement stmt) => stmt;

        [Rule("stmt : 'return' expr? ';'")]
        private static Statement Ret(Token _1, Expression? expr, Token _2) => new Statement.Return(expr);

        [Rule("stmt : 'var' Ident '=' expr ';'")]
        private static Statement Ret(Token _1, Token name, Token _2, Expression value, Token _3) =>
            new Statement.Var(name.Text, value);

        [Rule("block_stmt : '{' stmt* '}'")]
        private static Statement Block(Token _1, IReadOnlyList<Statement> statements, Token _2) =>
            new Statement.List(statements);

        [Rule("stmt: expr ';'")]
        private static Statement Expr(Expression expr, Token _1) => new Statement.Expr(expr);

        // Expressions

        [Left("*", "/", "%")]
        [Left("+", "-")]
        [Left(">", "<", ">=", "<=")]
        [Left("==", "!=")]
        [Left("and")]
        [Left("or")]
        [Right("=")]
        [Rule("expr")]
        private static Expression Bin(Expression left, Token op, Expression right) =>
            new Expression.Binary(left, ToBinOp(op.Kind), right);

        [Rule("expr : pre_expr")]
        [Rule("pre_expr : post_expr")]
        [Rule("post_expr : atom_expr")]
        private static Expression Identity(Expression e) => e;

        [Rule("pre_expr : ('+' | '-' | 'not') pre_expr")]
        private static Expression Unary(Token<TokenType> op, Expression sub) =>
            new Expression.Unary(ToUnaryOp(op.Kind), sub);

        [Rule("post_expr : post_expr '(' (expr (',' expr)*)? ')'")]
        private static Expression Call(Expression func, Token _1, Punctuated<Expression, Token> args, IToken _2) =>
            new Expression.Call(func, args.Values.ToArray());

        [Rule("atom_expr : '(' expr ')'")]
        private static Expression Ident(Token _1, Expression e, Token _2) => e;
        [Rule("atom_expr : Ident")]
        private static Expression Ident(Token t) => new Expression.Ident(t.Text);
        [Rule("atom_expr : IntLit")]
        private static Expression IntLit(Token t) => new Expression.IntLit(int.Parse(t.Text));
        [Rule("atom_expr : StrLit")]
        private static Expression StrLit(Token t) => new Expression.StringLit(EscapeString(t.Text));

        // TODO: Proper escape
        private static string EscapeString(string str) => str.Substring(1, str.Length - 2);

        private static BinOp ToBinOp(TokenType tt) => tt switch
        {
            TokenType.Star => BinOp.Mul,
            TokenType.Slash => BinOp.Div,
            TokenType.Percent => BinOp.Mod,
            TokenType.Plus => BinOp.Add,
            TokenType.Minus => BinOp.Sub,
            TokenType.Greater => BinOp.Greater,
            TokenType.Less => BinOp.Less,
            TokenType.GreaterEqual => BinOp.GreaterEq,
            TokenType.LessEqual => BinOp.LessEq,
            TokenType.Equal => BinOp.Eq,
            TokenType.NotEqual => BinOp.Neq,
            TokenType.KwAnd => BinOp.And,
            TokenType.KwOr => BinOp.Or,
            TokenType.Assign => BinOp.Assign,
            _ => throw new InvalidOperationException(),
        };

        private static UnaryOp ToUnaryOp(TokenType tt) => tt switch
        {

            TokenType.Plus => UnaryOp.Ponate,
            TokenType.Minus => UnaryOp.Negate,
            TokenType.KwNot => UnaryOp.Not,
            _ => throw new InvalidOperationException(),
        };
    }
}
