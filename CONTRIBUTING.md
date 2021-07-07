# Contributing to the Yoakke Compiler Infrastructure

Firstly, thank you for considering contributing to the project! You are awesome! :tada:

This document describes how a contribution should generally happen. These are mostly guidelines, not rules, but they are intended to help the streamlined development of Yoakke. Feel free to propose changes to this document, if you see a way to improve it.

## Table of Content
 * [Have a question?](#have-a-question)
 * [Before you get started](#before-you-get-started)
 * [Ways to contribute](#ways-to-contribute)
	 * [Bug reports](#bug-reports)
 	 * [Feature requests](#feature-requests)
	 * [Module requests](#module-requests)
	 * [Code contribution](#code-contribution)
	 * [Other](#other)
 * [Styleguides](#style-guides)

## Have a question?
We are happy to answer any questions you may have. We do not have a separate communication channel - like Gitter or Discord - yet, but we are planning to open one in the future. If you have a question that you cannot find the answer to in the documentation - and you do not have my contact -, feel free to open an issue and we try to get to you as soon as possible. For smaller questions, please use [GitHub Discussions](https://github.com/LanguageDev/Yoakke/discussions).

## Before you get started
* Domain-specific knowledge: If you would like to be a contributor, it is a big advantage to have some experience with compilers, interpreters or formal languages in general. It is okay if you are still learning - maybe even in the middle of a compiler course -, this project could be a great learning opportunity!
 * C# and .NET knowledge: Because the infrastructure is written in C#, at least being familiar with the language would be great. It is not an issue if you do not know the shiny latest features, we will fill you in on those when when reviewing your pull-request. For documentation, you should familiarize yourself with the [standard XML documentation format](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/), as we use that to document our API.

## Ways to contribute
You can contribute in many ways, not just by writing code or fixing bugs. We accept help from all aspects around the project, and your help is greatly appreciated!

### Bug reports
If you find a bug in one of the existing features, feel free to open an issue about it, so we can fix it as soon as possible. Make sure to use the appropriate issue template!

### Feature requests
If a module does not have a feature you would like to see, you can open an issue to request it. Make sure to use the appropriate issue template! If your feature is so big that it probably belongs in its own module, open a new [module request](#module-requests) instead.

### Module requests
If you would like to see an entirely new module or library, please open an issue about that. One of our goals is provide all the necessary tooling for writing compilers, so there can never be enough modules! Make sure to use the appropriate issue template!

### Code contribution
If you would like to contribute with code, you could start out with the issues tagged with `help-wanted` or `good-first-issue`. That does not mean you can not take on any issue that is not being worked on!

If you finished a feature, open a Pull-Request referencing the issue it resolves - hopefully there is one. Make sure your contribution addresses a single issue - or maybe a few related or duplicate ones.

For bugs, it is highly advised to write a test first, if it does not exist yet. This will help us not to reintroduce the bug later!

Since testing ensures that the project will not suddenly collapse on our head, please test any new feature you develop! All tests are ran in the CI, so new tests will help other contributors too.

### Other
There are many other ways to contribute, hopefully someone can find joy in some of these other tasks:
 * Documentation and Tutorials
 * Configuration (StyleCop, SonarCloud, CI, ...)
 * Meta (like improving this document)

## Style-guides
To ensure that our repository will stay at a good quality, you should follow the below style-guides while contributing.

* Commit messages: There are no strict regulations at the moment, but please try to be somewhat descriptive about what you did. Try to be short and descriptive, you can detail your work in the PR or in the commit details.
* StyleCop: We have a StyleCop with a custom ruleset set up in the solution. Its setup is still in very early stage, but eventually we would like it to dictate the C# format we write in this repository. Feel free to contribute towards configuring StyleCop, if you know ways to improve its settings.
* Documentation: Use the [standard XML documentation format](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/), so we can generate our API documentation with it.
