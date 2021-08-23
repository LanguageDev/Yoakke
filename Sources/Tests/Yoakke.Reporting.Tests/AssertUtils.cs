// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yoakke.Reporting.Tests
{
    /// <summary>
    /// Assertion helpers.
    /// </summary>
    public static class AssertUtils
    {
        public static void AreEqualIgnoreNewlineEncoding(string expected, string got) =>
            Assert.AreEqual(NormalizeNewlines(expected), NormalizeNewlines(got));

        private static string NormalizeNewlines(string s) => s.Replace("\r\n", "\n").Replace('\r', '\n');
    }
}
