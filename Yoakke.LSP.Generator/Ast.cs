using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.LSP.Generator
{
    abstract class DefBase { }

    class InterfaceDef : DefBase
    {
        public readonly string Name;
        public readonly IReadOnlyList<string> Bases;
        public readonly IReadOnlyList<InterfaceField> Fields;
        public string? Docs { get; set; }

        public InterfaceDef(string name, IReadOnlyList<string> bases, IReadOnlyList<InterfaceField> fields)
        {
            Name = name;
            Bases = bases;
            Fields = fields;
        }
    }

    class NamespaceDef : DefBase
    {
        public readonly string Name;
        public readonly IReadOnlyList<NamespaceField> Fields;
        public string? Docs { get; set; }

        public NamespaceDef(string name, IReadOnlyList<NamespaceField> fields)
        {
            Name = name;
            Fields = fields;
        }
    }

    class InterfaceField
    {
        public readonly string Name;
        public readonly bool Optional;
        public readonly TypeNode Type;
        public string? Docs { get; set; }

        public InterfaceField(string name, bool optional, TypeNode type)
        {
            Name = name;
            Optional = optional;
            Type = type;
        }
    }

    class NamespaceField
    {
        public readonly string Name;
        public readonly object Value;
        public string? Docs { get; set; }

        public NamespaceField(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }

    abstract class TypeNode
    {
        public class Ident : TypeNode
        {
            public readonly string Name;

            public Ident(string name)
            {
                Name = name;
            }
        }

        public class Array : TypeNode
        {
            public readonly TypeNode ElementType;

            public Array(TypeNode elementType)
            {
                ElementType = elementType;
            }
        }

        public class Or : TypeNode
        {
            public readonly IReadOnlyList<TypeNode> Alternatives;

            public Or(IReadOnlyList<TypeNode> alternatives)
            {
                Alternatives = alternatives;
            }
        }

        public class Object : TypeNode
        {
            public readonly IReadOnlyList<InterfaceField> Fields;

            public Object(IReadOnlyList<InterfaceField> fields)
            {
                Fields = fields;
            }
        }
    }
}
