using System;

namespace Yoakke.Lexer.Attributes
{
    /// <summary>
    /// An attribute to mark an enum value to be an ignored token type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class IgnoreAttribute : Attribute
    {
    }
}
