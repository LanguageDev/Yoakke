// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.C.Syntax;

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
        /// <summary>
        /// The <see cref="CToken"/> that might be a one-to-one expansion or a parameter reference.
        /// </summary>
        public CToken Token { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Literal"/> class.
        /// </summary>
        /// <param name="token">The <see cref="CToken"/> that might be a one-to-one expansion or a parameter reference.</param>
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
        /// <summary>
        /// The name of the macro argument to stringify.
        /// </summary>
        public string Argument { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Stringify"/> class.
        /// </summary>
        /// <param name="argument">The name of the macro argument to stringify.</param>
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
        /// <summary>
        /// The first element to paste.
        /// </summary>
        public MacroElement Left { get; }

        /// <summary>
        /// The second element to paste.
        /// </summary>
        public MacroElement Right { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Paste"/> class.
        /// </summary>
        /// <param name="left">The first element to paste.</param>
        /// <param name="right">The second element to paste.</param>
        public Paste(MacroElement left, MacroElement right)
        {
            this.Left = left;
            this.Right = right;
        }
    }
}
