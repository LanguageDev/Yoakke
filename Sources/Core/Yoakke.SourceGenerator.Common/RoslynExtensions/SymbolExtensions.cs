// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        /// Checks, if a <see cref="ITypeSymbol"/>s declaration is partial.
        /// </summary>
        /// <param name="symbol">The <see cref="ITypeSymbol"/> to check.</param>
        /// <returns>True, if <paramref name="symbol"/> is declared partial.</returns>
        public static bool IsPartial(this ITypeSymbol symbol) => symbol.DeclaringSyntaxReferences
            .Any(syntaxRef => syntaxRef.GetSyntax() is TypeDeclarationSyntax typeDecl && typeDecl.IsPartial());

        /// <summary>
        /// Checks, if a <see cref="ISymbol"/> is a nested type.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check.</param>
        /// <returns>True, if <paramref name="symbol"/> is a nested type.</returns>
        public static bool IsNested(this ISymbol symbol) => symbol.ContainingSymbol is ITypeSymbol;

        /// <summary>
        /// Checks, if an <see cref="ISymbol"/> can accept declarations from other source files.
        /// This usually means that the symbol is a namespace or is in partial type definitions.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check.</param>
        /// <returns>True, if <paramref name="symbol"/> accepts declarations inside it from other sources.</returns>
        public static bool CanDeclareInsideExternally(this ISymbol symbol)
        {
            if (symbol is null || symbol is INamespaceSymbol) return true;
            if (symbol is ITypeSymbol type) return type.IsPartial() && (type.ContainingSymbol?.CanDeclareInsideExternally() ?? true);
            return false;
        }

        /// <summary>
        /// Builds up code that is required to define something inside an <see cref="ISymbol"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to build up the code for.</param>
        /// <returns>A pair of prefix and suffix text, that is enough to write declarations inside <paramref name="symbol"/>.</returns>
        public static (string Prefix, string Suffix) DeclareInsideExternally(this ISymbol symbol)
        {
            var prefixBuilder = new StringBuilder();
            var suffixBuilder = new StringBuilder();

            void DeclareInsideExternallyRec(ISymbol symbol)
            {
                if (symbol is null) return;
                if (symbol is ITypeSymbol type)
                {
                    Debug.Assert(type.IsPartial(), "Type declaration must be partial");
                    DeclareInsideExternallyRec(symbol.ContainingSymbol);
                    prefixBuilder!.AppendLine($"partial {type.GetTypeKindName()} {{");
                    suffixBuilder!.AppendLine("}");
                    return;
                }
                if (symbol is INamespaceSymbol ns)
                {
                    prefixBuilder!.AppendLine($"namespace {ns.ToDisplayString()} {{");
                    suffixBuilder!.AppendLine("}");
                    return;
                }
                throw new InvalidOperationException();
            }

            DeclareInsideExternallyRec(symbol);
            return (prefixBuilder.ToString(), suffixBuilder.ToString());
        }

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
        /// <param name="interfaceSymbol">The generic interface symbol to search for.</param>
        /// <returns>True, if <paramref name="symbol"/> implements <paramref name="interfaceSymbol"/>.</returns>
        public static bool ImplementsGenericInterface(this ITypeSymbol symbol, INamedTypeSymbol interfaceSymbol) =>
            ImplementsGenericInterface(symbol, interfaceSymbol, out _);

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
        /// Retrieves all <see cref="AttributeData"/> attached to a <see cref="ISymbol"/> of the same attribute type.
        /// </summary>
        /// <typeparam name="T">The element type of the structure to parse into.</typeparam>
        /// <param name="symbol">The <see cref="ISymbol"/> to search in.</param>
        /// <param name="attributeSymbol">The symbol of the attribute to search for.</param>
        /// <returns>The found <see cref="AttributeData"/> list parsed.</returns>
        public static IReadOnlyList<T> GetAttributes<T>(this ISymbol symbol, INamedTypeSymbol attributeSymbol)
            where T : new() => symbol
            .GetAttributes(attributeSymbol)
            .Select(attr => attr.ParseInto<T>())
            .ToList();

        /// <summary>
        /// Retrieves all <see cref="AttributeData"/> attached to a <see cref="ISymbol"/> of the same attribute type.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to search in.</param>
        /// <param name="attributeSymbol">The symbol of the attribute to search for.</param>
        /// <returns>The found <see cref="AttributeData"/> list.</returns>
        public static IReadOnlyList<AttributeData> GetAttributes(this ISymbol symbol, INamedTypeSymbol attributeSymbol) => symbol
            .GetAttributes()
            .Where(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attributeSymbol))
            .ToList();

        /// <summary>
        /// Checks if a <see cref="ISymbol"/> has a given attribute.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check.</param>
        /// <param name="attributeSymbol">The attribute symbol to search for.</param>
        /// <returns>True, if <paramref name="symbol"/> has an attribut <paramref name="attributeSymbol"/>.</returns>
        public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attributeSymbol) =>
            symbol.TryGetAttribute(attributeSymbol, out _);

        /// <summary>
        /// Retrieves a given <see cref="AttributeData"/> attached to a <see cref="ISymbol"/>.
        /// </summary>
        /// <typeparam name="T">The type of the structure to parse into.</typeparam>
        /// <param name="symbol">The <see cref="ISymbol"/> to search in.</param>
        /// <param name="attributeSymbol">The symbol of the attribute to search for.</param>
        /// <returns>The found and parsed <see cref="AttributeData"/>.</returns>
        public static T GetAttribute<T>(this ISymbol symbol, INamedTypeSymbol attributeSymbol)
            where T : new()
        {
            var attrData = symbol.GetAttribute(attributeSymbol);
            return attrData.ParseInto<T>();
        }

        /// <summary>
        /// Retrieves a given <see cref="AttributeData"/> attached to a <see cref="ISymbol"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to search in.</param>
        /// <param name="attributeSymbol">The symbol of the attribute to search for.</param>
        /// <returns>The found <see cref="AttributeData"/>.</returns>
        public static AttributeData GetAttribute(this ISymbol symbol, INamedTypeSymbol attributeSymbol)
        {
            if (!symbol.TryGetAttribute(attributeSymbol, out var result)) throw new InvalidOperationException();
            return result;
        }

        /// <summary>
        /// Tries to retrieve a given <see cref="AttributeData"/> attached to a <see cref="ISymbol"/>.
        /// </summary>
        /// <typeparam name="T">The type of the structure to parse into.</typeparam>
        /// <param name="symbol">The <see cref="ISymbol"/> to search in.</param>
        /// <param name="attributeSymbol">The attribute symbol to search for.</param>
        /// <param name="result">The parsed structure gets written here, an attribute is found.</param>
        /// <returns>True, if the attribute <paramref name="attributeSymbol"/> is found.</returns>
        public static bool TryGetAttribute<T>(
            this ISymbol symbol,
            INamedTypeSymbol attributeSymbol,
            [MaybeNullWhen(false)] out T result)
            where T : new()
        {
            if (symbol.TryGetAttribute(attributeSymbol, out var attributeData))
            {
                result = attributeData.ParseInto<T>();
                return true;
            }
            else
            {
                result = default;
                return false;
            }
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
