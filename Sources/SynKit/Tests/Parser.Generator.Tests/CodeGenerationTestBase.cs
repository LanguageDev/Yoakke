// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.Parser.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using Xunit;
using Yoakke.SynKit.Parser.Generator;

public abstract class CodeGenerationTestBase
{
    static CodeGenerationTestBase()
    {
        // To disable Visual Studio popping up on every test execution. Also make tests work on .NET 6
        Environment.SetEnvironmentVariable("DiffEngine_Disabled", "true");
        Environment.SetEnvironmentVariable("Verify_DisableClipboard", "true");
    }

    protected CodeGenerationTestBase() { }

    protected static Task VerifyCSharp(string source, NullableContextOptions nullableContextOptions)
    {
        return Verifier.Verify(GetCSharpGeneratedOutput(source, nullableContextOptions)).UseDirectory("verified");
    }

    protected static string GetCSharpGeneratedOutput(string source, NullableContextOptions nullableContextOptions)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = new List<MetadataReference>();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            if (!assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
            {
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }

        references.Add(MetadataReference.CreateFromFile(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute).Assembly.Location));

        var compilation = CSharpCompilation.Create(
            "foo",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: nullableContextOptions, usings: []));

        // var compileDiagnostics = compilation.GetDiagnostics();
        // Assert.IsFalse(compileDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error), "Failed: " + compileDiagnostics.FirstOrDefault()?.GetMessage());
        var generator = new ParserSourceGenerator();

        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generateDiagnostics);
        Assert.False(generateDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error), "Failed: " + generateDiagnostics.FirstOrDefault()?.GetMessage());

        string output = outputCompilation.SyntaxTrees.Last().ToString();

        Console.WriteLine(output);

        return output;
    }
}
