using System;
using System.Diagnostics.CodeAnalysis;

namespace Yoakke.Lexer.Attributes
{
    /// <summary>
    /// An attribute to mark an enum value as the end of source token type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [ExcludeFromCodeCoverage]
    public class EndAttribute : Attribute
    {
    }
}
