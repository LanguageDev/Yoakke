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
    /// A sinlge form of an instruction, as an instruction has multiple encodings because of different forms/operand sizes.
    /// </summary>
    public class InstructionForm
    {
        /// <summary>
        /// The extensions this form belongs to.
        /// </summary>
        [XmlElement("ISA")]
        public List<IsaExtension> Extension { get; set; } = new();

        /// <summary>
        /// The <see cref="Operand"/>s this <see cref="InstructionForm"/> uses.
        /// </summary>
        [XmlElement("Operand")]
        public List<Operand> Operands { get; set; } = new();

        /// <summary>
        /// The <see cref="ImplicitOperand"/>s this <see cref="InstructionForm"/> uses.
        /// </summary>
        [XmlElement("ImplicitOperand")]
        public List<ImplicitOperand> ImplicitOperands { get; set; } = new();

        /// <summary>
        /// The different possible encodings for this <see cref="InstructionForm"/>.
        /// </summary>
        [XmlElement("Encoding")]
        public List<Encoding> Encodings { get; set; } = new();
    }
}
