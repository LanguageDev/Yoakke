// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic.Polyfill;

/// <summary>
/// Extensions for <see cref="Dictionary{TKey, TValue}"/>.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Removes the value with the specified key from the <see cref="Dictionary{TKey, TValue}"/>, and copies
    /// the element to the <paramref name="value"/> parameter.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="dictionary">The <see cref="Dictionary{TKey, TValue}"/>.</param>
    /// <param name="key">The key of the element to remove.</param>
    /// <param name="value">The removed element.</param>
    /// <returns>True if the element is successfully found and removed; otherwise, false.</returns>
    public static bool Remove<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        [MaybeNullWhen(false)] out TValue value)
    {
        if (dictionary.TryGetValue(key, out value))
        {
            dictionary.Remove(key);
            return true;
        }
        else
        {
            return false;
        }
    }
}
