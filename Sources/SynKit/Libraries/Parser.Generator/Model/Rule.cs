// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.SynKit.Parser.Generator.Ast;

namespace Yoakke.SynKit.Parser.Generator.Model;

/// <summary>
/// Represents a single BNF rule-definition.
/// </summary>
internal class Rule
{
    /// <summary>
    /// The name of the <see cref="Rule"/>. This is on the left-hand side of the rule-definition, before the colon.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The AST of the grammar to match.
    /// </summary>
    public BnfAst Ast { get; set; }

    /// <summary>
    /// True, if this <see cref="Rule"/> should be part of the public API.
    /// </summary>
    public bool PublicApi { get; }

    /// <summary>
    /// The visual name of this <see cref="Rule"/>.
    /// </summary>
    public string VisualName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rule"/> class.
    /// </summary>
    /// <param name="name">The name of the grammar rule.</param>
    /// <param name="ast">The AST of the rule.</param>
    /// <param name="publicApi">True, if the rule should be part of public API.</param>
    public Rule(string name, BnfAst ast, bool publicApi = true)
    {
        this.Name = name;
        this.Ast = ast;
        this.PublicApi = publicApi;
        this.VisualName = name;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{this.Name} : {this.Ast}";
}
