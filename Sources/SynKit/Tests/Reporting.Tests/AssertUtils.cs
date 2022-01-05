// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Xunit;

namespace Yoakke.SynKit.Reporting.Tests;

/// <summary>
/// Assertion helpers.
/// </summary>
public static class AssertUtils
{
    public static void AreEqualIgnoreNewlineEncoding(string expected, string got) =>
        Assert.Equal(NormalizeNewlines(expected), NormalizeNewlines(got));

    private static string NormalizeNewlines(string s) => s.Replace("\r\n", "\n").Replace('\r', '\n');
}
