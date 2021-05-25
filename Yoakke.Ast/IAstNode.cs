using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Ast
{
    /// <summary>
    /// Interface for all AST nodes.
    /// </summary>
    public interface IAstNode
    {
        /// <summary>
        /// The child nodes of this one.
        /// </summary>
        public IEnumerable<IAstNode> ChildNodes { get; }
        /// <summary>
        /// The leaf objects of this node.
        /// </summary>
        public IEnumerable<object> LeafObjects { get; }
    }
}
