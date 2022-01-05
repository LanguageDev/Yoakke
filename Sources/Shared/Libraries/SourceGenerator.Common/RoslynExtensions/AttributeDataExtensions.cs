// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Yoakke.SourceGenerator.Common.RoslynExtensions;

/// <summary>
/// Extension functionality for <see cref="AttributeData"/>.
/// </summary>
public static class AttributeDataExtensions
{
    /// <summary>
    /// Parses an <see cref="AttributeData"/> into a descriptive class instance.
    /// </summary>
    /// <typeparam name="T">The instance to parse into.</typeparam>
    /// <param name="data">The <see cref="AttributeData"/> to parse.</param>
    /// <returns>The parsed object.</returns>
    public static T ParseInto<T>(this AttributeData data)
        where T : new()
    {
        var result = new T();

        // Build the descriptor
        var desc = new AttributeDescriptor();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props) desc.WithParameter(prop.Name, prop.GetValue(result));

        // Parse
        var values = data.Parse(desc);
        foreach (var prop in props) prop.SetValue(result, values[prop.Name]);

        return result;
    }

    /// <summary>
    /// Parses the <see cref="AttributeData"/> based on an <see cref="AttributeDescriptor"/>.
    /// </summary>
    /// <param name="data">The <see cref="AttributeData"/> to parse.</param>
    /// <param name="descriptor">The <see cref="AttributeDescriptor"/> to parse based on.</param>
    /// <returns>The associated parameter names and values.</returns>
    public static IReadOnlyDictionary<string, object?> Parse(this AttributeData data, AttributeDescriptor descriptor) =>
        descriptor.Parse(data);

    /// <summary>
    /// Retrieves a given positional attribute constructor argument.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <param name="data">The <see cref="AttributeData"/> to search in.</param>
    /// <param name="idx">The index of the constructor argument.</param>
    /// <returns>The constructor argument at the position.</returns>
    public static T GetCtorValue<T>(this AttributeData data, int idx = 0)
    {
        if (!data.TryGetCtorValue(idx, out T? result)) throw new InvalidOperationException();
        return result!;
    }

    /// <summary>
    /// Retrieves a given named attribute constructor argument.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <param name="data">The <see cref="AttributeData"/> to search in.</param>
    /// <param name="name">The name of the constructor argument.</param>
    /// <returns>The constructor argument with the name.</returns>
    public static T GetCtorValue<T>(this AttributeData data, string name)
    {
        if (!data.TryGetCtorValue(name, out T? result)) throw new InvalidOperationException();
        return result!;
    }

    /// <summary>
    /// Retrieves a given positional attribute constructor argument.
    /// </summary>
    /// <param name="data">The <see cref="AttributeData"/> to search in.</param>
    /// <param name="idx">The index of the constructor argument.</param>
    /// <returns>The constructor argument at the position.</returns>
    public static object? GetCtorValue(this AttributeData data, int idx = 0)
    {
        if (!data.TryGetCtorValue(idx, out var result)) throw new InvalidOperationException();
        return result;
    }

    /// <summary>
    /// Retrieves a given named attribute constructor argument.
    /// </summary>
    /// <param name="data">The <see cref="AttributeData"/> to search in.</param>
    /// <param name="name">The name of the constructor argument.</param>
    /// <returns>The constructor argument with the name.</returns>
    public static object? GetCtorValue(this AttributeData data, string name)
    {
        if (!data.TryGetCtorValue(name, out var result)) throw new InvalidOperationException();
        return result;
    }

    /// <summary>
    /// Tries to retrieve a given positional attribute constructor argument.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <param name="data">The <see cref="AttributeData"/> to search in.</param>
    /// <param name="idx">The index of the constructor argument.</param>
    /// <param name="value">The value gets written here, if a constructor argument is present at the position.</param>
    /// <returns>True, if a constructor argument is present at the position.</returns>
    public static bool TryGetCtorValue<T>(this AttributeData data, int idx, [MaybeNullWhen(false)] out T value)
    {
        if (data.TryGetCtorValue(idx, out var objectValue))
        {
            value = (T)objectValue!;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    /// <summary>
    /// Tries to retrieve a given named attribute constructor argument.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <param name="data">The <see cref="AttributeData"/> to search in.</param>
    /// <param name="name">The name of the constructor argument.</param>
    /// <param name="value">The value gets written here, if a constructor argument is present with the name.</param>
    /// <returns>True, if a constructor argument is present with the name.</returns>
    public static bool TryGetCtorValue<T>(this AttributeData data, string name, [MaybeNullWhen(false)] out T value)
    {
        if (data.TryGetCtorValue(name, out var objectValue))
        {
            value = (T)objectValue!;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    /// <summary>
    /// Tries to retrieve a given positional attribute constructor argument.
    /// </summary>
    /// <param name="data">The <see cref="AttributeData"/> to search in.</param>
    /// <param name="idx">The index of the constructor argument.</param>
    /// <param name="value">The value gets written here, if a constructor argument is present at the position.</param>
    /// <returns>True, if a constructor argument is present at the position.</returns>
    public static bool TryGetCtorValue(this AttributeData data, int idx, out object? value)
    {
        if (data.ConstructorArguments.Length <= idx)
        {
            value = null;
            return false;
        }
        else
        {
            value = data.ConstructorArguments[idx].Value;
            return true;
        }
    }

    /// <summary>
    /// Tries to retrieve a given named attribute constructor argument.
    /// </summary>
    /// <param name="data">The <see cref="AttributeData"/> to search in.</param>
    /// <param name="name">The name of the constructor argument.</param>
    /// <param name="value">The value gets written here, if a constructor argument is present with the name.</param>
    /// <returns>True, if a constructor argument is present with the name.</returns>
    public static bool TryGetCtorValue(this AttributeData data, string name, out object? value)
    {
        foreach (var arg in data.NamedArguments)
        {
            if (arg.Key == name)
            {
                value = arg.Value;
                return true;
            }
        }
        value = null;
        return false;
    }
}
