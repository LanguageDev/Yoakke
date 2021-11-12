# Using the Parser library
The parser library is constructed very similarly to the lexer, consisting of 2 libraries:
 * `Yoakke.Parser`: Basic structures for parsing.
 * `Yoakke.Parser.Generator`: A source generator that generates a parser from the annotations.

Check out the [basic structures](#basic-structures), then [using the parser generator](#using-the-parser-generator). Since parsing is a lot more diverse then lexing, the library does not provide an as-complete abstraction for a handwritten version as the `LexerBase<TKind>`. Still, if you want to, you can utilize the very basic functionalities of [`ParserBase`](#the-parserbase-abstraction).

## Basic structures
Parsing something can succeed, or it can fail. Since both are likely outcomes - errors are not exceptional -, the outcome is modeled as a `ParseResult<T>` type, where the type-parameter `T` is the parsed value, given that it succeeded.

The succeeding variant is `ParseOk<T>`, that contains the actual parsed value alongside other parsing-related information. The error variant is `ParseError`.

## Using the parser generator
The parser generator can be used by annotating methods with BNF-like rules. Let's see a toy example demonstrating the basics:
```csharp
[Parser(typeof(TokenType))]
public partial class Parser
{
    [Rule("if_stmt: KwIf expr '{' stmt '}'")]
    private static Statement MakeIf(
        IToken<TokenType> kwIf,
        Expression condition,
        IToken<TokenType> openBrace,
        Statement body,
        IToken<TokenType> openBrace) => new IfStatement(condition, body);

    [Rule("stmt : expr")]
    private static Statement MakeExprStatement(Expression expr) => new ExprStatement(expr);

    [Rule("stmt : if_stmt")]
    private static Statement Identity(Statement stmt) => stmt;

    [Rule("expr : IntLit '==' IntLit")]
    [Rule("expr : IntLit '!=' IntLit")]
    private static Expression MakeCmp(
        IToken<TokenType> left,
        IToken<TokenType> op,
        IToken<TokenType> right) => new CmpExpression(left, op, right);

    [Rule("expr : 'true' | 'false'")]
    private static Expression MakeBool(IToken<TokenType> t) => new BoolExpression(t);
}
```
The attribute `Parser(typeof(TokenType))` tells the source generator that the annotated class is a parser with rules inside it. Also, the kind of the tokens will be the specified type inside the attribute - `TokenType` in this case. You can choose not to pass in a token kind, in that case all tokens will be assumed to be `IToken`. **Note, that it is important to qualify the parser class as `partial`.** The generator has to generate code for the class, that can be only achieved through partial classes because of source generator limitations.

Everything else inside the class is normal, except the methods marked with one or more `Rule` attributes. Those are methods that transform the resulting elements of the parse into a structure - like a Parse-Tree or AST. The attributes themselves contain the rules name, along with how it looks. It uses a custom, regex-like extension to BNF, you can find a reference for the supported constructs later in this document.

The annotated methods can be static or non-static, the generator does not differentiate them. Their names, rule and method ordering also don't matter.

The generated parser will inherit from `ParserBase` and will have `ParseXXX` methods defined publicly, where `XXX` is the _PascalCase_ version of the given rule name. For example, the above parser would have a `ParseIfStmt`, `ParseStmt` and `ParseExpr` defined. The return type of these would be a `ParseResult<T>`, where `T` is what their corresponding method returns.

Using the parser is very simple:

```csharp
var parser = new Parser(lexer);
var result = parser.ParseStmt();
if (result.IsOk)
{
    var ast = result.Ok.Value;
    // Do something with your AST
}
else
{
    var error = result.Error;
    Console.WriteLine("Parse error!");
    // We will talk about the structure of errors later
}
```

The passed in lexer has to implement the `ILexer` interface defined in the `Yoakke.Lexer` library.

### Matching terminals
As you can see in the above sample, terminals can be matched two ways: by value and by kind.

You can match by value by writing the textual value between single-quotes. This is how the `{` and `}` tokens are matched, as well as the keywords `true` and `false`.

You can match by token kind by simply typing its enumeration field name. For example, the `KwIf`  and `IntLit` are matched this way - assuming they are defined in the enumeration `TokenType`.

### The structure of parse errors

Parse errors are described in the `ParseError` class. They might seem a bit complicated at first, that is because they contain all possibilities the code could have followed, when the error occurred. They also try to provide contextual information of where the error occurred.

`ParseError` contains the token it got stuck on (a member called `Got`) and a `Dictionary` of all alternative paths the parser took and errored out on that token. The dictionary maps from the context - which is the name of the rule that was parsed at the moment - to a smaller structure - `ParseErrorElement` -, that describes what that alternative expected instead of the gotten token.

Printing out all the information the error provides would look something like:
```csharp
var err = result.Error;
foreach (var element in err.Elements.Values)
{
    Console.WriteLine($"  expected {string.Join(" or ", element.Expected)} while parsing {element.Context}");
}
Console.WriteLine($"  but got {(err.Got == null ? "end of input" : err.Got.Text)}");
```

### Left-recursion

The parser supports left- and right-recursion out of the box:
```csharp
[Rule("left : left 'a'")]
private static string Left(string a, IToken b) => $"({a}), {b.Text}";

[Rule("right : 'a' right")]
private static string Left(IToken a, string b) => $"{a.Text}, ({b})";

[Rule("left : 'a'")]
[Rule("right : 'a'")]
private static string Identity(IToken a) => a.Text;
```

On the input `a a a a`, `ParseLeft` would return `"(((a) a) a) a"`, while `ParseRight` would return `a (a (a (a)))`.

### Precedence parser

Since parsing expression that contain operators with proper precedence and associativity is something most compilers have to deal with, there is support to simplify the process. You can build a precedence-based parser like so:
```csharp
[Right("^")]
[Left("*", "/", "%")]
[Left("+", "-")]
[Rule("expression")]
public static int BinOp(int a, IToken op, int b) => op.Text switch
{
    "^" => (int)Math.Pow(a, b),
    "*" => a * b,
    "/" => a / b,
    "%" => a % b,
    "+" => a + b,
    "-" => a - b,
    _ => throw new NotImplementedException(),
};

[Rule("expression : '(' expression ')'")]
public static int Grouping(IToken _1, int n, IToken _2) => n;

[Rule("expression : IntLit")]
public static int IntLit(IToken token) => int.Parse(token.Text);
```

The above implements a calculator, that properly handles 6 arithmetic operations (addition, subtraction, multiplication, division, modulo and exponentiation) with their precedence and associativity. It also handles grouping using parenthesis.

The precedence-table starts with the first `Left` or `Right` attribute on the top, each describing a level of precedence. The topmost attribute has the highest precedence - `[Right("^")]` in this case. `Left` defines a left-associative precedence level, `Right` defines a right-associative one.

After the precedence-table, a dummy `Rule` has to be added, that defines the operands of the operators. In this case it is said to be `expression`, so the `ParseExpression` method will first walk through the precedence table, and treat all other definitions - like grouping - as the lowest level elements.

### The supported BNF flavor
The notation supported for the grammar is extended, some regular expression operators can be used to make writing common constructs simpler. Below is a table summarizing the available constructs.

| Name | Syntax | Value type |
| --- | --- | --- |
| Literal by text | `'text'` | An `IToken<TKind>`, if the parser is given a token-kind type, `IToken` otherwise |
| Literal by kind | `KindName` | An `IToken<TKind>`, if the parser is given a token-kind type, `IToken` otherwise |
| Rule invocation | `rule_name` | Whatever the invoked rule returns. |
| Sequence | `a b c` | On top-level they become separate parameters. Nested, they become tuples of the elements. |
| Alternative | `a | b` | The two sides are assumed to result in the same parsed type. |
| Grouping | `(a b c)` | A tuple of all the sub-elements. |
| Optional | `a?` | A nullable type of whatever the sub-element returns. |
| 0 or more | `a*` | An `IReadOnlyList<T>` of whatever the sub-element returns. |
| 1 or more | `a+` | An `IReadOnlyList<T>` of whatever the sub-element returns. |

### Punctuated sequences
Some patterns are kind of clumsy to handle, because their types become very ugly and complex to type. Most commonly, these would be punctuated sequences of values. Let's see how you would write them and what the library can give you to simplify these.

#### 0 or more elements without a trailing separator
This would be how C handles it's argument list for a function call. 0 or more expressions, each separated by a comma, trailing comma is not allowed. In grammar you would write this as: `(expr (',' expr)*)?`. The type of this would be `(Expression, IReadOnlyList<(IToken<TKind>, Expression)>)?`, which is not very pretty.

#### 1 or more elements without a trailing separator
Similar to the previous example, but at least one element is mandatory: `expr (',' expr)*`. The type of this is `(Expression, IReadOnlyList<(IToken<TKind>, Expression)>)`.

#### 0 or more elements with an optional trailing separator
This would be how C# handles it's brace-initialization. 0 or more initializers, each separated by a comma, the trailing comma is optional. In grammar this would be `(initializer (',' initializer)* ','?)?`. The type of this would be the worst so far: `(Initializer, IReadOnlyList<(IToken<TKind>, Expression)>, IToken<TKind>?)?`.

#### 1 or more elements with an optional trailing separator
Similar to the previous case, but at least one element is mandatory: `(initializer (',' initializer)* ','?)`. The type of this is `(Initializer, IReadOnlyList<(IToken<TKind>, Expression)>, IToken<TKind>?)`.

#### The solution
Since these are very common patterns, the parser library provides a `Punctuated<TValue, TPunct>` type. `TValue` is the punctuated element type - like an `Expression`, and `TPunct` is the punctuation type - like a `IToken<TKind>`. The above complex types are implicitly convertible to `Punctuated<TValue, TPunct>`, which provides simpler access to its elements.

Let's see an example usage:
```csharp
[Rule("name_list : Lparen (Identifier (',' Identifier)*)? Rparen")]
private static List<string> NameList(
    IToken<TokenType> _lp,
    Punctuated<IToken<TokenType>, IToken<TokenType>> elements,
    IToken<TokenType> _rp) => elements.Values.Select(t => t.Text).ToList();
```

**Important:** In case of 1-or-more patterns the result is a non-nullable tuple. If it's on the top level, the library will unwrap it into the parameter list, which makes it impossible to convert to `Punctuated<TValue, TPunct>`. To avoid that, group the construct inside parentheses.

## The `ParserBase` abstraction
TBD
