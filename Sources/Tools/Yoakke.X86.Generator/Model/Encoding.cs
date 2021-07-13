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
    /// A description on how to encode an <see cref="InstructionForm"/>.
    /// </summary>
    public class Encoding
    {
        // TODO: This is not correct. The opcode can be followed by a ModRM and then another Opcode
        // So we should only consume consecutive tags here, but I didn't figure out how to do that yet

        /// <summary>
        /// The main opcodes of the instruction.
        /// </summary>
        [XmlElement("Opcode")]
        public List<Opcode> Opcodes { get; set; } = new();
    }
}
