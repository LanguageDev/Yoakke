using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lexer
{
    /// <summary>
    /// An attribute to mark an enum value to be the C-like identifier that matches exactly the regular expression 
    /// [A-Za-Z_][A-Za-Z0-9_]*.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class IdentAttribute : Attribute
    {
    }
}
