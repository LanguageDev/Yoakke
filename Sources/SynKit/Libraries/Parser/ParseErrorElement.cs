// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Collections.Generic.Polyfill;

namespace Yoakke.SynKit.Parser;

/// <summary>
/// A single error case of parsing.
/// </summary>
public class ParseErrorElement
{
    private readonly ParseErrorExpectationSet expected;

    /// <summary>
    /// The expected possible inputs.
    /// </summary>
    public ISet<object> Expected => this.expected;

    /// <summary>
    /// The context in which the error occurred.
    /// </summary>
    public string Context { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParseErrorElement"/> class.
    /// </summary>
    /// <param name="expected">The expected input.</param>
    /// <param name="context">The context in which the error occurred.</param>
    public ParseErrorElement(object expected, string context)
    {
        this.expected = new ParseErrorExpectationSet(expected);
        this.Context = context;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParseErrorElement"/> class.
    /// </summary>
    /// <param name="expected">The expected possible inputs.</param>
    /// <param name="context">The context in which the error occurred.</param>
    public ParseErrorElement(ISet<object> expected, string context)
    {
        this.expected = new ParseErrorExpectationSet(expected);
        this.Context = context;
    }

    internal ParseErrorElement CreateMergedElement(ParseErrorElement parseErrorElement)
    {
        var newExpected = parseErrorElement.expected.Merge(this.expected);
        return new(newExpected, parseErrorElement.Context);
    }
}
