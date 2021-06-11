using System;

namespace Yoakke.Lexer.Attributes
{
    /// <summary>
    /// An attribute to mark an enum value as the end of source token type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class EndAttribute : Attribute
    {
    }
}
