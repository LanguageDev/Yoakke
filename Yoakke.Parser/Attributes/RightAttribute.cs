using System;

namespace Yoakke.Parser.Attributes
{
    /// <summary>
    /// An attribute to annotate a right-associative operator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RightAttribute : Attribute
    {
        /// <summary>
        /// The separators that should be right-associative.
        /// </summary>
        public object[] Separators { get; set; }

        /// <summary>
        /// Initializes a new <see cref="RightAttribute"/>.
        /// </summary>
        /// <param name="separators">The separator elements that should be right-associative.</param>
        public RightAttribute(params object[] separators)
        {
            this.Separators = separators;
        }
    }
}
