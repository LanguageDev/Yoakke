// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.C.Syntax
{
    /// <summary>
    /// An element in a macro body.
    /// </summary>
    public abstract class MacroElement
    {
        /// <summary>
        /// A literal <see cref="CToken"/> that could refer to an argument.
        /// </summary>
        public class Literal : MacroElement
        {
            public CToken Token { get; }

            public Literal(CToken token)
            {
                this.Token = token;
            }
        }

        /// <summary>
        /// An argument that is stringified.
        /// </summary>
        public class Stringify : MacroElement
        {
            public string Argument { get; }

            public Stringify(string argument)
            {
                this.Argument = argument;
            }
        }

        /// <summary>
        /// Two elements that are pasted together to become a single token.
        /// </summary>
        public class Paste : MacroElement
        {
            public MacroElement Left { get; }

            public MacroElement Right { get; }

            public Paste(MacroElement left, MacroElement right)
            {
                this.Left = left;
                this.Right = right;
            }
        }
    }
}
