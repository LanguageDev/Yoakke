// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Yoakke.SourceGenerator.Common.RoslynExtensions;

/// <summary>
/// A class that describes how an attribute should look.
/// This can be used for parsing.
/// </summary>
public class AttributeDescriptor
{
    private readonly List<(string Name, object? Default)> parameters = new();

    /// <summary>
    /// Adds a parameter definition to this descriptor.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="defaultValue">The default value of the parameter.</param>
    /// <returns>This instance to chain calls.</returns>
    public AttributeDescriptor WithParameter(string name, object? defaultValue = null)
    {
        this.parameters.Add((name, defaultValue));
        return this;
    }

    /// <summary>
    /// Parses an <see cref="AttributeData"/> based on this description.
    /// </summary>
    /// <param name="attributeData">The <see cref="AttributeData"/> to parse.</param>
    /// <returns>The associated parameter names and values.</returns>
    public IReadOnlyDictionary<string, object?> Parse(AttributeData attributeData)
    {
        var result = new Dictionary<string, object?>();
        var uncovered = this.parameters.Select(p => p.Name).ToList();

        // First deal with positional
        for (var i = 0; i < attributeData.ConstructorArguments.Length; ++i)
        {
            var name = this.parameters[i].Name;
            var value = attributeData.ConstructorArguments[i];
            result[this.parameters[i].Name] = value.Value;
            uncovered.Remove(name);
        }

        // Then deal with named
        foreach (var kv in attributeData.NamedArguments)
        {
            result[kv.Key] = kv.Value.Value;
            uncovered.Remove(kv.Key);
        }

        // Assign defaults
        foreach (var name in uncovered)
        {
            var param = this.parameters.FirstOrDefault(p => p.Name == name);
            result[name] = param.Default;
        }

        return result;
    }
}
