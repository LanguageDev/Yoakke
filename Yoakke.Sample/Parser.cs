using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Lexer;
using Yoakke.Parser;
using Yoakke.Parser.Attributes;

namespace Yoakke.Sample
{
    [Parser(typeof(TokenType))]
    public partial class Parser
    {
        [Left("*", "/", "%")]
        [Left("+", "-")]
        [Left(">", "<", ">=", "<=")]
        [Left("==", "!=")]
        [Left("and")]
        [Left("or")]
        [Right("=")]
        [Rule("expr")]
        public static Expression Bin(Expression left, Token<TokenType> op, Expression right) =>
            new Expression.Binary(left, ToBinOp(op.Kind), right);

        [Rule("expr : pre_expr")]
        [Rule("pre_expr : post_expr")]
        [Rule("post_expr : atom_expr")]
        public static Expression Identity(Expression e) => e;

        [Rule("pre_expr : ('+' | '-' | 'not') pre_expr")]
        public static Expression Unary(Token<TokenType> op, Expression sub) => 
            new Expression.Unary(ToUnaryOp(op.Kind), sub);

        [Rule("post_expr : post_expr '(' (expr (',' expr)*)? ')'")]
        public static Expression Call(Expression func, IToken _1, Punctuated<Expression, Token<TokenType>> args, IToken _2) =>
            new Expression.Call(func, args.Values.ToArray());

        [Rule("atom_expr : '(' expr ')'")]
        public static Expression Ident(IToken _1, Expression e, IToken _2) => e;
        [Rule("atom_expr : Ident")]
        public static Expression Ident(IToken t) => new Expression.Ident(t.Text);
        [Rule("atom_expr : IntLit")]
        public static Expression IntLit(IToken t) => new Expression.IntLit(int.Parse(t.Text));
        [Rule("atom_expr : StrLit")]
        public static Expression StrLit(IToken t) => new Expression.StringLit(EscapeString(t.Text));

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
