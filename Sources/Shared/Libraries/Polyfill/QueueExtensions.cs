// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System.Collections.Generic.Polyfill;

/// <summary>
/// Extensions for <see cref="Queue{T}"/>.
/// </summary>
public static class QueueExtensions
{
    /// <summary>
    /// Removes the object at the beginning of the <see cref="Queue{T}"/>, and copies it to the
    /// <paramref name="result"/> parameter.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
    /// <param name="queue">The queue to dequeue from.</param>
    /// <param name="result">The removed object.</param>
    /// <returns>True if the object is successfully removed, false if the <see cref="Queue{T}"/> is empty.</returns>
    public static bool TryDequeue<T>(this Queue<T> queue, [MaybeNullWhen(false)] out T result)
    {
        if (queue.Count > 0)
        {
            result = queue.Dequeue();
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }
}
