using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Ast.Attributes;

namespace Yoakke.Sample
{
    [Ast]
    [Visitor("PassVisitor", typeof(void))]
    [Visitor("TreeEvaluator", typeof(void))]
    public abstract partial class AstNode
    {
    }

    [Ast]
    public abstract partial class Statement : AstNode
    {
        [Ast]
        public partial class Program : Statement
        {
            public readonly IReadOnlyList<Statement> Statements;
        }

        [Ast]
        public partial class List : Statement
        {
            public readonly IReadOnlyList<Statement> Statements;
        }

        [Ast]
        public partial class Var : Statement
        {
            public readonly string Name;
            public readonly Expression Value;
        }

        [Ast]
        public partial class Func : Statement
        {
            public readonly string Name;
            public readonly IReadOnlyList<string> Parameters;
            public readonly Statement Body;
        }

        [Ast]
        public partial class Return : Statement
        {
            public readonly Expression? Value;
        }

        [Ast]
        public partial class If : Statement
        {
            public readonly Expression Condition;
            public readonly Statement Then;
            public readonly Statement? Else;
        }

        [Ast]
        public partial class While : Statement
        {
            public readonly Expression Condition;
            public readonly Statement Body;
        }

        [Ast]
        public partial class Expr : Statement
        {
            public readonly Expression Expression;
        }
    }

    public enum UnaryOp
    {
        Negate,
        Ponate,
        Not,
    }

    public enum BinOp
    {
        Assign,
        Add, Sub, Mul, Div, Mod,
        Eq, Neq, Greater, Less, GreaterEq, LessEq,
        And, Or,
    }

    [Ast]
    [Visitor("TreeEvaluator", typeof(object))]
    public abstract partial class Expression : AstNode
    {
        [Ast]
        public partial class Call : Expression
        {
            public readonly Expression Function;
            public readonly IReadOnlyList<Expression> Arguments;
        }

        [Ast]
        public partial class Unary : Expression
        {
            public readonly UnaryOp Op;
            public readonly Expression Subexpr;
        }

        [Ast]
        public partial class Binary : Expression
        {
            public readonly Expression Left;
            public readonly BinOp Op;
            public readonly Expression Right;
        }

        [Ast]
        public partial class Ident : Expression
        {
            public readonly string Name;
        }

        [Ast]
        public partial class StringLit : Expression
        {
            public readonly string Value;
        }

        [Ast]
        public partial class IntLit : Expression
        {
            public readonly int Value;
        }
    }
}
