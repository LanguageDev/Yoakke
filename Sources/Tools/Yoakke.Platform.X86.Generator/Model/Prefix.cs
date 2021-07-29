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

namespace Yoakke.Platform.X86.Generator.Model
{
    /// <summary>
    /// A prefix byte for an operand.
    /// </summary>
    public class Prefix
    {
        /// <summary>
        /// The actual numeric opcode.
        /// </summary>
        [XmlIgnore]
        public byte Code { get; set; }

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
        /// True, if the prefix is required.
        /// </summary>
        [XmlAttribute(AttributeName = "mandatory")]
        public bool Required { get; set; }
    }
}
