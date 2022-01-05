// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace System.Polyfill;

/// <summary>
/// Extensions for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Determines whether the current <paramref name="type"/> can be assigned to a variable of the
    /// specified <paramref name="targetType"/>.
    /// </summary>
    /// <param name="type">The current <see cref="Type"/> to check if it's assignable to <paramref name="targetType"/>.</param>
    /// <param name="targetType">The type to compare with the current type.</param>
    /// <returns>True, if a value of type <paramref name="type"/> is assignable to <paramref name="targetType"/>.</returns>
    public static bool IsAssignableTo(this Type type, Type? targetType) =>
        targetType?.IsAssignableFrom(type) ?? false;
}
