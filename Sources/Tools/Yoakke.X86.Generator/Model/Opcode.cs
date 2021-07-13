// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Yoakke.X86.Generator.Model
{
    /// <summary>
    /// A single opcode.
    /// </summary>
    public class Opcode
    {
        /// <summary>
        /// The actual numeric opcode.
        /// </summary>
        [XmlIgnore]
        public byte Code { get; set; }

        /// <summary>
        /// The operand number that the last 3 bits of the opcode modifies.
        /// Null, if it does no such thing.
        /// </summary>
        [XmlIgnore]
        public int? Last3BitsEncodedOperand { get; set; }

        /// <summary>
        /// The hex string of this opcode.
        /// </summary>
        [XmlAttribute(AttributeName = "byte")]
        public string HexString
        {
            get => this.Code.ToString("x");
            set => this.Code = byte.Parse(value, NumberStyles.HexNumber);
        }

        /// <summary>
        /// Denotes that the last 3 bits of the operand encode an operand.
        /// Which operand it is, is encoded after a '#'.
        /// </summary>
        [XmlAttribute(AttributeName = "addend")]
        public string? RegisterCodeAtEnd
        {
            get => this.Last3BitsEncodedOperand is null ? null : $"#{this.Last3BitsEncodedOperand}";
            set => this.Last3BitsEncodedOperand = value is null ? null : int.Parse(value.Substring(1));
        }
    }
}
