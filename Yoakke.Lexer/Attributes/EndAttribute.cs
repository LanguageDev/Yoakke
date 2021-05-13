using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
