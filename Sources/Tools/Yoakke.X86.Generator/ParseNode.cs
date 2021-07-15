// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Yoakke.X86.Generator.Model;

namespace Yoakke.X86.Generator
{
    /// <summary>
    /// A single node in the tree where we build the parse switch statement.
    /// </summary>
    public class ParseNode
    {
        /// <summary>
        /// The subnodes when we match a certain byte.
        /// </summary>
        public IDictionary<byte, ParseNode> Subnodes { get; } = new Dictionary<byte, ParseNode>();

        /// <summary>
        /// The encodings for this node.
        /// </summary>
        public IList<Encoding> Encodings { get; } = new List<Encoding>();

        /// <summary>
        /// True, if the last 3 bits encode a register.
        /// </summary>
        public bool Last3BitsEncodeRegister { get; set; }

        /// <summary>
        /// Adds an <see cref="Encoding"/> to this tree.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to add.</param>
        public void AddEncoding(Encoding encoding)
        {
            var root = this;
            foreach (var opcode in encoding.Opcodes)
            {
                if (!root.Subnodes.TryGetValue(opcode.Code, out var nextRoot))
                {
                    nextRoot = new ParseNode();
                    root.Subnodes.Add(opcode.Code, nextRoot);
                }
                root = nextRoot;
            }

            if (encoding.Opcodes.Count > 0 && encoding.Opcodes[encoding.Opcodes.Count - 1].RegisterCodeAtEnd is not null)
            {
                Debug.Assert(root.Subnodes.Count == 0, "Can't handle subnodes with last bits encoding");
                Debug.Assert(
                    root.Encodings.All(e => e.Opcodes[e.Opcodes.Count - 1].RegisterCodeAtEnd is not null),
                    "All encodings must have the last 3 bits encoding the register, if one does it");
                root.Last3BitsEncodeRegister = true;
            }
            root.Encodings.Add(encoding);
        }
    }
}
