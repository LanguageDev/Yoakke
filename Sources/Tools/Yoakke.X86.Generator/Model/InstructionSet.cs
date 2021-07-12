// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Yoakke.X86.Generator.Model
{
    /// <summary>
    /// Represents a whole instruction set.
    /// </summary>
    public class InstructionSet
    {
        /// <summary>
        /// The name of the instruction set.
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The <see cref="Instruction"/>s in this <see cref="InstructionSet"/>.
        /// </summary>
        [XmlElement("Instruction")]
        public List<Instruction> Instructions { get; set; } = new();
    }
}
