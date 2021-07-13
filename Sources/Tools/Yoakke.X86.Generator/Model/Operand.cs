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
    /// A single instruction operand.
    /// </summary>
    public class Operand
    {
        /// <summary>
        /// The operand type.
        /// </summary>
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// True, if this is an input operand.
        /// </summary>
        [XmlAttribute(AttributeName = "input")]
        public bool IsInput { get; set; }

        /// <summary>
        /// True, if this is an output operand.
        /// </summary>
        [XmlAttribute(AttributeName = "output")]
        public bool IsOutput { get; set; }

        /// <summary>
        /// The extended size of the operand, in case it's being extended with the instruction.
        /// </summary>
        [XmlIgnore]
        public int? ExtendedSize { get; set; }

        /// <summary>
        /// Just for the XML writer to function properly.
        /// </summary>
        [XmlAttribute(AttributeName = "extended-size")]
        public int ExtendedSizeXml
        {
            set => this.ExtendedSize = value;
        }
    }
}
