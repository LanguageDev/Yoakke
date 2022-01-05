// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.IO;

namespace Yoakke.SynKit.Text;

/// <summary>
/// Represents a source file.
/// </summary>
public interface ISourceFile
{
    /// <summary>
    /// Retrieves a path that uniquely identifies this <see cref="ISourceFile"/>.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Retrieves the currently available amount of lines in the <see cref="ISourceFile"/>.
    /// This means the file might has more, but it has not been read yet.
    /// </summary>
    public int AvailableLines { get; }

    /// <summary>
    /// The <see cref="TextReader"/> for the contents of the <see cref="ISourceFile"/>.
    /// </summary>
    public TextReader Reader { get; }

    /// <summary>
    /// Retrieves a given line from the <see cref="ISourceFile"/>.
    /// </summary>
    /// <param name="index">The index of the line to retrieve.</param>
    /// <returns>The line with the given index.</returns>
    public string GetLine(int index);
}
