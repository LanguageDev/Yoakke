using System;

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
