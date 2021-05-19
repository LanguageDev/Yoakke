using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Ast.Attributes
{
    /// <summary>
    /// An attribute to annotate that the given node is part of an AST.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AstAttribute : Attribute
    {
    }
}
