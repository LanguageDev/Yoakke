// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.Lr
{
    /// <summary>
    /// An accept action, representing that the input is being accepted.
    /// </summary>
    public sealed record Accept : Action
    {
        /// <summary>
        /// The singleton instance to use.
        /// </summary>
        public static Accept Instance { get; } = new();

        private Accept()
        {
        }

        /// <inheritdoc/>
        public override string ToString() => "accept";
    }
}
