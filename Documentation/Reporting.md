
# Using the Reporting library
Creating nice diagnostic messages in console is hard. You have to print the appropriate source line, highlight the erroneous text, aligning multiple arrows in a line, ...
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
This does not print it anywhere though. That requires a renderer. Fortunately, we can just use the default-provided one, if we are fine with the look above:
```csharp
var presenter = new TextDiagnosticPresenter(Console.Error);
presenter.Present(diagnostic );
```
## Custom syntax highlighting
TBD