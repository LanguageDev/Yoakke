# Error Handling Best Practices

This guide covers best practices for handling errors in SynKit parsers and lexers.

## Understanding ParseError

The `ParseError` class in SynKit is designed to provide rich, contextual error information:

```csharp
public class ParseError
{
    public IReadOnlyDictionary<string, ParseErrorElement> Elements { get; }
    public object? Got { get; }
    public IComparable Position { get; }
}
```

### Key Features

1. **Multiple Contexts**: Errors can represent failures in multiple parsing contexts
2. **Position Tracking**: Precise location information for error reporting
3. **Mergeable**: Errors can be combined using the `|` operator
4. **Rich Information**: Expected vs. actual tokens with semantic context

## Error Merging Patterns

### Same Position Merging
When errors occur at the same position, they are merged:

```csharp
var error1 = new ParseError("^", null, 12, "expression");
var error2 = new ParseError("|", null, 12, "expression");
var merged = error1 | error2;

// Result: Expected "^" or "|" in expression context
```

### Different Context Merging
Errors from different parsing contexts are preserved:

```csharp
var error1 = new ParseError("^", null, 12, "expression");
var error2 = new ParseError("if", null, 12, "statement");
var merged = error1 | error2;

// Result: Two separate error contexts preserved
```

### Position Priority
Errors at later positions take precedence:

```csharp
var early = new ParseError("(", null, 10, "expression");
var later = new ParseError(")", null, 15, "expression");  
var result = early | later;

// Result: Only the error at position 15 is kept
```

## Error Recovery Strategies

### 1. Synchronization Points

Implement synchronization methods to recover from errors:

```csharp
[Parser(typeof(TokenType))]
public partial class MyParser
{
    public void SynchronizeToStatement()
    {
        // Skip until we find a statement boundary
        while (this.TokenStream.TryPeek(out var token) && 
               token.Kind != TokenType.Semicolon &&
               token.Kind != TokenType.CloseBrace &&
               token.Kind != TokenType.End)
        {
            this.TokenStream.Consume(1);
        }
        
        // Consume the synchronization token
        if (this.TokenStream.TryPeek(out var sync))
        {
            if (sync.Kind == TokenType.Semicolon)
                this.TokenStream.Consume(1);
        }
    }
}
```

### 2. Panic Mode Recovery

Continue parsing after errors by skipping to safe points:

```csharp
public List<Statement> ParseStatements()
{
    var statements = new List<Statement>();
    
    while (!this.TokenStream.IsEnd)
    {
        var result = this.ParseStatement();
        if (result.IsOk)
        {
            statements.Add(result.Ok.Value);
        }
        else
        {
            // Report error
            ReportError(result.Error);
            
            // Synchronize and continue
            this.SynchronizeToStatement();
        }
    }
    
    return statements;
}
```

### 3. Context-Aware Recovery

Use different recovery strategies based on parsing context:

```csharp
public void Synchronize(string context)
{
    switch (context)
    {
        case "expression":
            SynchronizeToOperator();
            break;
        case "statement":
            SynchronizeToStatement();
            break;
        case "declaration":
            SynchronizeToDeclaration();
            break;
    }
}
```

## Error Reporting

### 1. User-Friendly Messages

Convert technical parse errors to user-friendly messages:

```csharp
public string FormatError(ParseError error)
{
    var messages = new List<string>();
    
    foreach (var (context, element) in error.Elements)
    {
        var expected = string.Join(" or ", element.Expected);
        var got = error.Got?.ToString() ?? "end of input";
        
        messages.Add($"Expected {expected} in {context}, but got {got}");
    }
    
    return string.Join("\n", messages);
}
```

### 2. Position Information

Include source location in error messages:

```csharp
public void ReportError(ParseError error, SourceFile sourceFile)
{
    if (error.Position is Position pos)
    {
        Console.WriteLine($"Error at line {pos.Line}, column {pos.Column}:");
        Console.WriteLine(FormatError(error));
        
        // Show source context
        ShowSourceContext(sourceFile, pos);
    }
}
```

### 3. Multiple Error Reporting

Handle multiple errors in a single parse session:

```csharp
public class ErrorCollector
{
    private readonly List<ParseError> errors = new();
    
    public void AddError(ParseError error)
    {
        errors.Add(error);
    }
    
    public void ReportAll(SourceFile sourceFile)
    {
        foreach (var error in errors.OrderBy(e => e.Position))
        {
            ReportError(error, sourceFile);
        }
    }
}
```

## Testing Error Cases

### 1. Unit Test Error Scenarios

Test specific error conditions:

```csharp
[Fact]
public void TestMissingClosingParen()
{
    var lexer = new TestLexer("(1 + 2");
    var parser = new TestParser(lexer);
    
    var result = parser.ParseExpression();
    
    Assert.True(result.IsError);
    Assert.Contains(")", result.Error.Elements.Values
        .SelectMany(e => e.Expected));
}
```

### 2. Error Recovery Testing

Verify parser can continue after errors:

```csharp
[Fact]
public void TestErrorRecovery()
{
    var lexer = new TestLexer("x = ; y = 5;");
    var parser = new TestParser(lexer);
    
    var statements = parser.ParseStatements();
    
    // Should recover and parse the second statement
    Assert.Single(statements);
    Assert.Equal("y = 5", statements[0].ToString());
}
```

### 3. Error Merging Tests

Test error combination behavior:

```csharp
[Fact]
public void TestErrorMerging()
{
    var error1 = new ParseError("(", null, 10, "expression");
    var error2 = new ParseError("[", null, 10, "expression");
    
    var merged = error1 | error2;
    
    Assert.Single(merged.Elements);
    Assert.Equal(2, merged.Elements["expression"].Expected.Count);
}
```

## Performance Considerations

### 1. Error Object Reuse

Avoid creating unnecessary error objects:

```csharp
// Good: Reuse common error patterns
private static readonly ParseError MissingSemicolon = 
    new ParseError(";", null, Position.Unknown, "statement");

// Avoid: Creating new errors for common cases
// new ParseError(";", null, position, "statement");
```

### 2. Lazy Error Formatting

Defer expensive error message formatting:

```csharp
public class LazyErrorMessage
{
    private readonly ParseError error;
    private string? cachedMessage;
    
    public override string ToString()
    {
        return cachedMessage ??= FormatError(error);
    }
}
```

### 3. Error Context Optimization

Use lightweight context identifiers:

```csharp
// Good: Use constant strings
private const string ExpressionContext = "expression";
private const string StatementContext = "statement";

// Avoid: Dynamic context generation
// $"parsing_{ruleNumber}"
```

## Common Anti-Patterns

### 1. Catching and Ignoring Errors
```csharp
// Bad: Losing error information
try { parser.ParseExpression(); }
catch { return defaultValue; }

// Good: Handle errors explicitly
var result = parser.ParseExpression();
return result.IsOk ? result.Ok.Value : defaultValue;
```

### 2. Generic Error Messages
```csharp
// Bad: Unhelpful messages
"Syntax error"

// Good: Specific expectations
"Expected ')' to close parentheses in expression"
```

### 3. No Error Recovery
```csharp
// Bad: Stop on first error
if (result.IsError) throw new Exception();

// Good: Collect errors and continue
errors.Add(result.Error);
parser.Synchronize();
```
