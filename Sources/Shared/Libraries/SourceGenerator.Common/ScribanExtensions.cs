// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Scriban;
using Scriban.Runtime;

namespace Yoakke.SourceGenerator.Common;

/// <summary>
/// Extensions to Scriban.
/// </summary>
public static class ScribanExtensions
{
    /// <summary>
    /// Renders the template with a given model.
    /// </summary>
    /// <param name="template">The template to render.</param>
    /// <param name="model">The model object for the template.</param>
    /// <param name="format">True, if the result should be formatted as C# code.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>The rendered text.</returns>
    public static string Render(
        this Template template,
        object? model,
        bool format,
        CancellationToken cancellationToken)
    {
        var context = new TemplateContext
        {
            CancellationToken = cancellationToken,
            MemberRenamer = member => member.Name,
        };
        var scriptObj = new ScriptObject();
        scriptObj.Import(model, renamer: member => member.Name);
        context.PushGlobal(scriptObj);
        var result = template.Render(context);
        if (format)
        {
            result = SyntaxFactory
                .ParseCompilationUnit(result)
                .NormalizeWhitespace()
                .GetText()
                .ToString();
        }
        return result;
    }
}
