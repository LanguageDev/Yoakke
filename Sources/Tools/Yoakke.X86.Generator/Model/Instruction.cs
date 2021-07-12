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
    }
}
