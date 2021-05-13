using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Parser.Attributes
{
    /// <summary>
    /// An attribute to annotate a left-associative operator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class LeftAttribute : Attribute
    {
        /// <summary>
        /// The separators that should be left-associative.
        /// </summary>
        public object[] Separators { get; set; }

        /// <summary>
        /// Initializes a new <see cref="LeftAttribute"/>.
        /// </summary>
        /// <param name="separators">The separator elements that should be left-associative.</param>
        public LeftAttribute(params object[] separators)
        {
            Separators = separators;
        }
    }
}
