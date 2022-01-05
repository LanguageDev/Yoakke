// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.SynKit.C.Syntax;

/// <summary>
/// The interface of how all C pre-processors should work.
/// </summary>
public interface IPreProcessor
{
    /// <summary>
    /// The <see cref="IMacro"/>s defined in this <see cref="IPreProcessor"/>.
    /// </summary>
    public IReadOnlyDictionary<string, IMacro> Macros { get; }

    /// <summary>
    /// Defines the given <see cref="IMacro"/> for usage.
    /// </summary>
    /// <param name="macro">The <see cref="IMacro"/> to define.</param>
    /// <returns>The <see cref="IMacro"/> the newly added overwrote because of name collision,
    /// or null if it overrode nothing.</returns>
    public IMacro? Define(IMacro macro);

    /// <summary>
    /// Undefines an <see cref="IMacro"/> by name.
    /// </summary>
    /// <param name="name">The name of the <see cref="IMacro"/> to undefine.</param>
    /// <returns>The undefined <see cref="IMacro"/>, or null if there was none defined with this name.</returns>
    public IMacro? Undefine(string name);

    /// <summary>
    /// Checks if an <see cref="IMacro"/> is defined with the given name.
    /// </summary>
    /// <param name="name">The name of the <see cref="IMacro"/> to search for.</param>
    /// <returns>True, if an <see cref="IMacro"/> with name <paramref name="name"/> is defined.</returns>
    public bool IsDefined(string name);

    /// <summary>
    /// Retrieves the next <see cref="CToken"/> from the pre-processor.
    /// </summary>
    /// <returns>The next upcoming <see cref="CToken"/>.</returns>
    public CToken Next();

    /// <summary>
    /// Expands the given sequence of <see cref="CToken"/>s a single time.
    /// </summary>
    /// <param name="tokens">The sequence of <see cref="CToken"/>s to expand.</param>
    /// <returns>The expanded sequence of <see cref="CToken"/>s.</returns>
    public IEnumerable<CToken> Expand(IEnumerable<CToken> tokens);
}
