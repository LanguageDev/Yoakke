using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Yoakke.Parser.Generator.Ast;

namespace Yoakke.Parser.Generator.Syntax
{
    /// <summary>
    /// The parser for BNF grammar, that creates the <see cref="BnfAst"/>.
    /// </summary>
    internal class BnfParser
    {
        public static (string Name, BnfAst? Ast) Parse(string source, TokenKindSet tokenKinds) =>
            Parse(BnfLexer.Lex(source), tokenKinds);

        public static (string Name, BnfAst? Ast) Parse(IList<BnfToken> tokens, TokenKindSet tokenKinds) =>
            new BnfParser(tokens, tokenKinds).ParseRule();

        private readonly IList<BnfToken> tokens;
        private readonly TokenKindSet tokenKinds;
        private int index;

        public BnfParser(IList<BnfToken> tokens, TokenKindSet tokenKinds)
        {
            this.tokens = tokens;
            this.tokenKinds = tokenKinds;
        }

        public (string Name, BnfAst? Ast) ParseRule()
        {
            var name = this.Expect(BnfTokenType.Identifier);
            BnfAst? ast = null;
            if (this.TryMatch(BnfTokenType.Colon)) ast = this.ParseAlt();
            this.Expect(BnfTokenType.End);
            return (name.Value, ast);
        }

        private BnfAst ParseAlt()
        {
            var first = this.ParseSeq();
            if (this.TryMatch(BnfTokenType.Pipe)) return new BnfAst.Alt(first, this.ParseAlt());
            return first;
        }

        private BnfAst ParseSeq()
        {
            var first = this.ParsePostfix();
            var ptype = this.Peek().Type;
            if (ptype != BnfTokenType.End && ptype != BnfTokenType.CloseParen && ptype != BnfTokenType.Pipe)
            {
                return new BnfAst.Seq(first, this.ParseSeq());
            }
            return first;
        }

        private BnfAst ParsePostfix()
        {
            var atom = this.ParseAtom();
            if (this.TryMatch(BnfTokenType.QuestionMark)) return new BnfAst.Opt(atom);
            if (this.TryMatch(BnfTokenType.Star)) return new BnfAst.Rep0(atom);
            if (this.TryMatch(BnfTokenType.Plus)) return new BnfAst.Rep1(atom);
            return atom;
        }

        private BnfAst ParseAtom()
        {
            if (this.TryMatch(BnfTokenType.OpenParen))
            {
                var sub = this.ParseAlt();
                this.Expect(BnfTokenType.CloseParen);
                return new BnfAst.Group(sub);
            }
            if (this.TryMatch(BnfTokenType.Identifier, out var ident))
            {
                if (this.tokenKinds.TryGetVariant(ident!.Value, out var kind))
                {
                    // It's a literal match
                    return new BnfAst.Literal(kind);
                }
                else
                {
                    // It's a rule call
                    return new BnfAst.Call(ident.Value);
                }
            }
            if (this.TryMatch(BnfTokenType.StringLiteral, out var str)) return new BnfAst.Literal(this.StrToString(str!));

            throw new FormatException($"Unexpected token {this.Peek().Type} (index {this.Peek().Index})");
        }

        private string StrToString(BnfToken token) => token.Value.Substring(1, token.Value.Length - 2);

        private BnfToken Expect(BnfTokenType type)
        {
            if (!this.TryMatch(type, out var token))
            {
                throw new FormatException($"Expected token {type}, but got {this.Peek().Value} (index {this.Peek().Index})");
            }
            return token!;
        }

        private bool TryMatch(BnfTokenType type) => this.TryMatch(type, out var _);

        private bool TryMatch(BnfTokenType type, [MaybeNullWhen(false)] out BnfToken? token)
        {
            if (this.Peek().Type == type)
            {
                token = this.Consume();
                return true;
            }
            else
            {
                token = null;
                return false;
            }
        }

        private BnfToken Consume()
        {
            var result = this.Peek();
            ++this.index;
            return result;
        }

        private BnfToken Peek(int ahead = 0) => this.tokens[this.index + ahead];
    }
}
