using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lexer
{
    /// <summary>
    /// An attribute to mark an enum value as an error token type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ErrorAttribute : Attribute
    {
    }
}
