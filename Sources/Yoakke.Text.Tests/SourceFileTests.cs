// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yoakke.Text.Tests
{
    [TestClass]
    public class SourceFileTests
    {
        [TestMethod]
        public void GetAllLinesOfString()
        {
            var sourceFile = new SourceFile(
                "a.txt",
@"abc
xyz
qwe");
            Assert.AreEqual(3, sourceFile.AvailableLines);
            Assert.AreEqual("abc", sourceFile.GetLine(0).TrimEnd());
            Assert.AreEqual("xyz", sourceFile.GetLine(1).TrimEnd());
            Assert.AreEqual("qwe", sourceFile.GetLine(2).TrimEnd());
            Assert.AreEqual("xyz", sourceFile.GetLine(1).TrimEnd());
            Assert.AreEqual("abc", sourceFile.GetLine(0).TrimEnd());
        }

        [TestMethod]
        public void GetAllLinesOfStringReader()
        {
            var sourceFile = new SourceFile(
                "a.txt",
new StringReader(@"abc
xyz
qwe"));
            Assert.AreEqual(0, sourceFile.AvailableLines);
            Assert.AreEqual("abc", sourceFile.GetLine(0).TrimEnd());
            Assert.AreEqual(1, sourceFile.AvailableLines);
            Assert.AreEqual("xyz", sourceFile.GetLine(1).TrimEnd());
            Assert.AreEqual(2, sourceFile.AvailableLines);
            Assert.AreEqual("qwe", sourceFile.GetLine(2).TrimEnd());
            Assert.AreEqual(3, sourceFile.AvailableLines);
            Assert.AreEqual("xyz", sourceFile.GetLine(1).TrimEnd());
            Assert.AreEqual(3, sourceFile.AvailableLines);
            Assert.AreEqual("abc", sourceFile.GetLine(0).TrimEnd());
            Assert.AreEqual(3, sourceFile.AvailableLines);
        }

        [TestMethod]
        public void ReaderPositionUnaffectedByGetLine()
        {
            var sourceFile = new SourceFile(
                "a.txt",
new StringReader(@"abc
xyz
qwe"));
            sourceFile.GetLine(2);
            Assert.AreEqual("abc", sourceFile.ReadLine()?.TrimEnd());
        }
    }
}
