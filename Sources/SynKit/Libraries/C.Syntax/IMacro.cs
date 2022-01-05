// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.SynKit.C.Syntax;

/// <summary>
/// A pre-processor macro.
/// </summary>
public interface IMacro
{
    /// <summary>
    /// The name of this <see cref="IMacro"/>.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Retrieves the list of parameter names for this <see cref="IMacro"/>.
    /// If null is returned, it means it's a parameterless macro that has to be invoked without parenthesis.
    /// A parameterless function-like macro invoked with parenthesis should return an empty array.
    /// A variadic macro should have "..." as it's last parameter.
    /// </summary>
    public IReadOnlyList<string>? Parameters { get; }

    /// <summary>
    /// Calls the <see cref="IMacro"/> with the given arguments.
    /// </summary>
    /// <param name="preProcessor">The <see cref="IPreProcessor"/> that called the expansion.</param>
    /// <param name="arguments">The list of token sequences that are considered arguments for this <see cref="IMacro"/>.
    /// We need a list of lists because an argument can consist of multiple tokens - or even 0.</param>
    /// <returns>The <see cref="CToken"/>s that result from this macro expansion.</returns>
    public IEnumerable<CToken> Expand(IPreProcessor preProcessor, IReadOnlyList<IReadOnlyList<CToken>> arguments);
}
