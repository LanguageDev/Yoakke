using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Yoakke.Reporting.Present;
using Yoakke.Text;

namespace Yoakke.Reporting.Tests
{
    [TestClass]
    public class TextPresenterTests
    {
        [TestMethod]
        public void BasicSingleAnnotation()
        {
            var src = new SourceFile("simple.txt",
@"line 1
prev line
this is a line of text
next line
some other line");
            var diag = new Diagnostic
            {
                Severity = Severity.Error,
                Code = "E0001",
                Message = "Some error message",
                Information =
                {
                    new SourceDiagnosticInfo
                    {
                        Location = new Location(src, new Range(new Position(line: 2, column: 10), 4)),
                        Message = "some annotation",
                        Severity = Severity.Error,
                    }
                },
            };
            var result = new StringWriter();
            var renderer = new TextDiagnosticPresenter(result);
            renderer.Present(diag);
            Assert.AreEqual(@"error[E0001]: Some error message
  ┌─ simple.txt:3:11
  │
2 │ prev line
3 │ this is a line of text
  │           ^^^^ some annotation
4 │ next line
  │
", result.ToString());
        }

#if false
        [TestMethod]
        public void TwoAnnotationsRightUnderEachother()
        {
            var src = new SourceFile("simple.txt",
@"line 1
prev line
this is a line of text
some other line
last line");
            var diag = new Diagnostic
            {
                Severity = Severity.Error,
                Code = "E0001",
                Message = "Some error message",
                Information =
                {
                    new SpannedDiagnosticInfo
                    {
                        Span = new Span(src, new Position(line: 2, column: 10), 4),
                        Message = "some annotation1",
                        Severity = Severity.Error,
                    },
                    new SpannedDiagnosticInfo
                    {
                        Span = new Span(src, new Position(line: 3, column: 5), 5),
                        Message = "some annotation2",
                    },
                },
            };
            var result = new StringWriter();
            var renderer = new TextDiagnosticRenderer(result);
            renderer.Render(diag);
            Assert.AreEqual(@"error[E0001]: Some error message
  ┌─ simple.txt:3:11
  │
2 │ prev line
3 │ this is a line of text
  │           ^^^^ some annotation1
4 │ some other line
  │      ----- some annotation2
5 │ last line
  │
", result.ToString());
        }

        [TestMethod]
        public void TwoAnnotationsClose()
        {
            var src = new SourceFile("simple.txt",
@"line 1
prev line
this is a line of text
next line
some other line
last line");
            var diag = new Diagnostic
            {
                Severity = Severity.Error,
                Code = "E0001",
                Message = "Some error message",
                Information =
                {
                    new SpannedDiagnosticInfo
                    {
                        Span = new Span(src, new Position(line: 2, column: 10), 4),
                        Message = "some annotation1",
                        Severity = Severity.Error,
                    },
                    new SpannedDiagnosticInfo
                    {
                        Span = new Span(src, new Position(line: 4, column: 5), 5),
                        Message = "some annotation2",
                    },
                },
            };
            var result = new StringWriter();
            var renderer = new TextDiagnosticRenderer(result);
            renderer.Render(diag);
            Assert.AreEqual(@"error[E0001]: Some error message
  ┌─ simple.txt:3:11
  │
2 │ prev line
3 │ this is a line of text
  │           ^^^^ some annotation1
4 │ next line
5 │ some other line
  │      ----- some annotation2
6 │ last line
  │
", result.ToString());
        }

        [TestMethod]
        public void TwoAnnotationsTouching()
        {
            var src = new SourceFile("simple.txt",
@"line 1
prev line
this is a line of text
next line
next line2
some other line
last line");
            var diag = new Diagnostic
            {
                Severity = Severity.Error,
                Code = "E0001",
                Message = "Some error message",
                Information =
                {
                    new SpannedDiagnosticInfo
                    {
                        Span = new Span(src, new Position(line: 2, column: 10), 4),
                        Message = "some annotation1",
                        Severity = Severity.Error,
                    },
                    new SpannedDiagnosticInfo
                    {
                        Span = new Span(src, new Position(line: 5, column: 5), 5),
                        Message = "some annotation2",
                    },
                },
            };
            var result = new StringWriter();
            var renderer = new TextDiagnosticRenderer(result);
            renderer.Render(diag);
            Assert.AreEqual(@"error[E0001]: Some error message
  ┌─ simple.txt:3:11
  │
2 │ prev line
3 │ this is a line of text
  │           ^^^^ some annotation1
4 │ next line
5 │ next line2
6 │ some other line
  │      ----- some annotation2
7 │ last line
  │
", result.ToString());
        }

        [TestMethod]
        public void TwoAnnotationsAlmostDotted()
        {
            var src = new SourceFile("simple.txt",
@"line 1
prev line
this is a line of text
next line
line between
next line2
some other line
last line");
            var diag = new Diagnostic
            {
                Severity = Severity.Error,
                Code = "E0001",
                Message = "Some error message",
                Information =
                {
                    new SpannedDiagnosticInfo
                    {
                        Span = new Span(src, new Position(line: 2, column: 10), 4),
                        Message = "some annotation1",
                        Severity = Severity.Error,
                    },
                    new SpannedDiagnosticInfo
                    {
                        Span = new Span(src, new Position(line: 6, column: 5), 5),
                        Message = "some annotation2",
                    },
                },
            };
            var result = new StringWriter();
            var renderer = new TextDiagnosticRenderer(result);
            renderer.Render(diag);
            Assert.AreEqual(@"error[E0001]: Some error message
  ┌─ simple.txt:3:11
  │
2 │ prev line
3 │ this is a line of text
  │           ^^^^ some annotation1
4 │ next line
5 │ line between
6 │ next line2
7 │ some other line
  │      ----- some annotation2
8 │ last line
  │
", result.ToString());
        }

        [TestMethod]
        public void TwoAnnotationsDotted()
        {
            var src = new SourceFile("simple.txt",
@"line 1
prev line
this is a line of text
next line
line between
line between2
next line2
some other line
last line");
            var diag = new Diagnostic
            {
                Severity = Severity.Error,
                Code = "E0001",
                Message = "Some error message",
                Information =
                {
                    new SpannedDiagnosticInfo
                    {
                        Span = new Span(src, new Position(line: 2, column: 10), 4),
                        Message = "some annotation1",
                        Severity = Severity.Error,
                    },
                    new SpannedDiagnosticInfo
                    {
                        Span = new Span(src, new Position(line: 7, column: 5), 5),
                        Message = "some annotation2",
                    },
                },
            };
            var result = new StringWriter();
            var renderer = new TextDiagnosticRenderer(result);
            renderer.Render(diag);
            Assert.AreEqual(@"error[E0001]: Some error message
  ┌─ simple.txt:3:11
  │
2 │ prev line
3 │ this is a line of text
  │           ^^^^ some annotation1
4 │ next line
  │ ...
7 │ next line2
8 │ some other line
  │      ----- some annotation2
9 │ last line
  │
", result.ToString());
        }
#endif
    }
}
