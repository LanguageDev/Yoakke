// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Yoakke.Platform.X86.Generator.Model
{
    /// <summary>
    /// A single description of an ISA extension.
    /// </summary>
    public class IsaExtension
    {
        /// <summary>
        /// The name of the extension.
        /// </summary>
        [XmlAttribute(AttributeName = "id")]
        public string Name { get; set; } = string.Empty;
    }
}
