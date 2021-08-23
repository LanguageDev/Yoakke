// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Xml.Serialization;

namespace Yoakke.Platform.X86.Generator.Model
{
    /// <summary>
    /// A single x86 instruction.
    /// </summary>
    public class Instruction
    {
        /// <summary>
        /// The name of the <see cref="Instruction"/>.
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// A summary text of the <see cref="Instruction"/>.
        /// </summary>
        [XmlAttribute(AttributeName = "summary")]
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// All the different forms (loosely overloads) of this <see cref="Instruction"/>.
        /// </summary>
        [XmlElement("InstructionForm")]
        public List<InstructionForm> Forms { get; set; } = new();
    }
}
