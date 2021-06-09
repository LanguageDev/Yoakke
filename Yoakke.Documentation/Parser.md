# Using the Parser library
The parser library is constructed very similarly to the lexer, consisting of 2 libraries:
 * `Yoakke.Parser`: Basic structures for parsing.
 * `Yoakke.Parser.Generator`: A source generator that generates a parser from the annotations.

Check out the [basic structures](#basic-structures), then [using the parser generator](#using-the-parser-generator). Since parsing is a lot more diverse then lexing, the library does not provide an as-complete abstraction for a handwritten version as the `LexerBase`. Still, if you want to, you can utilize the very basic functionalities of [`ParserBase`](#the-parserbase-abstraction).

## Basic structures
Parsing something can be succeed, or it can fail. Since both are likely outcomes - errors are not exceptional -, the outcome is modeled as a `ParseResult<T>` type, where the type-parameter `T` is the parsed value, given that it succeeded.

The succeeding variant is `ParseOk<T>`, that contains the actual parsed value alongside other parsing-related information. The error variant is `ParseError`.

## Using the parser generator
The parser generator can be used by annotating methods with BNF-like rules. Let's see a toy example demonstrating the basics:
```csharp
[Parser(typeof(TokenType))]
public partial class Parser
{
    [Rule("if_stmt: KwIf expr '{' stmt '}'")]
    private static Statement MakeIf(
        Token<TokenType> kwIf, 
        Expression condition,
        Token<TokenType> openBrace,
        Statement body,
        Token<TokenType> openBrace) => new IfStatement(condition, body);	

    [Rule("stmt : expr")]
    private static Statement MakeExprStatement(Expression expr) => new ExprStatement(expr);

    [Rule("stmt : if_stmt")]
    private static Statement Identity(Statement stmt) => stmt;

    [Rule("expr : IntLit '==' IntLit")]
    [Rule("expr : IntLit '!=' IntLit")]
    private static Expression MakeCmp(
        Token<TokenType> left, 
        Token<TokenType> op, 
        Token<TokenType> right) => new CmpExpression(left, op, right);

    [Rule("expr : 'true' | 'false'")]
    private static Expression MakeBool(Token<TokenType> t) => new BoolExpression(t);
}
```
The attribute `Parser(typeof(TokenType))` tells the source generator that the annotated class is a parser with rules inside it and the kind of the tokens will be the passed in type - `TokenType` in this case. You can choose not to pass in a token kind, in that case all tokens will be assumed to be `IToken`. **Note, that it is important to qualify the parser class as `partial`.** The generator has to generate code for the class, that can be only achieved through partial classes because of source generator limitations.

Everything else inside the class is normal, except the methods marked with one or more `Rule` attributes. Those are methods that transform the resulting elements of the parse into a structure - like a Parse-Tree or AST. The attributes themselves contain the rules name, along with how it looks. It uses a custom, regex-like extension to BNF, you can find a reference for the supported constructs later in this document.

The annotated methods can be static or non-static, the generator does not differentiate them. Their names, rule and method ordering also don't matter.

The generated parser will inherit from `ParserBase` and will have `ParseXXX` methods defined publicly, there `XXX` is the _PascalCase_ version of the given rule name. For example, the above parser would have a `ParseIfStmt`, `ParseStmt` and `ParseExpr` defined. The return type of these would be a `ParseResult<T>`, where `T` is what their corresponding method returns.

TBD

### Matching terminals

TBD

### The structure of parse errors

TBD

### Left-recursion and precedence

TBD

### The supported BNF flavor

TBD

## The `ParserBase` abstraction

TBD