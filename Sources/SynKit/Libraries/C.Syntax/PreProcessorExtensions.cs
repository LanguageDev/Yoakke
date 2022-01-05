// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Yoakke.SynKit.C.Syntax.Macros;

namespace Yoakke.SynKit.C.Syntax;

/// <summary>
/// Extension functionality for <see cref="IPreProcessor"/>s.
/// </summary>
public static class PreProcessorExtensions
{
    /// <summary>
    /// Defines a new <see cref="IMacro"/> with a factory function in the given <see cref="IPreProcessor"/>.
    /// </summary>
    /// <param name="preProcessor">The <see cref="IPreProcessor"/> to define the macro in.</param>
    /// <param name="makeMacro">The factory function that constructs the <see cref="IMacro"/>.</param>
    /// <returns>The <see cref="IPreProcessor"/> to be able to chain calls.</returns>
    public static IPreProcessor Define(this IPreProcessor preProcessor, Func<IPreProcessor, IMacro> makeMacro)
    {
        preProcessor.Define(makeMacro(preProcessor));
        return preProcessor;
    }

    /// <summary>
    /// Defines a default-constructible <see cref="IMacro"/> in the given <see cref="IPreProcessor"/>.
    /// </summary>
    /// <typeparam name="TMacro">The type of the <see cref="IMacro"/> to define.</typeparam>
    /// <param name="preProcessor">The <see cref="IPreProcessor"/> to define the macro in.</param>
    /// <returns>The <see cref="IPreProcessor"/> to be able to chain calls.</returns>
    public static IPreProcessor Define<TMacro>(this IPreProcessor preProcessor)
        where TMacro : IMacro, new() => preProcessor.Define(_ => new TMacro());

    /// <summary>
    /// Defines the __COUNTER__ macro extension.
    /// </summary>
    /// <param name="preProcessor">The <see cref="IPreProcessor"/> to define the macro in.</param>
    /// <returns>The <see cref="IPreProcessor"/> to be able to chain calls.</returns>
    public static IPreProcessor DefineCounter(this IPreProcessor preProcessor) =>
        preProcessor.Define<CounterMacro>();
}
