using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Parser.Generator.Ast;

namespace Yoakke.Parser.Generator
{
    internal class BnfParser
    {
        public static (string Name, BnfAst Ast) Parse(string source, HashSet<IFieldSymbol> tokenKinds) => 
            Parse(BnfLexer.Lex(source), tokenKinds);
        public static (string Name, BnfAst Ast) Parse(IList<BnfToken> tokens, HashSet<IFieldSymbol> tokenKinds) => 
            new BnfParser(tokens, tokenKinds).ParseRule();

        private IList<BnfToken> tokens;
        private int index;
        private HashSet<IFieldSymbol> tokenKinds;

        public BnfParser(IList<BnfToken> tokens, HashSet<IFieldSymbol> tokenKinds)
        {
            this.tokens = tokens;
            this.tokenKinds = tokenKinds;
        }

        public (string Name, BnfAst Ast) ParseRule()
        {
            var name = Expect(BnfTokenType.Identifier);
            BnfAst ast = null;
            if (TryMatch(BnfTokenType.Colon)) ast = ParseAlt();
            Expect(BnfTokenType.End);
            return (name.Value, ast);
        }

        private BnfAst ParseAlt()
        {
            var first = ParseSeq();
            if (TryMatch(BnfTokenType.Pipe)) return new BnfAst.Alt(first, ParseAlt());
            return first;
        }

        private BnfAst ParseSeq()
        {
            var first = ParsePostfix();
            var ptype = Peek().Type;
            if (ptype != BnfTokenType.End && ptype != BnfTokenType.CloseParen && ptype != BnfTokenType.Pipe)
            {
                return new BnfAst.Seq(first, ParseSeq());
            }
            return first;
        }

        private BnfAst ParsePostfix()
        {
            var atom = ParseAtom();
            if (TryMatch(BnfTokenType.QuestionMark)) return new BnfAst.Opt(atom);
            if (TryMatch(BnfTokenType.Star)) return new BnfAst.Rep0(atom);
            if (TryMatch(BnfTokenType.Plus)) return new BnfAst.Rep1(atom);
            return atom;
        }

        private BnfAst ParseAtom()
        {
            if (TryMatch(BnfTokenType.OpenParen))
            {
                var sub = ParseAlt();
                Expect(BnfTokenType.CloseParen);
                return sub;
            }
            if (TryMatch(BnfTokenType.Identifier, out var ident))
            {
                if (TryParseTokenType(ident.Value, out var kind))
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
            if (TryMatch(BnfTokenType.StringLiteral, out var str)) return new BnfAst.Literal(StrToString(str));

            throw new FormatException($"Unexpected token {Peek().Type} (index {Peek().Index})");
        }

        private string StrToString(BnfToken token) => token.Value.Substring(1, token.Value.Length - 2);

        private BnfToken Expect(BnfTokenType type)
        {
            if (!TryMatch(type, out var token))
            {
                throw new FormatException($"Expected token {type}, but got {Peek().Value} (index {Peek().Index})");
            }
            return token;
        }

        private bool TryMatch(BnfTokenType type) => TryMatch(type, out var _);

        private bool TryMatch(BnfTokenType type, out BnfToken token)
        {
            if (Peek().Type == type)
            {
                token = Consume();
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
            var result = Peek();
            ++index;
            return result;
        }

        private BnfToken Peek(int ahead = 0) => tokens[index + ahead];

        private bool TryParseTokenType(string value, out object kind)
        {
            kind = null;
            if (tokenKinds == null) return false;
            kind = tokenKinds.FirstOrDefault(k => k.Name == value);
            return kind != null;
        }
    }
}
