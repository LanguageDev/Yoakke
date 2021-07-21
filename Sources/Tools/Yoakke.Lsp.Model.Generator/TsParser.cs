// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Lexer;
using Yoakke.Parser;
using Yoakke.Parser.Attributes;

namespace Yoakke.Lsp.Generator
{
    [Parser(typeof(TokenType))]
    internal partial class TsParser
    {
#pragma warning disable SA1117 // Parameters should be on same line or separate lines

        [Rule("definition : namespace | interface")]
        private static DefBase Definition(DefBase db) => db;

        [Rule("namespace : DocComment? 'export'? 'namespace' Ident '{' n_field* '}'")]
        private static NamespaceDef Namespace(
            IToken? doc,
            IToken _1, IToken _2,
            IToken name,
            IToken _3, IReadOnlyList<NamespaceField> fields, IToken _4) =>
            new(name.Text, fields)
            {
                Docs = doc?.Text,
            };

        [Rule("namespace : DocComment? 'export'? 'type' Ident '=' (StringLit ('|' StringLit)+) ';'")]
        private static NamespaceDef StringSumType(
            IToken? doc,
            IToken _1, IToken _2,
            IToken name, IToken _3,
            Punctuated<IToken<TokenType>, IToken<TokenType>> items,
            IToken _4) =>
            new(name.Text, items.Values.Select(StringLitToNamespaceField).ToArray())
            {
                Docs = doc?.Text,
            };

        [Rule("n_field : DocComment? 'export'? 'const' Ident (':' (Ident | StringLit | NumLit))? '=' (StringLit | NumLit) ';'")]
        private static NamespaceField NField(
            IToken? doc,
            IToken? _1, IToken _2,
            IToken name, (IToken, IToken)? _3, IToken _4,
            IToken<TokenType> value, IToken _5) =>
            new(name.Text, MakeNamespaceValue(value))
            {
                Docs = doc?.Text,
            };

        [Rule("interface : DocComment? 'export'? 'interface' Ident ('extends' (Ident (',' Ident)*))? '{' i_field* '}'")]
        private static InterfaceDef Interface(
            IToken? doc,
            IToken _1, IToken _2,
            IToken name, (IToken, Punctuated<IToken<TokenType>, IToken<TokenType>>)? extend,
            IToken _3, IReadOnlyList<InterfaceField> fields, IToken _4) =>
            new(name.Text, extend?.Item2?.Values?.Select(v => v.Text)?.ToArray() ?? Array.Empty<string>(), fields)
            {
                Docs = doc?.Text,
            };

        [Rule("i_field : DocComment? Ident '?'? ':' type ';'")]
        private static InterfaceField IField(IToken? doc, IToken name, IToken? opt, IToken _1, TypeNode type, IToken _2) =>
            new(name.Text, opt != null, type)
            {
                Docs = doc?.Text,
            };

        [Rule("type : type_postfix ('|' type_postfix)*")]
        private static TypeNode Or(TypeNode first, IReadOnlyList<(IToken<TokenType>, TypeNode)> rest) => rest.Count == 0
            ? first
            : new TypeNode.Or(rest.Select(v => v.Item2).Prepend(first).ToArray());

        [Rule("type_postfix : type_postfix '[' ']'")]
        private static TypeNode Ident(TypeNode n, IToken _1, IToken _2) => new TypeNode.Array(n);

        [Rule("type_postfix : type_atom")]
        private static TypeNode Id(TypeNode n) => n;

        [Rule("type_atom : Ident")]
        private static TypeNode TypeName(IToken t) => new TypeNode.Ident(t.Text);

        [Rule("type_atom : '(' type ')'")]
        private static TypeNode Grouping(IToken _1, TypeNode s, IToken _2) => s;

        [Rule("type_atom : '{' i_field* '}'")]
        private static TypeNode Object(IToken _1, IReadOnlyList<InterfaceField> fs, IToken _2) => new TypeNode.Object(fs);

#pragma warning restore SA1117 // Parameters should be on same line or separate lines

        private static NamespaceField StringLitToNamespaceField(IToken<TokenType> t)
        {
            if (t.Kind != TokenType.StringLit) throw new ArgumentException("string literal expected");
            var text = t.Text[1..^1];
            var capitalText = char.ToUpper(text[0]) + text[1..];
            return new NamespaceField(capitalText, text);
        }

        private static object MakeNamespaceValue(IToken<TokenType> t) => t.Kind == TokenType.NumLit
            ? int.Parse(t.Text)
            : t.Text[1..^1];
    }
}
