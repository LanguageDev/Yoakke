
# Using the Reporting library
Creating nice diagnostic messages in console is hard. You have to print the appropriate source line, highlight the erroneous text, aligning multiple arrows in a line, ...

## Basic usage
With the reporting library you can create nice error messages like this in an easy, declarative style:
```
error[E001]: type mismatch between int and bool in return value
   ┌─ main.prg:8:4
   │
 6 │ // This is the main
 7 │ fn main() -> int {
   │              --- defined to be int here
 8 │     println("Hello, World!");
 9 │     return true;
   │            ^^^^ coerced to bool here
10 │ }
   │
```
To describe the above diagnostic, you'd write the following code:
```csharp
var diagnostic = new Diagnostic()
    .WithSeverity(Severity.Error)
    .WithCode("E001")
    .WithMessage("type mismatch between int and bool in return value")
    .WithSourceInfo(new Location(sourceFile, returnValueRange, Severity.Error, "coerced to bool here"))
    .WithSourceInfo(new Location(sourceFile, returnTypeRange, Severity.Note, "defined to be int here"));
```
Here, `sourceFile` is an `ISourceFile`, `returnValueRange` and `returnTypeRange` are `Range`s. For more on these types, please refer to the `Yoakke.Text` library.

`Severity` is a basic structure that describes a given severity level with a name and priority value. There are some defined for you already.

The above code does not print the diagnostic anywhere though. That requires a renderer. Fortunately, we can just use the default-provided one, if we are fine with the look presented above:
```csharp
var presenter = new TextDiagnosticPresenter(Console.Error);
presenter.Present(diagnostic );
```

You can also write your custom presenter, you just need to implement the `IDiagnosticPresenter`.

## Styles
The primary way to customize the provided renderer is through the `Style` property, which holds a `DiagnosticStyle` type. `DiagnosticStyle` describes different formatting options to apply.

A short description of each:
 * `SeverityColors`: A dictionary that maps each severity level to a `ConsoleColor`.
 * `LineNumberColor`: The `ConsoleColor` to print line numbers with.
 * `DefaultColor`: The `ConsoleColor` to print anything that is not specified with.
 * `LineNumberPadding`: The padding character to use for line numbers if they are of different length.
 * `TabSize`: The width of a tab-character in spaces.
 * `SurroundingLines`: How many surrounding lines to print for the annotated line.
 * `ConnectUpLines`: How big of a gap can we still connect up with source lines instead of dotting it out.

## Custom syntax highlighting
You can provide a custom syntax highlighter for diagnostic messages with the `SyntaxHighlighter` property inside `IDiagnosticPresenter`s. They must implement the `ISyntaxHighlighter` interface.

The interface defines a method, that must return which tokens belong to what category in a given source line:
```csharp
public IReadOnlyList<ColoredToken> GetHighlightingForLine(ISourceFile sourceFile, int line);
```

**Note:** The exact color of the different tokens should not be determined by the `ISyntaxHighlighter` implementor. The `ISyntaxHighlighter` interface defines a property named `Style`, that is of the type `SyntaxHighlightStyle`. Similarly to `DiagnosticStyle`, this is where you would actually assign color to the tokens. So you can change your color schemes without affecting the syntax highlighter.