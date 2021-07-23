// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Yoakke.Collections;
using Yoakke.SyntaxTree.Attributes;

namespace Yoakke.Sample
{
    [SyntaxTree]
    [SyntaxTreeVisitor("PassVisitor")]
    [SyntaxTreeVisitor("TreeEvaluator")]
    public abstract partial record AstNode;

    [SyntaxTree]
    public abstract partial record Statement : AstNode
    {
        [SyntaxTree]
        public partial record Program(IReadOnlyValueList<Statement> Statements) : Statement;

        [SyntaxTree]
        public partial record List(IReadOnlyValueList<Statement> Statements) : Statement;

        [SyntaxTree]
        public partial record Var(string Name, Expression Value) : Statement;

        [SyntaxTree]
        public partial record Func(string Name, IReadOnlyValueList<string> Parameters, Statement Body) : Statement;

        [SyntaxTree]
        public partial record Return(Expression? Value) : Statement;

        [SyntaxTree]
        public partial record If(Expression Condition, Statement Then, Statement? Else) : Statement;

        [SyntaxTree]
        public partial record While(Expression Condition, Statement Body) : Statement;

        [SyntaxTree]
        public partial record Expr(Expression Expression) : Statement;
    }

    [SyntaxTree]
    [SyntaxTreeVisitor("TreeEvaluator", typeof(object))]
    public abstract partial record Expression : AstNode
    {
        [SyntaxTree]
        public partial record Call(Expression Function, IReadOnlyValueList<Expression> Arguments) : Expression;

        [SyntaxTree]
        public partial record Unary(UnaryOp Op, Expression Subexpr) : Expression;

        [SyntaxTree]
        public partial record Binary(Expression Left, BinOp Op, Expression Right) : Expression;

        [SyntaxTree]
        public partial record Ident(string Name) : Expression;

        [SyntaxTree]
        public partial record StringLit(string Value) : Expression;

        [SyntaxTree]
        public partial record IntLit(int Value) : Expression;
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
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        Eq,
        Neq,
        Greater,
        Less,
        GreaterEq,
        LessEq,
        And,
        Or,
    }
}
