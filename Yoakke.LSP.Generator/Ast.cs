using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.LSP.Generator
{
    class InterfaceDef
    {
        public readonly string Name;
        public readonly IReadOnlyList<string> Bases;
        public readonly IReadOnlyList<Field> Fields;

        public InterfaceDef(string name, IReadOnlyList<string> bases, IReadOnlyList<Field> fields)
        {
            Name = name;
            Bases = bases;
            Fields = fields;
        }
    }

    class Field
    {
        public readonly string Name;
        public readonly bool Optional;
        public readonly TypeNode Type;

        public Field(string name, bool optional, TypeNode type)
        {
            Name = name;
            Optional = optional;
            Type = type;
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
            public readonly IReadOnlyList<Field> Fields;

            public Object(IReadOnlyList<Field> fields)
            {
                Fields = fields;
            }
        }
    }
}
