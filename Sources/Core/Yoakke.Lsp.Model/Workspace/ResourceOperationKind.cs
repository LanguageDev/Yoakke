// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Runtime.Serialization;

namespace Yoakke.Lsp.Model.Workspace
{
    /// <summary>
    /// The kind of resource operations supported by the client.
    /// </summary>
    public enum ResourceOperationKind
    {
        /// <summary>
        /// Supports creating new files and folders.
        /// </summary>
        [EnumMember(Value = "create")]
        Create,

        /// <summary>
        /// Supports renaming existing files and folders.
        /// </summary>
        [EnumMember(Value = "rename")]
        Rename,

        /// <summary>
        /// Supports deleting existing files and folders.
        /// </summary>
        [EnumMember(Value = "delete")]
        Delete,
    }
}
