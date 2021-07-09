<p align="center">
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-6-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->
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

- Lexing: Lexers can be defined completely declaratively, as regular expression annotations - inspired by [Logos](https://github.com/maciejhirsz/logos) - over token types. Do not worry, the regular expressions are compiled into _Deterministic Finite Automatas_, so the resulting code is a lot more efficient, than your average RegEx engine.
- Parsing: Parsers are defined using a BNF-like notation as annotations over methods that produce the syntax node for the appropriate rule - inspired by [rply](https://github.com/alex/rply/). The generated parser is a recursive descent one, but supports automatic transformation of direct left-recursion and precedence tables.
- Syntax trees: ASTs usually are a pain to implement because of all the crud that goes on with them in C#. With an annotation you can generate equality and hash implementations for AST nodes, or define visitors for them.
- Symbols: While lexical symbols are not too complex to implement, most compilers will need the same abstractions for it. Hence, this is also provided in a small library.
- Error reporting: Nice error messages on console are a pain. You have to align arrows, properly color text segments, number lines and so on. Fortunately, there is a component that provides you a nice fluent API - inspired by the beautiful [codespan](https://github.com/brendanzab/codespan) library - to generate a message, render it however you want to. You can even create a custom syntax-highlighter for source lines!
- Language Server: [The Language Server Protocol](https://microsoft.github.io/language-server-protocol/) defines a generic way for language tools to communicate with text editors. A library already implements the protocol, you just need to provide your own handler for events!

## Library roadmap

This is a list of the libraries that are planned for implementation, along with their status:

| Library                                     | Status | Nuget ![Nuget](https://upload.wikimedia.org/wikipedia/commons/thumb/2/25/NuGet_project_logo.svg/16px-NuGet_project_logo.svg.png)  | Notes |
| ------------------------------------------- | :------: | ---- | ----- |
| Lexer                                       | 🆗      | [![Nuget version (Yoakke.Lexer)](https://img.shields.io/nuget/v/Yoakke.Lexer.svg?logo=nuget&style=flat-square&logoColor=white&labelColor=004880&logoWidth=18&label=Yoakke.Lexer)](https://www.nuget.org/packages/Yoakke.Lexer) <br /> [![Nuget version (Yoakke.Lexer.Generator)](https://img.shields.io/nuget/v/Yoakke.Lexer.Generator.svg?logo=nuget&style=flat-square&logoColor=white&labelColor=004880&logoWidth=18&label=Yoakke.Lexer.Generator)](https://www.nuget.org/packages/Yoakke.Lexer.Generator) |       |
| Parser                                      | 🆗      | [![Nuget version (Yoakke.Parser)](https://img.shields.io/nuget/v/Yoakke.Parser.svg?logo=nuget&style=flat-square&logoColor=white&labelColor=004880&logoWidth=18&label=Yoakke.Parser)](https://www.nuget.org/packages/Yoakke.Parser)<br /> [![Nuget version (Yoakke.Parser.Generator)](https://img.shields.io/nuget/v/Yoakke.Parser.Generator.svg?logo=nuget&style=flat-square&logoColor=white&labelColor=004880&logoWidth=18&label=Yoakke.Parser.Generator)](https://www.nuget.org/packages/Yoakke.Parser.Generator) |       |
| Error message reporting                     | 🆗      | [![Nuget version (Yoakke.Reporting)](https://img.shields.io/nuget/v/Yoakke.Reporting.svg?logo=nuget&style=flat-square&logoColor=white&labelColor=004880&logoWidth=18&label=Yoakke.Reporting)](https://www.nuget.org/packages/Yoakke.Reporting) |       |
| AST generator, visitor                      | 🆗      | [![Nuget version (Yoakke.Ast)](https://img.shields.io/nuget/v/Yoakke.Ast.svg?logo=nuget&style=flat-square&logoColor=white&labelColor=004880&logoWidth=18&label=Yoakke.Ast)](https://www.nuget.org/packages/Yoakke.Ast) <br /> [![Nuget version (Yoakke.Ast.Generator)](https://img.shields.io/nuget/v/Yoakke.Ast.Generator.svg?logo=nuget&style=flat-square&logoColor=white&labelColor=004880&logoWidth=18&label=Yoakke.Ast.Generator)](https://www.nuget.org/packages/Yoakke.Ast.Generator) |       |
| Lexical symbols                             | 🆗      | [![Nuget version (Yoakke.Symbols)](https://img.shields.io/nuget/v/Yoakke.Symbols.svg?logo=nuget&style=flat-square&logoColor=white&labelColor=004880&logoWidth=18&label=Yoakke.Symbols)](https://www.nuget.org/packages/Yoakke.Symbols) |       |
| Language server protocol                    | 🚧      | [![Nuget version (Yoakke.Lsp.Model)](https://img.shields.io/nuget/v/Yoakke.Lsp.Model.svg?logo=nuget&style=flat-square&logoColor=white&labelColor=004880&logoWidth=18&label=Yoakke.Lsp.Model)](https://www.nuget.org/packages/Yoakke.Lsp.Model) <br /> [![Nuget version (Yoakke.Lsp.Server)](https://img.shields.io/nuget/v/Yoakke.Lsp.Server.svg?logo=nuget&style=flat-square&logoColor=white&labelColor=004880&logoWidth=18&label=Yoakke.Lsp.Server)](https://www.nuget.org/packages/Yoakke.Lsp.Server) |       |
| C syntax <br /> (pre-processor, lexer, parser)     | 🚧      | [![Nuget version (Yoakke.C.Syntax)](https://img.shields.io/nuget/v/Yoakke.C.Syntax.svg?logo=nuget&style=flat-square&logoColor=white&labelColor=004880&logoWidth=18&label=Yoakke.C.Syntax)](https://www.nuget.org/packages/Yoakke.C.Syntax) |       |
| Type-system                                 | 📝      |      |       |
| Virtual Machine                             | 📝      |      |       |
| X86 Assembly                                | 🚧      | [![Nuget version (Yoakke.X86 )](https://img.shields.io/nuget/v/Yoakke.X86.svg?logo=nuget&style=flat-square&logoColor=white&labelColor=004880&logoWidth=18&label=Yoakke.X86)](https://www.nuget.org/packages/Yoakke.X86)|       |
| Debugger                                    | 📝      |      |       |
| Dependency system <br /> (incremental compilation) | 📝      |      |       |
| Debug adapter protocol                      | 📝      |      |       |

- Done: ✅
- Consider mostly done: 🆗
- Work in progress: 🚧
- Planned: 📝

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
<a href="https://github.com/LanguageDev/Yoakke" alt="Using Yoakke"
  ><img
    src="https://raw.githubusercontent.com/LanguageDev/Yoakke/master/.github/resources/UsingYoakke.svg"
    title="Using Yoakke"
    alt="Using Yoakke"
/></a>
```

## Contributors ✨

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><a href="https://github.com/LPeter1997"><img src="https://avatars.githubusercontent.com/u/7904867?v=4?s=100" width="100px;" alt=""/><br /><sub><b>LPeter1997</b></sub></a><br /><a href="https://github.com/LanguageDev/Yoakke/commits?author=LPeter1997" title="Code">💻</a> <a href="https://github.com/LanguageDev/Yoakke/commits?author=LPeter1997" title="Documentation">📖</a> <a href="#example-LPeter1997" title="Examples">💡</a> <a href="#ideas-LPeter1997" title="Ideas, Planning, & Feedback">🤔</a> <a href="#maintenance-LPeter1997" title="Maintenance">🚧</a> <a href="#infra-LPeter1997" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/LanguageDev/Yoakke/pulls?q=is%3Apr+reviewed-by%3ALPeter1997" title="Reviewed Pull Requests">👀</a> <a href="https://github.com/LanguageDev/Yoakke/commits?author=LPeter1997" title="Tests">⚠️</a> <a href="#research-LPeter1997" title="Research">🔬</a> <a href="#projectManagement-LPeter1997" title="Project Management">📆</a></td>
    <td align="center"><a href="https://github.com/ZacharyPatten"><img src="https://avatars.githubusercontent.com/u/3385986?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Zachary Patten</b></sub></a><br /><a href="#design-ZacharyPatten" title="Design">🎨</a></td>
    <td align="center"><a href="http://skneko.moe/"><img src="https://avatars.githubusercontent.com/u/13376606?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Neko</b></sub></a><br /><a href="#ideas-skneko" title="Ideas, Planning, & Feedback">🤔</a> <a href="https://github.com/LanguageDev/Yoakke/commits?author=skneko" title="Code">💻</a></td>
    <td align="center"><a href="https://github.com/hantatsang"><img src="https://avatars.githubusercontent.com/u/11912225?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Sang</b></sub></a><br /><a href="https://github.com/LanguageDev/Yoakke/commits?author=hantatsang" title="Code">💻</a></td>
    <td align="center"><a href="https://github.com/lucyelle"><img src="https://avatars.githubusercontent.com/u/35396043?v=4?s=100" width="100px;" alt=""/><br /><sub><b>lucyelle</b></sub></a><br /><a href="https://github.com/LanguageDev/Yoakke/commits?author=lucyelle" title="Documentation">📖</a></td>
    <td align="center"><a href="https://angouri.org/"><img src="https://avatars.githubusercontent.com/u/31178401?v=4?s=100" width="100px;" alt=""/><br /><sub><b>WhiteBlackGoose</b></sub></a><br /><a href="#infra-WhiteBlackGoose" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/LanguageDev/Yoakke/commits?author=WhiteBlackGoose" title="Documentation">📖</a></td>
  </tr>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!