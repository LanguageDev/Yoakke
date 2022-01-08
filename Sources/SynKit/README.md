## SynKit - The Syntax Toolkit libraries

SynKit is a subcomponent of Yoakke that collects modules and tools related to syntax. The aim is to cover every general need when it comes to analyzing grammars, generating lexers and parsers or even helping you developing your own syntax tooling.

### Components

- Lexing: Lexers can be defined completely declaratively, as regular expression annotations - inspired by [Logos](https://github.com/maciejhirsz/logos) - over token types. Do not worry, the regular expressions are compiled into _Deterministic Finite Automatas_, so the resulting code is a lot more efficient, than your average RegEx engine.
- Parsing: Parsers are defined using a BNF-like notation as annotations over methods that produce the syntax node for the appropriate rule - inspired by [rply](https://github.com/alex/rply/). The generated parser is a recursive descent one, but supports automatic transformation of direct left-recursion and precedence tables.
- Syntax trees: ASTs usually are a pain to implement because of all the crud that goes on with them in C#. With an annotation you can generate equality and hash implementations for AST nodes, or define visitors for them.
- Error reporting: Nice error messages on console are a pain. You have to align arrows, properly color text segments, number lines and so on. Fortunately, there is a component that provides you a nice fluent API - inspired by the beautiful [codespan](https://github.com/brendanzab/codespan) library - to generate a message, render it however you want to. You can even create a custom syntax-highlighter for source lines!

TODO: Update the library listing with the other modules too
