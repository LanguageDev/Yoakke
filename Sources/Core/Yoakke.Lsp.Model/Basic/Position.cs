// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Newtonsoft.Json;

namespace Yoakke.Lsp.Model.Basic
{
    /// <summary>
    /// See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#position.
    /// </summary>
    public readonly struct Position
    {
        /// <summary>
        /// Line position in a document (zero-based).
        /// </summary>
        [JsonProperty("line")]
        public readonly uint Line;

        /// <summary>
        /// Character offset on a line in a document (zero-based). Assuming that
        /// the line is represented as a string, the `character` value represents
        /// the gap between the `character` and `character + 1`.
        ///
        /// If the character value is greater than the line length it defaults back
        /// to the line length.
        /// </summary>
        [JsonProperty("character")]
        public readonly uint Character;

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> struct.
        /// </summary>
        /// <param name="line">The line index.</param>
        /// <param name="character">The column index.</param>
        public Position(uint line, uint character)
        {
            this.Line = line;
            this.Character = character;
        }
    }
}
