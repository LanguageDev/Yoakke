// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Lsp.Model.LanguageFeatures
{
    /// <summary>
    /// A symbol kind.
    /// </summary>
    public enum SymbolKind
    {
        /// <summary>
        /// File symbol.
        /// </summary>
        File = 1,

        /// <summary>
        /// Module symbol.
        /// </summary>
        Module = 2,

        /// <summary>
        /// Namespace symbol.
        /// </summary>
        Namespace = 3,

        /// <summary>
        /// Package symbol.
        /// </summary>
        Package = 4,

        /// <summary>
        /// Class symbol.
        /// </summary>
        Class = 5,

        /// <summary>
        /// Method symbol.
        /// </summary>
        Method = 6,

        /// <summary>
        /// Property symbol.
        /// </summary>
        Property = 7,

        /// <summary>
        /// Field symbol.
        /// </summary>
        Field = 8,

        /// <summary>
        /// Constructor symbol.
        /// </summary>
        Constructor = 9,

        /// <summary>
        /// Enum symbol.
        /// </summary>
        Enum = 10,

        /// <summary>
        /// Interface symbol.
        /// </summary>
        Interface = 11,

        /// <summary>
        /// Function symbol.
        /// </summary>
        Function = 12,

        /// <summary>
        /// Variable symbol.
        /// </summary>
        Variable = 13,

        /// <summary>
        /// Constant symbol.
        /// </summary>
        Constant = 14,

        /// <summary>
        /// String symbol.
        /// </summary>
        String = 15,

        /// <summary>
        /// Number symbol.
        /// </summary>
        Number = 16,

        /// <summary>
        /// Boolean symbol.
        /// </summary>
        Boolean = 17,

        /// <summary>
        /// Array symbol.
        /// </summary>
        Array = 18,

        /// <summary>
        /// Object symbol.
        /// </summary>
        Object = 19,

        /// <summary>
        /// Key symbol.
        /// </summary>
        Key = 20,

        /// <summary>
        /// Null symbol.
        /// </summary>
        Null = 21,

        /// <summary>
        /// Enum member symbol.
        /// </summary>
        EnumMember = 22,

        /// <summary>
        /// Struct symbol.
        /// </summary>
        Struct = 23,

        /// <summary>
        /// Event symbol.
        /// </summary>
        Event = 24,

        /// <summary>
        /// Operator symbol.
        /// </summary>
        Operator = 25,

        /// <summary>
        /// Type parameter symbol.
        /// </summary>
        TypeParameter = 26,
    }
}
