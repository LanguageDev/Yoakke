// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace System.Polyfill;

/// <summary>
/// Extensions for <see cref="string"/>.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Returns a value indicating whether a specified character occurs within this string.
    /// </summary>
    /// <param name="str">The <see cref="string"/>.</param>
    /// <param name="value">The character to seek.</param>
    /// <returns>True if the <paramref name="value"/> parameter occurs within this string; otherwise, false.</returns>
    public static bool Contains(this string str, char value)
    {
        for (var i = 0; i < str.Length; ++i)
        {
            if (str[i] == value) return true;
        }
        return false;
    }
}
