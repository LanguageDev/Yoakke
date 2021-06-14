// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Lsp.Model
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum |
        AttributeTargets.Property | AttributeTargets.Field,
        AllowMultiple = false,
        Inherited = true)]
    public class SinceAttribute : Attribute
    {
        public int Major { get; set; }

        public int Minor { get; set; }

        public int Patch { get; set; }

        public SinceAttribute(int major = 1, int minor = 0, int patch = 0)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
        }
    }
}
