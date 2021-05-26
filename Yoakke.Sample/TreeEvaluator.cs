using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Sample
{
    public class TreeEvaluator : AstNode.TreeEvaluator
    {
        public object Evaluate(Expression expr) => Visit(expr);

        protected override object Visit(Expression.Binary bin) => bin.Op switch
        {
            BinOp.Add => (int)Visit(bin.Left) + (int)Visit(bin.Right),
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

            _ => throw new NotSupportedException(),
        };

        protected override object Visit(Expression.IntLit intLit) => intLit.Value;
    }
}
