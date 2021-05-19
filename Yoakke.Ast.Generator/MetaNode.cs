using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Ast.Generator
{
    internal class MetaNode
    {
        public readonly string Name;
        public readonly MetaNode? Parent;
        public readonly IList<MetaNode> Children = new List<MetaNode>();
        public readonly IList<(string Name, Type ReturnType)> Visitors = new List<(string Name, Type ReturnType)>();
        public readonly IList<string> NestClasses = new List<string>();
        public bool IsAbstract { get; set; } = false;
        public bool? ImplementEquality { get; set; } = null;
        public bool? ImplementHash { get; set; } = null;

        public MetaNode(string name, MetaNode? parent)
        {
            Name = name;
            Parent = parent;
        }
    }
}
