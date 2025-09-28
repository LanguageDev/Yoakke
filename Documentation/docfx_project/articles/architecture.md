# SynKit Architecture

This document provides an overview of the SynKit architecture and how its components work together.

## Core Design Principles

### 1. Source Generation Over Reflection
SynKit uses C# source generators to create lexers and parsers at compile time, providing:
- **Performance**: No runtime overhead from reflection
- **Type Safety**: Compile-time validation of grammar rules
- **Debugging**: Generated code can be stepped through

### 2. Attribute-Driven Configuration
Grammar rules and token definitions are specified using attributes:
- `[Token]` - Define literal tokens
- `[Regex]` - Define pattern-based tokens  
- `[Rule]` - Define parser production rules
- `[Left]`, `[Right]` - Specify operator precedence and associativity

### 3. Immutable Error Handling
Parse errors are immutable objects that can be combined and enriched:
- `ParseError` objects can be merged using the `|` operator
- Errors contain position information and expected elements
- Error contexts provide semantic information about what was being parsed

## Component Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│                 │    │                 │    │                 │
│   Text Layer    │    │  Lexer Layer    │    │  Parser Layer   │
│                 │    │                 │    │                 │
├─────────────────┤    ├─────────────────┤    ├─────────────────┤
│ • SourceFile    │───▶│ • ILexer<T>     │───▶│ • IParser       │
│ • Position      │    │ • Token<T>      │    │ • ParseResult<T>│
│ • Range         │    │ • TokenStream   │    │ • ParseError    │
│ • Location      │    │ • [Lexer] attr  │    │ • [Parser] attr │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         │              ┌─────────────────┐              │
         │              │                 │              │
         └──────────────▶│ Reporting Layer │◀─────────────┘
                        │                 │
                        ├─────────────────┤
                        │ • Diagnostics   │
                        │ • Severity      │
                        │ • Presenter     │
                        │ • Highlighter   │
                        └─────────────────┘
```

## Data Flow

### 1. Lexical Analysis
```
Source Text ──▶ ILexer<TToken> ──▶ Token Stream
     │               │                   │
     │               ▼                   ▼
     │         [Generated Lexer]    IToken<TToken>
     │               │                   │
     ▼               │                   │
SourceFile ──────────┘                   │
Position/Range                           │
     │                                   │
     └───────────────────────────────────┘
                Error Reporting
```

### 2. Syntactic Analysis  
```
Token Stream ──▶ IParser ──▶ ParseResult<T>
     │             │              │
     │             ▼              ├─ Ok<T>
     │      [Generated Parser]    │
     │             │              └─ Error
     │             ▼                  │
     └────▶ Error Recovery ──────────┘
              Synchronization
```

## Key Interfaces

### ILexer<TToken>
- `TToken Next()` - Advance and return next token
- `Position Position` - Current source position
- `bool IsEnd` - Check if input is exhausted

### IToken<TToken>
- `TToken Kind` - Token type/category
- `string Text` - Literal text content  
- `Range Range` - Source location
- `object? Value` - Parsed semantic value

### ParseResult<T>
- `bool IsOk/IsError` - Success/failure state
- `Ok<T> Ok` - Success result with value
- `ParseError Error` - Failure with detailed error info

### ParseError
- `IComparable Position` - Error location
- `object? Got` - What was actually found
- `IReadOnlyDictionary<string, ParseErrorElement> Elements` - Expected elements by context

## Generator Architecture

### Lexer Generation
1. **Analysis**: Scan `[Lexer]` attributed classes
2. **Token Discovery**: Find `[Token]` and `[Regex]` attributes on enum values
3. **Automata Construction**: Build finite automata for token recognition
4. **Code Generation**: Emit lexer implementation with optimized state machine

### Parser Generation  
1. **Grammar Analysis**: Extract `[Rule]` attributed methods
2. **Precedence Resolution**: Process `[Left]`, `[Right]`, `[Precedence]` attributes
3. **LR Table Construction**: Build parsing tables using LR algorithm
4. **Code Generation**: Emit parser with table-driven parsing logic

## Error Recovery

SynKit implements following error recovery:

### Lexer Errors
- Invalid characters produce `Error` tokens
- Lexer continues from next valid position
- Position information preserved for diagnostics

### Parser Errors
- ParseError objects accumulate expected elements
- Multiple alternative errors can be merged
- Context information helps with meaningful error messages
- Synchronization points allow recovery and continuation

## Thread Safety

- **Lexers**: Not thread-safe (maintain internal state)
- **Parsers**: Not thread-safe (maintain parse stacks)
- **Tokens**: Immutable and thread-safe
- **ParseResults**: Immutable and thread-safe
- **SourceFiles**: Thread-safe for reading
