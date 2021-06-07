using System;
using System.Diagnostics.CodeAnalysis;

namespace Yoakke.Lexer.Attributes
{
    /// <summary>
    /// An attribute to mark an enum value as an error token type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [ExcludeFromCodeCoverage]
    public class ErrorAttribute : Attribute
    {
    }
}
