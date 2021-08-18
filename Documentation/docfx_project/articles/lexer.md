# Using the Lexer library
There are 2 libraries for lexing:
-   `Yoakke.Lexer`: Basic structures for lexing, abstraction for a hand-written lexer.
-   `Yoakke.Lexer.Generator`: A source generator that can generate a lexer from attribute annotations on token-types.

Check out the [basic structures](#basic-structures), then either [using the lexer generator](#using-the-lexer-generator) or [rolling your own lexer](#rolling-your-own-lexer).

### Basic structures
No matter which version you choose, there are some basic structures defined in `Yoakke.Lexer` that you need to know. Here is a description of them:

1.  `IToken`: They represent a kind-less token, meaning it's just a text-range and the text value itself. All token implementations should implement this.
2.  `IToken<TKind>`: They represent a token with some kind tag. Implements `IToken`.
3.  `Token<TKind>`: A token with an actual kind, that is defined by the generic parameter `TKind`. This can be used as a default implementation for `IToken<TKind>` in most cases.
4.  `ILexer`: An interface for a lexer that can produce `IToken`s. All lexers should implement this.
5.  `ILexer<TToken>`: An interface for a lexer that can produce `TToken`s. It inherits from `ILexer`.

### Using the lexer generator
The lexer generator can be used by annotating a class, to generate a lexer for a token-kind enumeration with a few attributes. Let's see an example:
```csharp
public enum TokenType
{
    [Error] Error,
    [End] End,
    [Ignore] [Regex(@"[ \t\r\n]")] Whitespace,
    [Ignore] [Regex(@"//[^\r\n]*")] LineComment,

    [Token("if")] KwIf,
    [Token("else")] KwElse,
    [Regex(@"[A-Za-z_][A-Za-z0-9_]*")] Identifier,

    [Token("+")] Plus,
    [Token("-")] Minus,

    [Regex(@"[0-9]+")] IntLiteral,
}

[Lexer(typeof(TokenType))]
public partial class MyLexer {}
```
The attribute `[Lexer(typeof(TokenType))]` tells the source generator to generate the lexer for the `TokenType` enumeration inside the annotated class `MyLexer`.  **Note, that it is important to qualify the lexer class as `partial`.** The generator has to generate code for the class, that can be only achieved through partial classes because of source generator limitations.

The generated class implements `ILexer<Token<TokenType>>` and inherits from `LexerBase<Token<TokenType>>`. You can find more documentation about `LexerBase<TToken>` later in the chapter [rolling your own lexer](#rolling-your-own-lexer).

There are two special token types you need to define: `Error` and `End`. An `Error` kind token will be returned when there is no appropriate rule to apply, hence it denotes a lexical error. The `End` token type will signal the end of the source text.

All other tokens are defined using regular expressions with the `Regex("...")` attribute. **Important:** These are not your usual RegEx-engine flavored regular expressions. The expression power of these are actually only the set of regular languages, no backtracking support is present. This is so that the regexes can be transformed into _Deterministic Finite Automatas_, making the resulting lexer very fast. You can find a reference for the supported regex constructs [later in this document](#supported-regex-constructs).
The `Token` attribute is a shorthand for escaping all regex characters, meaning that it's a literal match.

`Ignore` tells the generator that the given token kind should not be part of the resulting token sequence.

#### Rule precedence
You might be wondering, how can the library differentiate between regexes, that both match the same input text. In our previous example, `"else"` could be a valid identifier token too, as it matches the regular expression `[A-Za-z_][A-Za-z0-9_]*`.

The rules define their precedence by their order of declaration. Rules defined earlier have higher precedence, that's why `"if"` and `"else"` get to match these literals before the identifier rule does. If you defined the identifier rule before the keywords, then `"if"` and `"else"` would be recognized as identifiers instead.

#### Built-in regexes
There are some regexes that are very common among language lexemes, so the `Regexes` class contains some of these definitions:
 * `Regexes.Identifier`: A C-style identifier, matching `[A-Za-z_][A-Za-z0-9_]*`
 * `Regexes.Whitespace`: Common whitespace characters, matching `[ \t\r\n]`
 * `Regexes.IntLiteral`: Decimal numbers, matching `[0-9]+`
 * `Regexes.HexLiteral`: A hexadecimal number with `0x` prefix, matching `0x[0-9a-fA-F]+`
 * `Regexes.StringLiteral`: A *single-line* string-literal that starts and ends with a `"`, supports any escape character with `\`
 * `Regexes.LineComment`: A C-style single-line comment, starting with a `//`, ending with the line, matching `//[^\r\n]*`
 * `Regexes.MultilineComment`: A C-style multi-line comment starting with `/*` and ending with `*/`. Please note, that this is _not a nestable multi-line comment_. That would make it non-regular, so the library wouldn't be able to reasonably support it. If you want to support this, you should probably [roll your own lexer](#rolling-your-own-lexer).

Rewriting the above sample to use these built-ins:
```csharp
public enum TokenType
{
    [Error] Error,
    [End] End,
    [Ignore] [Regex(Regexes.Whitespace)] Whitespace,
    [Ignore] [Regex(Regexes.LineComment)] LineComment,

    [Token("if")] KwIf,
    [Token("else")] KwElse,
    [Regex(Regexes.Identifier)] Identifier,

    [Token("+")] Plus,
    [Token("-")] Minus,

    [Regex(Regexes.IntLiteral)] IntLiteral,
}

[Lexer(typeof(TokenType))]
public partial class MyLexer {}
```

#### Multiple rules for the same token
You might want to give multiple definitions for the same token, and for good reasons. For example, you might want to ignore all comments and whitespaces under the same token name. Another use-case would be if your language allowed both `&&` and the keyword `and` as the conjunction operator. Having two separate definitions for these would make parsing very annoying.

Fortunately, you can define as many rules for a token-type, as you wish. Presenting the above examples:
```csharp
[Ignore]
[Regex(Regexes.Whitespace)]
[Regex(Regexes.LineComment)]
[Regex(Regexes.MultilineComment)] IgnoreMe,

[Token("and")]
[Token("&&")] AndOperator,
```

#### Supported regex constructs
Only those regular expressions are supported, that keep their expression power at the regular language level. The constructs are the usual, what you would find in a modern engine - just less of them:

 * `abc`: Match the literals `a`, `b` and `c` in sequence
 * `a|b`: Match either `a` or `b`
 * `a(b|c)`: Grouping for precedence, match `a`, then either `b` or `c`
 * `[abc]`: Match any character - `a`, `b` or `c` - in the character class
 * `[a-z]`: Match any character in the range `a` to `z` (inclusive)
 * `a?`: Optionally match `a`
 * `a*`: Match `a` 0 or more times
 * `a+`: Match `a` 1 or more times
 * `a{3,6}`: Match `a` 3 to 6 times (inclusive)
 * `a{3,}`: Match `a` at least 3 times

Any special characters need to be escaped with `\`. If you want to match escaped characters, like `\n`, it is advised to use `@` strings (verbatim strings).

### Rolling your own lexer
If, for some reason you'd like to roll your own lexer - either because your lexer needs to recognize non-regular languages, or just for fun -, the library gives you support to do so. Namely, you can inherit from the `LexerBase<TToken>` class.

`LexerBase<TToken>` helps you defined a lexer, that will produce `TToken`s. All you have to do, is overwrite it's `TToken Next()` method, to lex the next token from the input. You get a few helper methods for that:
 * `Matches(string, int)`: Checks, if the input text matches a given string in some offset
 * `TryPeek(out char, int)`: Peeks ahead some characters in the input without consuming it
 * `Peek(int, char)`: Peeks ahead some characters in the input, returning a default one, if the offset points past the end of source
 * `Skip()`: Skips (consumes) a single character in the input
 * `Skip(int)`: Skips (consumes) a given amount of characters in the input
 * `Take(int)`: Consumes a given amount of characters in the input and constructs a string from them
 * `TakeToken(int, Func<Range, string, TToken>)`: Consumes a given amount of characters in the input and constructs an `TToken` using the factory function
 * `TakeToken(TKind, int)`: Consumes a given amount of characters in the input and constructs a `Token<TKind>` from them

For more precise documentation please refer to the API docs for each method.

`LexerBase<TToken>` works with a `System.IO.TextReader`, so it can lex from simple strings, files or even from console input - handy for a REPL.

Let's implement the lexer we generated above, but using `LexerBase<TToken>`, and making multi-line comments nestable, to have an advantage over the generated implementation:
```csharp
public class MyLexer : LexerBase<Token<TokenType>>
{
    public MyLexer(TextReader reader)
        : base(reader)
    {
    }

    // Just a helper for valid identifier characters
    private static bool IsIdent(char ch) => char.IsLetterOrDigit(ch) || ch == '_';

    public Token<TokenType> Next()
    {
        while (true)
        {
            // If there's nothing to peek, means end of source
            if (!TryPeek(out var _)) return TakeToken(TokenType.End, 0);
            // Skip whitespace
            if (char.IsWhiteSpace(Peek()))
            {
                Skip();
                continue;
            }
            // Line-comment
            if (Matches("//"))
            {
                // Start from 2, as // is not consumed yet
                int i = 2;
                // Take advantage of the fact, that the default character can signal the EOF too
                for (; Peek(i, '\n') != '\n'; ++i);
                Skip(i);
                continue;
            }
            // Nestable multi-line comment
            if (Matches("/*"))
            {
                // Start from 2, as /* is not consumed yet
                int i = 2;
                int depth = 1;
                // While there is nesting and also it's not EOF
                while (depth > 0 && TryPeek(out var _))
                {
                    if (Match("/*"))
                    {
                        ++depth;
                        i += 2;
                    }
                    if (Match("*/"))
                    {
                        --depth;
                        i += 2;
                    }
                    else ++i;
                }
                Skip(i);
                continue;
            }
            // Plus and minus
            if (Peek() == '+') return TakeToken(TokenType.Plus, 1);
            if (Peek() == '-') return TakeToken(TokenType.Minus, 1);
            // Int literal
            if (char.IsDigit(Peek()))
            {
                int i = 1;
                for (; char.IsDigit(Peek(i)); ++i);
                return TakeToken(TokenType.IntLiteral, i);
            }
            // Keywords and identifier
            if (IsIdent(Peek()))
            {
                int i = 1;
                for (; IsIdent(Peek(i)); ++i);
                var result = TakeToken(TokenType.Identifier, i);
                // If it's a keyword, transform the token type
                if (result.Text == "if") return new Token<TokenType>(result.Range, result.Text, TokenType.KwIf);
                else if (result.Text == "else") return new Token<TokenType>(result.Range, result.Text, TokenType.KwElse);
                else return result;
            }
            // Unknown, error
            return TakeToken(TokenType.Error, 1);
        }
    }
}
```

Just like with the generated lexer, text position tracking is taken care of for you.
