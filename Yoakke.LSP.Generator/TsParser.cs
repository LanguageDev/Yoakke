using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Lexer;
using Yoakke.Parser.Attributes;

namespace Yoakke.LSP.Generator
{
    [Parser(typeof(TokenType))]
    partial class TsParser
    {
        [Rule("definition : namespace | interface")]
        private static DefBase Definition(DefBase db) => db;

        [Rule("namespace : DocComment? 'export'? 'namespace' Ident '{' n_field* '}'")]
        private static NamespaceDef Namespace(
            IToken? doc,
            IToken _1, IToken _2,
            IToken name,
            IToken _3, IReadOnlyList<NamespaceField> fields, IToken _4) =>
            new NamespaceDef(name.Text, fields)
            {
                Docs = doc?.Text,
            };

        [Rule("namespace : DocComment? 'export'? 'type' Ident '=' StringLit ('|' StringLit)+ ';'")]
        private static NamespaceDef StringSumType(
            IToken? doc,
            IToken _1, IToken _2,
            IToken name, IToken _3,
            Token<TokenType> first, IReadOnlyList<(Token<TokenType>, Token<TokenType>)> rest, IToken _4) =>
            new NamespaceDef(name.Text, rest.Select(r => r.Item2).Prepend(first).Select(StringLitToNamespaceField).ToArray())
            {
                Docs = doc?.Text,
            };

        [Rule("n_field : DocComment? 'export'? 'const' Ident (':' (Ident | StringLit | NumLit))? '=' (StringLit | NumLit) ';'")]
        private static NamespaceField NField(
            IToken? doc,
            IToken? _1, IToken _2,
            IToken name, (IToken, IToken)? _3, IToken _4,
            Token<TokenType> value, IToken _5) =>
            new NamespaceField(name.Text, MakeNamespaceValue(value))
            {
                Docs = doc?.Text,
            };

        [Rule("interface : DocComment? 'export'? 'interface' Ident ('extends' Ident (',' Ident)*)? '{' i_field* '}'")]
        private static InterfaceDef Interface(
            IToken? doc,
            IToken _1, IToken _2, 
            IToken name, (IToken, IToken, IReadOnlyList<(Token<TokenType>, Token<TokenType>)>)? extend, 
            IToken _3, IReadOnlyList<InterfaceField> fields, IToken _4) =>
            new InterfaceDef(name.Text, extend?.Item3?.Select(t => t.Item2.Text).Prepend(extend.Value.Item2.Text).ToArray() ?? new string[] { }, fields)
            { 
                Docs = doc?.Text,
            };

        [Rule("i_field : DocComment? Ident '?'? ':' type ';'")]
        private static InterfaceField IField(IToken? doc, IToken name, IToken? opt, IToken _1, TypeNode type, IToken _2) =>
            new InterfaceField(name.Text, opt != null, type)
            {
                Docs = doc?.Text,
            };

        [Rule("type : type_postfix ('|' type_postfix)*")]
        private static TypeNode Or(TypeNode first, IReadOnlyList<(Token<TokenType>, TypeNode)> rest) => rest.Count == 0
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

        private static NamespaceField StringLitToNamespaceField(Token<TokenType> t)
        {
            if (t.Kind != TokenType.StringLit) throw new ArgumentException("string literal expected");
            var text = t.Text.Substring(1, t.Text.Length - 2);
            var capitalText = char.ToUpper(text[0]) + text.Substring(1);
            return new NamespaceField(capitalText, text);
        }

        private static object MakeNamespaceValue(Token<TokenType> t) => t.Kind == TokenType.NumLit
            ? int.Parse(t.Text)
            : t.Text.Substring(1, t.Text.Length - 2);
    }
}
