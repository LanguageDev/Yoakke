// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.LSP.Generator
{
    internal abstract class DefBase
    {
    }

    internal class InterfaceDef : DefBase
    {
        public string Name { get; }
        public IReadOnlyList<string> Bases { get; }
        public IReadOnlyList<InterfaceField> Fields { get; }

        public string? Docs { get; set; }

        public InterfaceDef(string name, IReadOnlyList<string> bases, IReadOnlyList<InterfaceField> fields)
        {
            this.Name = name;
            this.Bases = bases;
            this.Fields = fields;
        }
    }

    internal class NamespaceDef : DefBase
    {
        public string Name { get; }
        public IReadOnlyList<NamespaceField> Fields { get; }

        public string? Docs { get; set; }

        public NamespaceDef(string name, IReadOnlyList<NamespaceField> fields)
        {
            this.Name = name;
            this.Fields = fields;
        }
    }

    internal class InterfaceField
    {
        public string Name { get; }
        public bool Optional { get; }
        public TypeNode Type { get; }

        public string? Docs { get; set; }

        public InterfaceField(string name, bool optional, TypeNode type)
        {
            this.Name = name;
            this.Optional = optional;
            this.Type = type;
        }
    }

    internal class NamespaceField
    {
        public string Name { get; }
        public object Value { get; }

        public string? Docs { get; set; }

        public NamespaceField(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    internal abstract class TypeNode
    {
        public class Ident : TypeNode
        {
            public string Name { get; }

            public Ident(string name)
            {
                this.Name = name;
            }
        }

        public class Array : TypeNode
        {
            public TypeNode ElementType { get; }

            public Array(TypeNode elementType)
            {
                this.ElementType = elementType;
            }
        }

        public class Or : TypeNode
        {
            public IReadOnlyList<TypeNode> Alternatives { get; }

            public Or(IReadOnlyList<TypeNode> alternatives)
            {
                this.Alternatives = alternatives;
            }
        }

        public class Object : TypeNode
        {
            public IReadOnlyList<InterfaceField> Fields { get; }

            public Object(IReadOnlyList<InterfaceField> fields)
            {
                this.Fields = fields;
            }
        }
    }
}
