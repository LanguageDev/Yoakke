<p align="center">
	<a href="#"><img src="https://github.com/LanguageDev/Yoakke/blob/master/.github/resources/YoakkeLogoAnimated.svg?raw=true" height="200"></a>
</p>

<h2 align="center">The Yoakke Compiler Infrastructure</h2>
<p align="center">
	<i>Language- and Compiler development tools in .NET.</i>
</p>

![BuildSolution workflow](https://github.com/LanguageDev/Yoakke/actions/workflows/BuildSolution.yml/badge.svg)
![RunTests workflow](https://github.com/LanguageDev/Yoakke/actions/workflows/RunTests.yml/badge.svg)

## What is Yoakke?
Yoakke is a collection of libraries aimed at compiler- and language developers to help them in their development. Most common tasks - like lexing and parsing - are completely automated, but extensibility is kept in mind at every step. All components let you roll your custom solution, but are capable enough to fit most cases by default.

## Status warning
Yoakke is still early in development. You can play around with the libraries, but there may be bugs or missing features. Please open an issue if you find a bug, or miss an important feature. All contributions are welcome!

## What components are there?
 * Lexing: Lexers can be defined completely declaratively, as regular expression annotations - inspired by [Logos](https://github.com/maciejhirsz/logos) - over token types. Do not worry, the regular expressions are compiled into _Deterministic Finite Automatas_, so the resulting code is a lot more efficient, than your average RegEx engine.
 * Parsing: Parsers are defined using a BNF-like notation as annotations over methods that produce the syntax node for the appropriate rule - inspired by [rply](https://github.com/alex/rply/). The generated parser is a recursive descent one, but supports automatic transformation of direct left-recursion and precedence tables.
 * Syntax trees: ASTs usually are a pain to implement because of all the crud that goes on with them in C#. With an annotation you can generate equality and hash implementations for AST nodes, or define visitors for them.
 * Symbols: While lexical symbols are not too complex to implement, most compilers will need the same abstractions for it. Hence, this is also provided in a small library. 
 * Error reporting: Nice error messages on console are a pain. You have to align arrows, properly color text segments, number lines and so on. Fortunately, there is a component that provides you a nice fluent API - inspired by the beautiful [codespan](https://github.com/brendanzab/codespan) library - to generate a message, render it however you want to. You can even create a custom syntax-highlighter for source lines!
 * Language Server: [The Language Server Protocol](https://microsoft.github.io/language-server-protocol/) defines a generic way for language tools to communicate with text editors. A library already implements the protocol, you just need to provide your own handler for events!

## Library roadmap

This is a list of the libraries that are planned for implementation, along with their status:
-   Lexer: Considered mostly done
-   Parser: Considered mostly done
-   Error message reporting: Considered mostly done
-   AST generator, visitor: Considered mostly done
-   Lexical symbols: Considered mostly done
-   Language server protocol: Work in progress
-   C syntax (pre-processor, lexer, parser): Work in progress
-   Type-system: Planned
-   Virtual Machine: Planned
-   X86 Assembly: Planned
-   Debugger: Planned
-   Dependency system (incremental compilation): Planned
-   Debug adapter protocol: Planned

## Documentation

Documentation is very much work-in-progress, but you can always find the latest version in the [Documentation folder](https://github.com/LanguageDev/Yoakke/tree/master/Documentation). Feel free to open up Pull-Requests, if you see mistakes!

## Developer documentation

Make sure to read our [contributing document](https://github.com/LanguageDev/Yoakke/blob/master/CONTRIBUTING.md)! Some developer documents about describing the projects is - hopefully - coming soon aswell.

## Examples

Work in progress...

## Sample projects

Work in progress...

## Using Yoakke Badge

If you use Yoakke and would be willing to show it, here is a badge you can copy-paste into your readme:</br>
<a href="#" alt="Using Yoakke"><img src="https://raw.githubusercontent.com/LanguageDev/Yoakke/master/.github/resources/UsingYoakke.svg" title="Using Yoakke" alt="Using Yoakke"></a>
```html
<a href="https://github.com/LanguageDev/Yoakke" alt="Using Yoakke"><img src="https://raw.githubusercontent.com/LanguageDev/Yoakke/master/.github/resources/UsingYoakke.svg" title="Using Yoakke" alt="Using Yoakke"></a>
```
