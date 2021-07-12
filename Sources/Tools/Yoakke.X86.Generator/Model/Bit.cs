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
    /// A single 0 or 1.
    /// </summary>
    public enum Bit
    {
        /// <summary>
        /// Literal 0.
        /// </summary>
        [XmlEnum(Name = "0")]
        Zero,

        /// <summary>
        /// Literal 1.
        /// </summary>
        [XmlEnum(Name = "1")]
        One,
    }
}
