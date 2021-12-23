// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Lsp.Model.LanguageFeatures;

/// <summary>
/// The kind of a completion entry.
/// </summary>
public enum CompletionItemKind
{
  /// <summary>
  /// Simple text completion.
  /// </summary>
  Text = 1,

  /// <summary>
  /// Method completion.
  /// </summary>
  Method = 2,

  /// <summary>
  /// Function completion.
  /// </summary>
  Function = 3,

  /// <summary>
  /// Constructor completion.
  /// </summary>
  Constructor = 4,

  /// <summary>
  /// Field completion.
  /// </summary>
  Field = 5,

  /// <summary>
  /// Variable completion.
  /// </summary>
  Variable = 6,

  /// <summary>
  /// Class completion.
  /// </summary>
  Class = 7,

  /// <summary>
  /// Interface completion.
  /// </summary>
  Interface = 8,

  /// <summary>
  /// Module completion.
  /// </summary>
  Module = 9,

  /// <summary>
  /// Property completion.
  /// </summary>
  Property = 10,

  /// <summary>
  /// Unit completion.
  /// </summary>
  Unit = 11,

  /// <summary>
  /// Value completion.
  /// </summary>
  Value = 12,

  /// <summary>
  /// Enum completion.
  /// </summary>
  Enum = 13,

  /// <summary>
  /// Keyword completion.
  /// </summary>
  Keyword = 14,

  /// <summary>
  /// Snippet completion.
  /// </summary>
  Snippet = 15,

  /// <summary>
  /// Color completion.
  /// </summary>
  Color = 16,

  /// <summary>
  /// File completion.
  /// </summary>
  File = 17,

  /// <summary>
  /// Reference completion.
  /// </summary>
  Reference = 18,

  /// <summary>
  /// Filter completion.
  /// </summary>
  Folder = 19,

  /// <summary>
  /// Enum member completion.
  /// </summary>
  EnumMember = 20,

  /// <summary>
  /// Constant completion.
  /// </summary>
  Constant = 21,

  /// <summary>
  /// Struct completion.
  /// </summary>
  Struct = 22,

  /// <summary>
  /// Event completion.
  /// </summary>
  Event = 23,

  /// <summary>
  /// Operator completion.
  /// </summary>
  Operator = 24,

  /// <summary>
  /// Type parameter completion.
  /// </summary>
  TypeParameter = 25,
}
