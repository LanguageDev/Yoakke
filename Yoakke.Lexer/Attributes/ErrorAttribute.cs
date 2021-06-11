using System;

namespace Yoakke.Lexer.Attributes
{
    /// <summary>
    /// An attribute to mark an enum value as an error token type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ErrorAttribute : Attribute
    {
    }
}
