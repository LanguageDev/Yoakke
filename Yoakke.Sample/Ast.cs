using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Sample
{
    public abstract class Ast
    {
        public abstract string Dump();
    }

    public abstract class Statement : Ast
    {
        public class List : Statement
        {
            public readonly IReadOnlyList<Statement> Statements;

            public List(IReadOnlyList<Statement> statements)
            {
                Statements = statements;
            }

            public override string Dump() =>
                $"<statement_list>{string.Join("", Statements.Select(s => s.Dump()))}</statement_list>";
        }

        public class Var : Statement
        {
            public readonly string Name;
            public readonly Expression Value;

            public Var(string name, Expression value)
            {
                Name = name;
                Value = value;
            }

            public override string Dump() => $"<var_definition name=\"{Name}\">{Value.Dump()}</<var_definition>>";
        }

        public class Func : Statement
        {
            public readonly string Name;
            public readonly IReadOnlyList<string> Parameters;
            public readonly Statement Body;

            public Func(string name, IReadOnlyList<string> parameters, Statement body)
            {
                Name = name;
                Parameters = parameters;
                Body = body;
            }

            public override string Dump() => 
                $"<func_definition name=\"{Name}\" params=\"{string.Join(", ", Parameters)}\">{Body.Dump()}</func_definition>";
        }

        public class Return : Statement
        {
            public readonly Expression? Value;

            public Return(Expression? value)
            {
                Value = value;
            }

            public override string Dump() => Value == null ? "<return/>" : $"<return>{Value.Dump()}</return>";
        }

        public class If : Statement
        {
            public readonly Expression Condition;
            public readonly Statement Then;
            public readonly Statement? Else;

            public If(Expression condition, Statement then, Statement? @else)
            {
                Condition = condition;
                Then = then;
                Else = @else;
            }

            public override string Dump() => 
                $"<if><condition>{Condition.Dump()}</condition><then>{Then.Dump()}</then>{(Else == null ? "" : $"<else>{Else.Dump()}</else>")}</if>";
        }

        public class While : Statement
        {
            public readonly Expression Condition;
            public readonly Statement Body;

            public While(Expression condition, Statement body)
            {
                Condition = condition;
                Body = body;
            }

            public override string Dump() =>
                $"<while><condition>{Condition.Dump()}</condition><body>{Body.Dump()}</body></while>";
        }

        public class Expr : Statement
        {
            public readonly Expression Expression;

            public Expr(Expression expression)
            {
                Expression = expression;
            }

            public override string Dump() => Expression.Dump();
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

    public abstract class Expression : Ast
    {
        public class Call : Expression
        {
            public readonly Expression Function;
            public readonly IReadOnlyList<Expression> Arguments;

            public Call(Expression function, IReadOnlyList<Expression> arguments)
            {
                Function = function;
                Arguments = arguments;
            }

            public override string Dump() => 
                $"<call><function>{Function.Dump()}</function><arguments>{string.Join("", Arguments.Select(a => $"<arg>{a.Dump()}</arg>"))}</arguments></call>";
        }

        public class Unary : Expression
        {
            public readonly UnaryOp Op;
            public readonly Expression Subexpr;

            public Unary(UnaryOp op, Expression subexpr)
            {
                Op = op;
                Subexpr = subexpr;
            }

            public override string Dump() => $"<unary op=\"{Op}\">{Subexpr.Dump()}</unary>";
        }

        public class Binary : Expression
        {
            public readonly Expression Left;
            public readonly BinOp Op;
            public readonly Expression Right;

            public Binary(Expression left, BinOp op, Expression right)
            {
                Left = left;
                Op = op;
                Right = right;
            }

            public override string Dump() => 
                $"<binary op=\"{Op}\"><left>{Left.Dump()}</left><right>{Right.Dump()}</right></binary>";
        }

        public class Ident : Expression
        {
            public readonly string Name;

            public Ident(string name)
            {
                Name = name;
            }

            public override string Dump() => $"<ident name=\"{Name}\"/>";
        }

        public class StringLit : Expression
        {
            public readonly string Value;

            public StringLit(string value)
            {
                Value = value;
            }

            public override string Dump() => $"<str value={Value}/>";
        }

        public class IntLit : Expression
        {
            public readonly int Value;

            public IntLit(int value)
            {
                Value = value;
            }

            public override string Dump() => $"<int value=\"{Value}\"/>";
        }
    }
}
