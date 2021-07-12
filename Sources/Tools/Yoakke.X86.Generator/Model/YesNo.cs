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
    /// Simple yes-no XML type.
    /// </summary>
    public enum YesNo
    {
        /// <summary>
        /// No.
        /// </summary>
        [XmlEnum(Name = "no")]
        No,

        /// <summary>
        /// Yes.
        /// </summary>
        [XmlEnum(Name = "yes")]
        Yes,
    }
}
