// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Yoakke.SourceGenerator.Common.RoslynExtensions
{
    /// <summary>
    /// Extension functionalities for <see cref="ISymbol"/>s.
    /// </summary>
    public static class SymbolExtensions
    {
        /// <summary>
        /// Retrieves the type kind of a <see cref="ISymbol"/> that can be used in codegen.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to get the kind of.</param>
        /// <returns>The code-friendly name of the type kind.</returns>
        public static string GetTypeKindName(this ITypeSymbol symbol) => symbol.TypeKind switch
        {
            TypeKind.Class => symbol.IsRecord ? "record" : "class",
            TypeKind.Struct => "struct",
            TypeKind.Interface => "interface",
            _ => throw new NotSupportedException(),
        };

        /// <summary>
        /// Checks, if a <see cref="ISymbol"/> is a nested type.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check.</param>
        /// <returns>True, if <paramref name="symbol"/> is a nested type.</returns>
        public static bool IsNested(this ISymbol symbol) =>
            !SymbolEqualityComparer.Default.Equals(symbol.ContainingSymbol, symbol.ContainingNamespace);

        /// <summary>
        /// Checks, if a <see cref="ISymbol"/> implements a given interface.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check the interface for.</param>
        /// <param name="interfaceName">The fully qualitied interface name.</param>
        /// <returns>True, if <paramref name="symbol"/> implements the interface named <paramref name="interfaceName"/>.</returns>
        public static bool ImplementsInterface(this ITypeSymbol symbol, string interfaceName) =>
               symbol.Name == interfaceName
            || symbol.AllInterfaces.Any(i => i.Name == interfaceName);

        /// <summary>
        /// Checks, if a <see cref="ISymbol"/> implements a given interface.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check the interface for.</param>
        /// <param name="interfaceSymbol">The interface symbol to search for.</param>
        /// <returns>True, if <paramref name="symbol"/> implements <paramref name="interfaceSymbol"/>.</returns>
        public static bool ImplementsInterface(this ITypeSymbol symbol, INamedTypeSymbol interfaceSymbol) =>
               SymbolEqualityComparer.Default.Equals(symbol, interfaceSymbol)
            || symbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, interfaceSymbol));

        /// <summary>
        /// Checks, if a <see cref="ISymbol"/> implements a generic interface.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check the interface for.</param>
        /// <param name="interfaceName">The generic interfaces fully qualified name to search for.</param>
        /// <returns>True, if <paramref name="symbol"/> implements <paramref name="interfaceName"/>.</returns>
        public static bool ImplementsGenericInterface(this ITypeSymbol symbol, string interfaceName) =>
            ImplementsGenericInterface(symbol, interfaceName, out var _);

        /// <summary>
        /// Checks, if a <see cref="ISymbol"/> implements a generic interface.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check the interface for.</param>
        /// <param name="interfaceName">The generic interfaces fully qualified name to search for.</param>
        /// <param name="genericArgs">The passed in type-arguments for <paramref name="interfaceName"/> are written here,
        /// in case <paramref name="symbol"/> implements it.</param>
        /// <returns>True, if <paramref name="symbol"/> implements <paramref name="interfaceName"/>.</returns>
        public static bool ImplementsGenericInterface(
            this ITypeSymbol symbol,
            string interfaceName,
            [MaybeNullWhen(false)] out IReadOnlyList<ITypeSymbol>? genericArgs)
        {
            if (symbol.OriginalDefinition.Name == interfaceName)
            {
                genericArgs = ((INamedTypeSymbol)symbol).TypeArguments;
                return true;
            }
            var sub = symbol.AllInterfaces.FirstOrDefault(i => i.OriginalDefinition.Name == interfaceName);
            if (sub is not null)
            {
                genericArgs = sub.TypeArguments;
                return true;
            }
            genericArgs = null;
            return false;
        }

        /// <summary>
        /// Checks, if a <see cref="ISymbol"/> implements a generic interface.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check the interface for.</param>
        /// <param name="interfaceSymbol">The generic interface symbol to search for.</param>
        /// <returns>True, if <paramref name="symbol"/> implements <paramref name="interfaceSymbol"/>.</returns>
        public static bool ImplementsGenericInterface(this ITypeSymbol symbol, INamedTypeSymbol interfaceSymbol) =>
            ImplementsGenericInterface(symbol, interfaceSymbol, out var _);

        /// <summary>
        /// Checks, if a <see cref="ISymbol"/> implements a generic interface.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check the interface for.</param>
        /// <param name="interfaceSymbol">The generic interface symbol to search for.</param>
        /// <param name="genericArgs">The passed in type-arguments for <paramref name="interfaceSymbol"/> are written here,
        /// in case <paramref name="symbol"/> implements it.</param>
        /// <returns>True, if <paramref name="symbol"/> implements <paramref name="interfaceSymbol"/>.</returns>
        public static bool ImplementsGenericInterface(
            this ITypeSymbol symbol,
            INamedTypeSymbol interfaceSymbol,
            [MaybeNullWhen(false)] out IReadOnlyList<ITypeSymbol>? genericArgs)
        {
            if (SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, interfaceSymbol))
            {
                genericArgs = ((INamedTypeSymbol)symbol).TypeArguments;
                return true;
            }
            var sub = symbol.AllInterfaces.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, interfaceSymbol));
            if (sub is not null)
            {
                genericArgs = sub.TypeArguments;
                return true;
            }
            genericArgs = null;
            return false;
        }

        /// <summary>
        /// Determines if the given property has a backing field or not.
        /// </summary>
        /// <param name="symbol">The <see cref="IPropertySymbol"/> to check.</param>
        /// <returns>True, if <paramref name="symbol"/> has a backing field.</returns>
        public static bool HasBackingField(this IPropertySymbol symbol) => symbol.ContainingType
            .GetMembers()
            .Any(m => m is IFieldSymbol f && SymbolEqualityComparer.Default.Equals(f.AssociatedSymbol, symbol));

        /// <summary>
        /// Checks if a <see cref="ISymbol"/> has a given attribute.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check.</param>
        /// <param name="attributeName">The fully qualified name of the attribute to search for.</param>
        /// <returns>True, if <paramref name="symbol"/> has an attribut <paramref name="attributeName"/>.</returns>
        public static bool HasAttribute(this ISymbol symbol, string attributeName) =>
            symbol.TryGetAttribute(attributeName, out var _);

        /// <summary>
        /// Checks if a <see cref="ISymbol"/> has a given attribute.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check.</param>
        /// <param name="attributeSymbol">The attribute symbol to search for.</param>
        /// <returns>True, if <paramref name="symbol"/> has an attribut <paramref name="attributeSymbol"/>.</returns>
        public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attributeSymbol) =>
            symbol.TryGetAttribute(attributeSymbol, out var _);

        /// <summary>
        /// Tries to retrieve a given <see cref="AttributeData"/> attached to a <see cref="ISymbol"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to search in.</param>
        /// <param name="attributeName">The fully qualified name of the attribute to search for.</param>
        /// <param name="attributeData">The <see cref="AttributeData"/> gets written here, if it's found.</param>
        /// <returns>True, if the attribute is found with name <paramref name="attributeName"/>.</returns>
        public static bool TryGetAttribute(
            this ISymbol symbol,
            string attributeName,
            [MaybeNullWhen(false)] out AttributeData attributeData)
        {
            attributeData = symbol
                .GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.Name == attributeName);
            return attributeData is not null;
        }

        /// <summary>
        /// Tries to retrieve a given <see cref="AttributeData"/> attached to a <see cref="ISymbol"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to search in.</param>
        /// <param name="attributeSymbol">The attribute symbol to search for.</param>
        /// <param name="attributeData">The <see cref="AttributeData"/> gets written here, if it's found.</param>
        /// <returns>True, if the attribute <paramref name="attributeSymbol"/> is found.</returns>
        public static bool TryGetAttribute(
            this ISymbol symbol,
            INamedTypeSymbol attributeSymbol,
            [MaybeNullWhen(false)] out AttributeData attributeData)
        {
            attributeData = symbol
                .GetAttributes()
                .FirstOrDefault(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attributeSymbol));
            return attributeData is not null;
        }
    }
}
