using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Sample
{
    public class TreeEvaluator : AstNode.TreeEvaluator
    {
        private class Return : Exception
        {
            public readonly object? Value;

            public Return(object? value)
            {
                Value = value;
            }
        }
        
        private Dictionary<string, object> bindings = new Dictionary<string, object>();

        public void Execute(Statement stmt) => Visit(stmt);
        public object Evaluate(Expression expr) => Visit(expr);

        // Statement related ///////////////////////////////////////////////////

        protected override void Visit(Statement.Var vars) => Bind(vars.Name, Visit(vars.Value));

        protected override void Visit(Statement.Func func) => Bind(func.Name, (Func<object[], object?>)(args => 
            {
                if (func.Parameters.Count != args.Length) throw new InvalidOperationException();
                foreach (var (name, value) in func.Parameters.Zip(args)) Bind(name, value);
                try
                {
                    Visit(func.Body);
                }
                catch (Return ret)
                {
                    return ret.Value;
                }
                return null;
            }));

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
            var func = (Delegate)Evaluate(call.Function);
            return func.DynamicInvoke(new object[] { call.Arguments.Select(Visit).ToArray() });
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

        protected override object Visit(Expression.Ident ident) => bindings[ident.Name];
        protected override object Visit(Expression.StringLit strLit) => strLit.Value;
        protected override object Visit(Expression.IntLit intLit) => intLit.Value;

        // Runtime drivers /////////////////////////////////////////////////////

        public object Bind(string name, object value) => bindings[name] = value;

        private object PerformAdd(object left, object right)
        {
            if (left is string || right is string) return $"{left}{right}";
            return (int)left + (int)right;
        }
    }
}
