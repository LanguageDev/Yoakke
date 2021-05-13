using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Parser.Attributes
{
    /// <summary>
    /// An attribute to annotate a rule over a transformation method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RuleAttribute : Attribute
    {
        /// <summary>
        /// The rule in grammar notation.
        /// </summary>
        public string Rule { get; set; }

        /// <summary>
        /// Initializes a new <see cref="RuleAttribute"/>.
        /// </summary>
        /// <param name="rule">The rule in grammar notation.</param>
        public RuleAttribute(string rule)
        {
            Rule = rule;
        }
    }
}
