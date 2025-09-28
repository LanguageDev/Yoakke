# Parser Component

The SynKit Parser component provides LR parser generation with attribute-based grammar specification.

## Overview

The parser component generates efficient LR parsers from grammar rules specified as C# method attributes. It supports:

- **LR(1) Parsing**: Efficient bottom-up parsing with one token lookahead
- **Precedence & Associativity**: Left/right associative operators with precedence levels
- **Error Recovery**: Rich error reporting with context information
- **Type Safety**: Strongly-typed parse results
- **Custom Actions**: User-defined semantic actions for each production rule

## Basic Usage

### 1. Define Parser Class

```csharp
[Parser(typeof(TokenType))]
public partial class ExpressionParser
{
    // Parser rules will be defined here
}
```

### 2. Grammar Rules

Grammar rules are defined as static methods with the `[Rule]` attribute:

```csharp
[Rule("expression: IntLiteral")]
public static int IntLiteral(IToken token) => int.Parse(token.Text);

[Rule("expression: '(' expression ')'")]  
public static int Parentheses(IToken open, int expr, IToken close) => expr;
```

### 3. Binary Operators

Use precedence attributes to define operator precedence and associativity:

```csharp
[Left("+", "-")]          // Lower precedence, left associative
[Left("*", "/")]          // Higher precedence, left associative  
[Right("^")]              // Right associative exponentiation
[Rule("expression")]      // Inferred rule for binary operators
public static int BinaryOp(int left, IToken op, int right)
{
    return op.Text switch
    {
        "+" => left + right,
        "-" => left - right,
        "*" => left * right,
        "/" => left / right,
        "^" => (int)Math.Pow(left, right),
        _ => throw new NotSupportedException($"Operator {op.Text}")
    };
}
```

## Rule Syntax

### Rule Format
```
[Rule("nonterminal: production")]
```

Where:
- `nonterminal` is the left-hand side symbol
- `production` is a sequence of terminals and nonterminals

### Terminal References
- `'literal'` - Match literal token text
- `TokenType` - Match token by enum value
- `"string"` - Alternative literal syntax

### Examples
```csharp
[Rule("statement: 'if' '(' expression ')' statement")]
[Rule("statement: 'if' '(' expression ')' statement 'else' statement")]
[Rule("expression: Identifier")]
[Rule("expression: IntLiteral")]
```

## Precedence and Associativity

### Precedence Levels
Precedence is determined by the order of attributes and method declaration:

```csharp
[Left("||")]                    // Lowest precedence
[Left("&&")]  
[Left("==", "!=")]
[Left("<", ">", "<=", ">=")]
[Left("+", "-")]
[Left("*", "/", "%")]
[Right("^")]                    // Highest precedence
[Rule("expression")]
public static T BinaryOperator(T left, IToken op, T right) { ... }
```

### Associativity
- `[Left(...)]` - Left associative (a + b + c = (a + b) + c)
- `[Right(...)]` - Right associative (a ^ b ^ c = a ^ (b ^ c))
- `[Precedence(...)]` - Non-associative

## Error Handling

### ParseResult<T>
All parse methods return `ParseResult<T>`:

```csharp
var result = parser.ParseExpression();
if (result.IsOk)
{
    Console.WriteLine($"Parsed value: {result.Ok.Value}");
}
else
{
    HandleParseError(result.Error);
}
```

### ParseError Details
ParseError provides rich error information:

```csharp
void HandleParseError(ParseError error)
{
    Console.WriteLine($"Parse failed at position {error.Position}");
    Console.WriteLine($"Got: {error.Got?.ToString() ?? "end of input"}");
    
    foreach (var (context, element) in error.Elements)
    {
        Console.WriteLine($"In {context}, expected: {string.Join(" or ", element.Expected)}");
    }
}
```

### Error Recovery

Implement synchronization points for error recovery:

```csharp
public void Synchronize()
{
    // Skip tokens until we find a synchronization point
    while (this.TokenStream.TryPeek(out var token) && 
           token.Kind != TokenType.Semicolon &&
           token.Kind != TokenType.End)
    {
        this.TokenStream.Consume(1);
    }
    
    // Consume the synchronization token
    if (this.TokenStream.TryPeek(out var sync) && sync.Kind == TokenType.Semicolon)
    {
        this.TokenStream.Consume(1);
    }
}
```

## Advanced Features

### Custom Start Rules
Specify custom start rules for different parsing contexts:

```csharp
[Rule("program: statement*")]
public static List<Statement> Program(IReadOnlyList<Statement> statements) 
    => statements.ToList();

[Rule("statement: expression ';'")]
public static Statement ExpressionStatement(Expression expr, IToken semi)
    => new ExpressionStatement(expr);
```

### Optional and Repetition
Use standard EBNF-style syntax:

```csharp
[Rule("parameter_list: parameter (',' parameter)*")]
public static List<Parameter> ParameterList(Parameter first, IReadOnlyList<(IToken, Parameter)> rest)
{
    var result = new List<Parameter> { first };
    result.AddRange(rest.Select(x => x.Item2));
    return result;
}

[Rule("modifier_list: modifier*")]
public static List<Modifier> ModifierList(IReadOnlyList<Modifier> modifiers)
    => modifiers.ToList();
```

## Performance Considerations

### Parser Generation
- Parsers are generated at compile time with no runtime overhead
- Generated parsing tables are optimized for the specific grammar
- Type-safe method dispatch avoids boxing/unboxing

### Memory Usage
- ParseResult<T> uses discriminated union pattern
- Immutable parse trees prevent accidental modification
- Error objects can be reused and combined efficiently

### Error Performance
- Error merging using `|` operator is efficient
- Position comparison short-circuits unnecessary work
- Context information is lazily formatted

## Best Practices

### Grammar Design
1. **Left Recursion**: Prefer left-recursive rules for left-associative operators
2. **Precedence**: Use precedence attributes instead of encoding precedence in grammar
3. **Error Recovery**: Place synchronization points at statement boundaries
4. **Semantic Actions**: Keep actions simple and focused on tree construction

### Performance
1. **Minimize Allocations**: Reuse collections where possible
2. **Efficient Comparisons**: Use token kinds instead of text comparison
3. **Lazy Evaluation**: Defer expensive computations until needed

### Debugging
1. **Generated Code**: Examine generated parser code for understanding
2. **Parse Tables**: Use debugging tools to inspect LR tables
3. **Error Context**: Provide meaningful context names in rules

<!--
## Examples

See the [Examples](../examples/README.md) section for complete working examples:
- [Simple Expression Parser](../examples/expression-parser.md)
- [Statement Parser](../examples/statement-parser.md)
- [JSON Parser](../examples/json-parser.md)
-->