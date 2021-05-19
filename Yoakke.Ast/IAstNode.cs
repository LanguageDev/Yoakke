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

        /// <summary>
        /// Pretty prints this AST node.
        /// </summary>
        /// <param name="format">The format to pretty-print in.</param>
        /// <returns>The pretty-printed string.</returns>
        public string PrettyPrint(PrettyPrintFormat format);
    }
}
