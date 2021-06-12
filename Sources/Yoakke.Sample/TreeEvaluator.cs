// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Symbols;

namespace Yoakke.Sample
{
    public class TreeEvaluator : AstNode.TreeEvaluator
    {
        private class Return : Exception
        {
            public readonly object? Value;

            public Return(object? value)
            {
                this.Value = value;
            }
        }

        private class StackFrame
        {
            public readonly Dictionary<ISymbol, object> Values = new();
        }

        private readonly SymbolResolution symbolResolution;
        private readonly StackFrame globalFrame = new();
        private readonly Stack<StackFrame> callStack = new();

        public TreeEvaluator(SymbolResolution symbolResolution)
        {
            this.symbolResolution = symbolResolution;
        }

        public void Execute(Statement stmt) => Visit(stmt);

        public object Evaluate(Expression expr) => Visit(expr);

        // Statement related ///////////////////////////////////////////////////

        protected override void Visit(Statement.Var vars)
        {
            var symbol = this.symbolResolution.Symbols[vars];
            Bind(symbol, Visit(vars.Value));
        }

        protected override void Visit(Statement.Func func)
        { /* no-op */
        }

        protected override void Visit(Statement.Return ret) =>
            throw new Return(ret.Value == null ? null : Visit(ret.Value));

        protected override void Visit(Statement.If iff)
        {
            if ((bool)Visit(iff.Condition)) Visit(iff.Then);
            else if (iff.Else != null) Visit(iff.Else);
        }

        protected override void Visit(Statement.While whil)
        {
            while ((bool)Visit(whil.Condition)) Visit(whil.Body);
        }

        // Expression related //////////////////////////////////////////////////

        protected override object? Visit(Expression.Call call)
        {
            var func = Visit(call.Function);
            var args = call.Arguments.Select(Visit).ToArray();
            if (func is Statement.Func langFunc)
            {
                // Language function
                if (langFunc.Parameters.Count != args.Length)
                {
                    throw new InvalidOperationException("argc mismatch");
                }
                // Push frame
                var frame = new StackFrame();
                this.callStack.Push(frame);
                // Bind arguments
                foreach (var (name, value) in langFunc.Parameters.Zip(args))
                {
                    var symbol = this.symbolResolution.Symbols[(langFunc, name)];
                    frame.Values[symbol] = value;
                }
                // Evaluate, return value
                object? returnValue = null;
                try
                {
                    Visit(langFunc.Body);
                }
                catch (Return ret)
                {
                    returnValue = ret.Value;
                }
                // Pop frame
                this.callStack.Pop();
                return returnValue;
            }
            else if (func is Func<object[], object> nativeFunc)
            {
                // Native function
                return nativeFunc(args);
            }
            else
            {
                throw new InvalidOperationException("tried to call non-function");
            }
        }

        protected override object Visit(Expression.Unary ury) => ury.Op switch
        {
            UnaryOp.Negate => -(int)Visit(ury.Subexpr),
            UnaryOp.Ponate => (int)Visit(ury.Subexpr),
            UnaryOp.Not => !(bool)Visit(ury.Subexpr),

            _ => throw new NotSupportedException(),
        };

        protected override object Visit(Expression.Binary bin) => bin.Op switch
        {
            BinOp.Add => PerformAdd(Visit(bin.Left), (int)Visit(bin.Right)),

            BinOp.Sub => (int)Visit(bin.Left) - (int)Visit(bin.Right),
            BinOp.Mul => (int)Visit(bin.Left) * (int)Visit(bin.Right),
            BinOp.Div => (int)Visit(bin.Left) / (int)Visit(bin.Right),
            BinOp.Mod => (int)Visit(bin.Left) % (int)Visit(bin.Right),

            BinOp.And => (bool)Visit(bin.Left) && (bool)Visit(bin.Right),
            BinOp.Or => (bool)Visit(bin.Left) || (bool)Visit(bin.Right),

            BinOp.Eq => EqualityComparer<object>.Default.Equals(Visit(bin.Left), Visit(bin.Right)),
            BinOp.Neq => !EqualityComparer<object>.Default.Equals(Visit(bin.Left), Visit(bin.Right)),

            BinOp.Greater => (int)Visit(bin.Left) > (int)Visit(bin.Right),
            BinOp.Less => (int)Visit(bin.Left) < (int)Visit(bin.Right),
            BinOp.GreaterEq => (int)Visit(bin.Left) >= (int)Visit(bin.Right),
            BinOp.LessEq => (int)Visit(bin.Left) <= (int)Visit(bin.Right),

            // TODO
            BinOp.Assign => throw new NotImplementedException(),

            _ => throw new NotSupportedException(),
        };

        protected override object? Visit(Expression.Ident ident)
        {
            var symbol = this.symbolResolution.Symbols[ident];
            if (symbol is VarSymbol)
            {
                // Variable
                if (this.callStack.TryPeek(out var top) && top.Values.TryGetValue(symbol, out var value)) return value;
                return this.globalFrame.Values[symbol];
            }
            else
            {
                // Constant
                return ((ConstSymbol)symbol).Value;
            }
        }

        protected override object Visit(Expression.StringLit strLit) => strLit.Value;

        protected override object Visit(Expression.IntLit intLit) => intLit.Value;

        // Runtime drivers /////////////////////////////////////////////////////

        private object Bind(ISymbol symbol, object value)
        {
            if (this.callStack.TryPeek(out var top))
            {
                return top.Values[symbol] = value;
            }
            else
            {
                return this.globalFrame.Values[symbol] = value;
            }
        }

        private object PerformAdd(object left, object right)
        {
            if (left is string || right is string) return $"{left}{right}";
            return (int)left + (int)right;
        }
    }
}
