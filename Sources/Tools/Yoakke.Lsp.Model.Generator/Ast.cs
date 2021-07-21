// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.Lsp.Model.Generator
{
    /// <summary>
    /// Base class for all defititions.
    /// </summary>
#pragma warning disable SA1649 // File name should match first type name
    internal abstract class DefBase
#pragma warning restore SA1649 // File name should match first type name
    {
    }

    /// <summary>
    /// Interface definition node.
    /// </summary>
    internal class InterfaceDef : DefBase
    {
        /// <summary>
        /// The name of the interface.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The base interfaces of this one.
        /// </summary>
        public IReadOnlyList<string> Bases { get; }

        /// <summary>
        /// The defined fields.
        /// </summary>
        public IReadOnlyList<InterfaceField> Fields { get; }

        /// <summary>
        /// Documentation comment attached, if any.
        /// </summary>
        public string? Docs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterfaceDef"/> class.
        /// </summary>
        /// <param name="name">The name of the interface.</param>
        /// <param name="bases">The base interfaces of this one.</param>
        /// <param name="fields">The defined fields.</param>
        public InterfaceDef(string name, IReadOnlyList<string> bases, IReadOnlyList<InterfaceField> fields)
        {
            this.Name = name;
            this.Bases = bases;
            this.Fields = fields;
        }
    }

    /// <summary>
    /// Namespace definition node.
    /// </summary>
    internal class NamespaceDef : DefBase
    {
        /// <summary>
        /// The name of the namespace.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The fields of the namespace.
        /// </summary>
        public IReadOnlyList<NamespaceField> Fields { get; }

        /// <summary>
        /// Documentation comment attached, if any.
        /// </summary>
        public string? Docs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceDef"/> class.
        /// </summary>
        /// <param name="name">The name of the namespace.</param>
        /// <param name="fields">The fields of the namespace.</param>
        public NamespaceDef(string name, IReadOnlyList<NamespaceField> fields)
        {
            this.Name = name;
            this.Fields = fields;
        }
    }

    /// <summary>
    /// An interface field.
    /// </summary>
    internal class InterfaceField
    {
        /// <summary>
        /// The name of the field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// True, if the field is optional.
        /// </summary>
        public bool Optional { get; }

        /// <summary>
        /// The type of the field.
        /// </summary>
        public TypeNode Type { get; }

        /// <summary>
        /// Documentation comment attached, if any.
        /// </summary>
        public string? Docs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterfaceField"/> class.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="optional">True, if the field is optional.</param>
        /// <param name="type">The type of the field.</param>
        public InterfaceField(string name, bool optional, TypeNode type)
        {
            this.Name = name;
            this.Optional = optional;
            this.Type = type;
        }
    }

    /// <summary>
    /// A namespace field.
    /// </summary>
    internal class NamespaceField
    {
        /// <summary>
        /// The name of the field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The value of the field.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Documentation comment attached, if any.
        /// </summary>
        public string? Docs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceField"/> class.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        public NamespaceField(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    /// <summary>
    /// Base class for type nodes.
    /// </summary>
    internal abstract class TypeNode
    {
        /// <summary>
        /// An identifier type.
        /// </summary>
        public class Ident : TypeNode
        {
            /// <summary>
            /// The identifier value.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Ident"/> class.
            /// </summary>
            /// <param name="name">The identifier.</param>
            public Ident(string name)
            {
                this.Name = name;
            }
        }

        /// <summary>
        /// An array type.
        /// </summary>
        public class Array : TypeNode
        {
            /// <summary>
            /// The element type of the array.
            /// </summary>
            public TypeNode ElementType { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Array"/> class.
            /// </summary>
            /// <param name="elementType">The element type of the array.</param>
            public Array(TypeNode elementType)
            {
                this.ElementType = elementType;
            }
        }

        /// <summary>
        /// A sum-type.
        /// </summary>
        public class Or : TypeNode
        {
            /// <summary>
            /// The different alternative types.
            /// </summary>
            public IReadOnlyList<TypeNode> Alternatives { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Or"/> class.
            /// </summary>
            /// <param name="alternatives">The list of different alternative types.</param>
            public Or(IReadOnlyList<TypeNode> alternatives)
            {
                this.Alternatives = alternatives;
            }
        }

        /// <summary>
        /// An anonymous object type.
        /// </summary>
        public class Object : TypeNode
        {
            /// <summary>
            /// The fields in this anonymous object.
            /// </summary>
            public IReadOnlyList<InterfaceField> Fields { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Object"/> class.
            /// </summary>
            /// <param name="fields">The fields in this anonymous object.</param>
            public Object(IReadOnlyList<InterfaceField> fields)
            {
                this.Fields = fields;
            }
        }
    }
}
