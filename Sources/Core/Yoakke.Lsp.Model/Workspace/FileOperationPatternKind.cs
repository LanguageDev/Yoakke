// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Runtime.Serialization;

namespace Yoakke.Lsp.Model.Workspace
{
    /// <summary>
    /// A pattern kind describing if a glob pattern matches a file a folder or
    /// both.
    /// </summary>
    [Since(3, 16, 0)]
    public enum FileOperationPatternKind
    {
        /// <summary>
        /// The pattern matches a file only.
        /// </summary>
        [EnumMember(Value = "file")]
        File,

        /// <summary>
        /// The pattern matches a folder only.
        /// </summary>
        [EnumMember(Value = "folder")]
        Folder,
    }
}
