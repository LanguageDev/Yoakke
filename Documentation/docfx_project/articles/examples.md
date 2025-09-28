# Examples Overview

This directory contains comprehensive examples showing how to use SynKit components.

## Available Examples

### Basic Examples
- [Simple Lexer](simple-lexer.md) - Basic token recognition
- [Expression Parser](expression-parser.md) - Mathematical expressions with precedence
- [Calculator](calculator.md) - Complete calculator with error handling

### Advanced Examples  
- [JSON Parser](json-parser.md) - Full JSON parsing with proper error reporting
- [C Preprocessor](c-preprocessor.md) - Using the built-in C syntax components
- [Custom Language](custom-language.md) - Building a small domain-specific language

### Integration Examples
- [Error Reporting](error-reporting.md) - Rich diagnostic output with syntax highlighting
- [AST Generation](ast-generation.md) - Building abstract syntax trees
- [Code Generation](code-generation.md) - Generating code from parsed input

## Running Examples

All examples are located in the `SynKit/Examples/` directory and can be run with:

```bash
cd SynKit/Examples/Lexer.Sample
dotnet run
```

## Example Structure

Each example follows this structure:
- **README.md** - Overview and usage instructions
- **Program.cs** - Main entry point
- **TokenType.cs** - Token definitions (if applicable)
- **Parser.cs** - Parser implementation (if applicable)
- **sample-input.txt** - Example input files

## Learning Path

1. **Start with [Simple Lexer](simple-lexer.md)** - Learn basic tokenization
2. **Move to [Expression Parser](expression-parser.md)** - Understand parsing concepts
3. **Try [Calculator](calculator.md)** - See complete error handling
4. **Explore [JSON Parser](json-parser.md)** - Study a real-world grammar
5. **Build [Custom Language](custom-language.md)** - Apply concepts to your domain
