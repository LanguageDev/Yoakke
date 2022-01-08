// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace System.Collections.Generic.Polyfill;

/// <summary>
/// Extensions for <see cref="KeyValuePair{TKey, TValue}"/>.
/// </summary>
public static class KeyValuePairExtensions
{
    /// <summary>
    /// Deconstructs the current <see cref="KeyValuePair{TKey, TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="keyValuePair">The <see cref="KeyValuePair{TKey, TValue}"/> to deconstruct.</param>
    /// <param name="key">The key of the current <see cref="KeyValuePair{TKey, TValue}"/>.</param>
    /// <param name="value">The value of the current <see cref="KeyValuePair{TKey, TValue}"/>.</param>
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair, out TKey key, out TValue value)
    {
        key = keyValuePair.Key;
        value = keyValuePair.Value;
    }
}
