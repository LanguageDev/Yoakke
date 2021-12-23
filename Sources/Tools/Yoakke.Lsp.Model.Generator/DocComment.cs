// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Yoakke.Lsp.Model.Generator;

internal class DocComment
{
  public string? Summary { get; set; }

  public string? Deprecation { get; set; }

  public string? Since { get; set; }

  public static DocComment FromComment(string docComment)
  {
    var lines = DocCommentToLines(docComment);
    var tags = ParseTags(lines);
    var summary = lines.Count == 0 ? null : string.Join('\n', lines);
    string? deprecation = null;
    string? since = null;
    if (tags.Remove("deprecated", out var deprecationLines))
    {
      deprecation = string.Join('\n', deprecationLines);
    }
    if (tags.Remove("since", out var sinceLines))
    {
      since = string.Join('\n', sinceLines);
    }
    if (tags.Count > 0)
    {
      throw new NotSupportedException($"unknown doc tag {tags.First().Key}");
    }
    return new DocComment
    {
      Summary = summary,
      Deprecation = deprecation,
      Since = since,
    };
  }

  private static Dictionary<string, List<string>> ParseTags(List<string> lines)
  {
    var result = new Dictionary<string, List<string>>();
    for (var i = 0; i < lines.Count;)
    {
      if (lines[i].StartsWith('@'))
      {
        var (name, tlines) = ParseTag(lines, i);
        result.Add(name, tlines);
      }
      else
      {
        ++i;
      }
    }
    return result;
  }

  private static (string Name, List<string> Lines) ParseTag(List<string> origLines, int lineIndex)
  {
    var lines = new List<string>();
    // First line determines tag
    if (origLines[lineIndex][0] != '@') throw new InvalidOperationException();
    var tag = new string(origLines[lineIndex].Skip(1).TakeWhile(char.IsLetterOrDigit).ToArray());
    // First line gets consumed, but cut out the tag
    lines.Add(origLines[lineIndex][(tag.Length + 1)..].Trim());
    origLines.RemoveAt(lineIndex);
    // Get all consecutive lines that don't start with a tag or not separated with empty line
    while (lineIndex < origLines.Count && !string.IsNullOrWhiteSpace(origLines[lineIndex]) && origLines[lineIndex][0] != '@')
    {
      lines.Add(origLines[lineIndex]);
      origLines.RemoveAt(lineIndex);
    }
    PruneLineList(origLines);
    return (tag, lines);
  }

  private static List<string> DocCommentToLines(string docComment)
  {
    // First we remove leading /* and trailing */
    var result = docComment[2..^2];
    // Divide up into lines
    var lines = new List<string>();
    var lineReader = new StringReader(result);
    while (true)
    {
      var line = lineReader.ReadLine();
      if (line == null) break;
      lines.Add(line);
    }
    // Now remove any prefix that is spaces and a star after
    for (var i = 0; i < lines.Count; ++i)
    {
      var lineChars = lines[i].ToCharArray()
          .SkipWhile(char.IsWhiteSpace)
          .SkipWhile(ch => ch == '*')
          .ToArray();
      lines[i] = new string(lineChars).Trim();
    }
    // Remove empty lines from front and back
    PruneLineList(lines);
    // We are done
    return lines;
  }

  private static void PruneLineList(List<string> lines)
  {
    while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[0])) lines.RemoveAt(0);
    while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[^1])) lines.RemoveAt(lines.Count - 1);
  }
}
