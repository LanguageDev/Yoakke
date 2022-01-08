<p align="center">
	<a href="#"><img src="https://github.com/LanguageDev/Yoakke/blob/master/.github/resources/YoakkeLogoAnimated.svg?raw=true" height="200"></a>
</p>

<h2 align="center">The Yoakke Compiler Infrastructure</h2>
<p align="center">
	<i>Language- and Compiler development tools in .NET.</i>
</p>

<!-- Badges -->
<a href="https://github.com/LanguageDev/Yoakke/actions"><img src="https://github.com/LanguageDev/Yoakke/actions/workflows/BuildSolution.yml/badge.svg" title="Go To GitHub Actions"></a>
<a href="https://github.com/LanguageDev/Yoakke/actions"><img src="https://github.com/LanguageDev/Yoakke/actions/workflows/RunTests.yml/badge.svg" title="Go To GitHub Actions"></a>
<a href="https://docs.microsoft.com/en-us/dotnet/csharp/"><img src="https://img.shields.io/badge/language-C%23-%23178600" title="Go To C# Documentation"></a>
<a href="https://www.nuget.org/packages?q=Yoakke+owner%3ALanguageDev"><img src="https://img.shields.io/nuget/v/Yoakke.Lexer.svg?logo=nuget" title="Go To Nuget Package" /></a>
<a href="https://languagedev.github.io/Yoakke"><img src="https://github.com/LanguageDev/Yoakke/blob/master/.github/resources/DocfxBadge.svg?raw=true" title="Go To Docfx Documentation"></a><!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-7-orange.svg)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->

## What is Yoakke?

Yoakke is a collection of libraries aimed at compiler- and language developers to help them in their development. Most common tasks - like lexing and parsing - are completely automated, but extensibility is kept in mind at every step. All components let you roll your custom solution, but are capable enough to fit most cases by default.

## Status warning

Yoakke is still early in development. You can play around with the libraries, but there may be bugs or missing features. Please open an issue if you find a bug, or miss an important feature. All contributions are welcome!

If you want to try out the latest features, there are nightly releases on [NuGet](https://www.nuget.org/packages?q=Yoakke+owner%3ALanguageDev)!

## A note for Visual Studio users

If you are experiencing build issues when using Visual Studio around Source Generators, please try enabling the [option described here](https://languagedev.github.io/Yoakke/articles/fixing-vs-issues.html).

## What components are there?

The most usable component is probably the syntax toolkit, called [SynKit](https://github.com/LanguageDev/Yoakke/tree/master/Sources/SynKit). There are many more components planned, see the next section for the roadmap.

## Library roadmap

This is a list of the libraries that are planned for implementation, along with their status:

- SynKit (🆗): A syntax toolkit to cover most syntax needs
  - Lexer (🆗)
  - Parser (🆗)
  - Error reporting (🆗)
- Language Server Protocol (📝)
- Custom IR and VM (📝)
- Native backends (📝)
- C front-end (🚧)

Legend
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
<a href="https://github.com/LanguageDev/Yoakke" alt="Using Yoakke"><img src="https://raw.githubusercontent.com/LanguageDev/Yoakke/master/.github/resources/UsingYoakke.svg" title="Using Yoakke" alt="Using Yoakke"/></a>
```

## Contributors ✨

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><a href="https://github.com/LPeter1997"><img src="https://avatars.githubusercontent.com/u/7904867?v=4?s=100" width="100px;" alt=""/><br /><sub><b>LPeter1997</b></sub></a><br /><a href="#projectManagement-LPeter1997" title="Project Management">📆</a> <a href="#ideas-LPeter1997" title="Ideas, Planning, & Feedback">🤔</a> <a href="https://github.com/LanguageDev/Yoakke/commits?author=LPeter1997" title="Code">💻</a> <a href="https://github.com/LanguageDev/Yoakke/commits?author=LPeter1997" title="Documentation">📖</a> <a href="#maintenance-LPeter1997" title="Maintenance">🚧</a> <a href="#infra-LPeter1997" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/LanguageDev/Yoakke/pulls?q=is%3Apr+reviewed-by%3ALPeter1997" title="Reviewed Pull Requests">👀</a> <a href="https://github.com/LanguageDev/Yoakke/commits?author=LPeter1997" title="Tests">⚠️</a></td>
    <td align="center"><a href="https://angouri.org/"><img src="https://avatars.githubusercontent.com/u/31178401?v=4?s=100" width="100px;" alt=""/><br /><sub><b>WhiteBlackGoose</b></sub></a><br /><a href="#infra-WhiteBlackGoose" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/LanguageDev/Yoakke/commits?author=WhiteBlackGoose" title="Documentation">📖</a></td>
    <td align="center"><a href="https://github.com/ZacharyPatten"><img src="https://avatars.githubusercontent.com/u/3385986?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Zachary Patten</b></sub></a><br /><a href="#design-ZacharyPatten" title="Design">🎨</a> <a href="#infra-ZacharyPatten" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a></td>
    <td align="center"><a href="http://skneko.moe/"><img src="https://avatars.githubusercontent.com/u/13376606?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Neko</b></sub></a><br /><a href="#ideas-skneko" title="Ideas, Planning, & Feedback">🤔</a> <a href="https://github.com/LanguageDev/Yoakke/commits?author=skneko" title="Code">💻</a> <a href="https://github.com/LanguageDev/Yoakke/commits?author=skneko" title="Tests">⚠️</a> <a href="#userTesting-skneko" title="User Testing">📓</a> <a href="https://github.com/LanguageDev/Yoakke/issues?q=author%3Askneko" title="Bug reports">🐛</a> <a href="#question-skneko" title="Answering Questions">💬</a></td>
    <td align="center"><a href="https://github.com/hantatsang"><img src="https://avatars.githubusercontent.com/u/11912225?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Sang</b></sub></a><br /><a href="https://github.com/LanguageDev/Yoakke/commits?author=hantatsang" title="Code">💻</a></td>
    <td align="center"><a href="https://github.com/lucyelle"><img src="https://avatars.githubusercontent.com/u/35396043?v=4?s=100" width="100px;" alt=""/><br /><sub><b>lucyelle</b></sub></a><br /><a href="https://github.com/LanguageDev/Yoakke/commits?author=lucyelle" title="Documentation">📖</a></td>
    <td align="center"><a href="https://github.com/FP5i"><img src="https://avatars.githubusercontent.com/u/11299637?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Péter Fónad</b></sub></a><br /><a href="https://github.com/LanguageDev/Yoakke/commits?author=FP5i" title="Documentation">📖</a> <a href="#infra-FP5i" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a></td>
  </tr>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!
