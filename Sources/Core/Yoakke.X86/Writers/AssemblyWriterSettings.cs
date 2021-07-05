// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.X86.Writers
{
    /// <summary>
    /// Settings for an <see cref="AssemblyWriter"/>.
    /// </summary>
    public class AssemblyWriterSettings
    {
        /// <summary>
        /// Returns a default instance with some reasonable settings.
        /// </summary>
        public static AssemblyWriterSettings Default => new();

        /// <summary>
        /// The <see cref="X86.SyntaxFlavor"/> to default to.
        /// </summary>
        public SyntaxFlavor SyntaxFlavor { get; set; } = SyntaxFlavor.Intel;

        /// <summary>
        /// The sequence to indent the instructions with.
        /// </summary>
        public string InstructionIndentation { get; set; } = "  ";

        /// <summary>
        /// True, if the segment selector should go insige the brackets.
        /// </summary>
        public bool SegmentSelectorInBrackets { get; set; } = false;

        /// <summary>
        /// True, if the instructions should be upper-cased.
        /// </summary>
        public bool InstructionsUpperCase { get; set; } = false;

        /// <summary>
        /// True, if the keywords should be upper-cased.
        /// </summary>
        public bool KeywordsUpperCase { get; set; } = false;

        /// <summary>
        /// True, if the registers should be upper-cased.
        /// </summary>
        public bool RegistersUpperCase { get; set; } = false;

        /// <summary>
        /// The prefix of line-comments.
        /// </summary>
        public string CommentPrefix { get; set; } = "; ";
    }
}
