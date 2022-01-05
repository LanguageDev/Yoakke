// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic.Polyfill;

/// <summary>
/// Extensions for <see cref="Stack{T}"/>.
/// </summary>
public static class StackExtensions
{
    /// <summary>
    /// Returns a value that indicates whether there is an object at the top of the <see cref="Stack{T}"/>,
    /// and if one is present, copies it to the result parameter, and removes it from the <see cref="Stack{T}"/>.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the stack.</typeparam>
    /// <param name="stack">The <see cref="Stack{T}"/>.</param>
    /// <param name="result">If present, the object at the top of the <see cref="Stack{T}"/>; otherwise, the default
    /// value of <typeparamref name="T"/>.</param>
    /// <returns>True if there is an object at the top of the <see cref="Stack{T}"/>; false if the
    /// <see cref="Stack{T}"/> is empty.</returns>
    public static bool TryPop<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T result)
    {
        if (stack.Count > 0)
        {
            result = stack.Pop();
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Returns a value that indicates whether there is an object at the top of the <see cref="Stack{T}"/>,
    /// and if one is present, copies it to the <paramref name="result"/> parameter.
    /// The object is not removed from the <see cref="Stack{T}"/>.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the stack.</typeparam>
    /// <param name="stack">The <see cref="Stack{T}"/>.</param>
    /// <param name="result">If present, the object at the top of the <see cref="Stack{T}"/>;
    /// otherwise, the default value of <typeparamref name="T"/>.</param>
    /// <returns>True if there is an object at the top of the <see cref="Stack{T}"/>;
    /// false if the <see cref="Stack{T}"/> is empty.</returns>
    public static bool TryPeek<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T result)
    {
        if (stack.Count > 0)
        {
            result = stack.Peek();
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }
}
