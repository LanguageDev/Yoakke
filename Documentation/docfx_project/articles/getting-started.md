# Getting Started with SynKit

This guide will help you get up and running with the Yoakke SynKit library.

## Installation

### NuGet Packages

SynKit is distributed through several NuGet packages:

```xml
<!-- Core lexer functionality -->
<PackageReference Include="Yoakke.SynKit.Lexer" Version="x.x.x" />

<!-- Core parser functionality -->
<PackageReference Include="Yoakke.SynKit.Parser" Version="x.x.x" />

<!-- Source generators for lexer/parser generation -->
<PackageReference Include="Yoakke.SynKit.Lexer.Generator" Version="x.x.x" />
<PackageReference Include="Yoakke.SynKit.Parser.Generator" Version="x.x.x" />

<!-- Error reporting and diagnostics -->
<PackageReference Include="Yoakke.SynKit.Reporting" Version="x.x.x" />

<!-- Text processing utilities -->
<PackageReference Include="Yoakke.SynKit.Text" Version="x.x.x" />
```

## Your First Lexer

1. Define your token types:

```csharp
public enum TokenType
{
    [Error] Error,
    [End] End,
    [Ignore] [Regex(Regexes.Whitespace)] Whitespace,
    
    [Token("+")] Plus,
    [Token("-")] Minus,
    [Token("*")] Multiply,
    [Token("/")] Divide,
    
    [Regex(Regexes.IntLiteral)] IntLiteral,
    [Regex(Regexes.Identifier)] Identifier,
}
```

2. Create the lexer:

```csharp
[Lexer(typeof(TokenType))]
public partial class MyLexer
{
    // The source generator will implement the lexer logic
}
```

3. Use the lexer:

```csharp
var lexer = new MyLexer("10 + 20 * 3");
while (true)
{
    var token = lexer.Next();
    if (token.Kind == TokenType.End) break;
    Console.WriteLine($"{token.Text} [{token.Kind}]");
}
```

## Your First Parser

1. Define grammar rules:

```csharp
[Parser(typeof(TokenType))]
public partial class MyParser
{
    [Left("+", "-")]        // Lower precedence
    [Left("*", "/")]        // Higher precedence  
    [Rule("expression")]
    public static int BinaryOp(int left, IToken op, int right)
    {
        return op.Text switch
        {
            "+" => left + right,
            "-" => left - right,
            "*" => left * right,
            "/" => left / right,
            _ => throw new NotSupportedException()
        };
    }
    
    [Rule("expression: IntLiteral")]
    public static int IntLiteral(IToken token) => int.Parse(token.Text);
    
    [Rule("expression: '(' expression ')'")]
    public static int Parentheses(IToken _, int expr, IToken __) => expr;
}
```

2. Use the parser:

```csharp
var lexer = new MyLexer("(10 + 20) * 3");
var parser = new MyParser(lexer);

var result = parser.ParseExpression();
if (result.IsOk)
{
    Console.WriteLine($"Result: {result.Ok.Value}"); // Result: 90
}
else
{
    Console.WriteLine($"Parse error: {result.Error}");
}
```

## Error Handling

SynKit provides comprehensive error handling:

```csharp
var result = parser.ParseExpression();
if (result.IsError)
{
    var error = result.Error;
    Console.WriteLine($"Parse failed at position {error.Position}");
    
    foreach (var element in error.Elements.Values)
    {
        Console.WriteLine($"Expected: {string.Join(", ", element.Expected)}");
        Console.WriteLine($"Context: {element.Context}");
    }
}
```

## Next Steps

- Learn about [Architecture](architecture.md) concepts
- Check [Best Practices](best-practices.md)
<!-- 
- Explore [Components](components/README.md) in detail
- Follow detailed [Tutorials](tutorials/README.md)
- Study the [Examples](examples/README.md)
 -->