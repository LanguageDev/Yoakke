using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Ast.Attributes
{
    /// <summary>
    /// An attribute to annotate if equality should be auto-implemented for an AST node.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImplementEqualityAttribute : Attribute
    {
        /// <summary>
        /// True, if should auto-implement, false if should not.
        /// </summary>
        public bool Implement { get; set; }

        /// <summary>
        /// Initializes a new <see cref="ImplementEqualityAttribute"/>.
        /// </summary>
        /// <param name="implement">True, if equality should be auto-implemented.</param>
        public ImplementEqualityAttribute(bool implement = true)
        {
            Implement = implement;
        }
    }
}
