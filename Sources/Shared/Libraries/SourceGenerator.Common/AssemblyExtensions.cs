// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Yoakke.SourceGenerator.Common;

/// <summary>
/// Extensions for .NET Assemblies.
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// Reads an embedded resource text from an assembly.
    /// </summary>
    /// <param name="assembly">The assembly to read from.</param>
    /// <param name="name">The name of the embedded resource text.</param>
    /// <returns>The resource text read from the assembly.</returns>
    public static string ReadManifestResourceText(this Assembly assembly, string name)
    {
        using var stream = assembly.GetManifestResourceStream(name);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
