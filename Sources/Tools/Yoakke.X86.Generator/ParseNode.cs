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
        /// Adds an <see cref="Encoding"/> to this tree.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to add.</param>
        public void AddEncoding(Encoding encoding) => AddEncoding(this, encoding, encoding.Opcodes);

        private static void AddEncoding(ParseNode root, Encoding encoding, IReadOnlyList<Opcode> opcodes)
        {
            if (opcodes.Count == 0)
            {
                // Base-case
                root.Encodings.Add(encoding);
                return;
            }

            // Recursive case
            var opcode = opcodes[0];
            var nextOpcodes = opcodes.Skip(1).ToList();

            for (var i = 0; i < (opcode.Last3BitsEncodedOperand is null ? 1 : 8); ++i)
            {
                var code = (byte)(opcode.Code + i);
                if (!root.Subnodes.TryGetValue(code, out var nextRoot))
                {
                    nextRoot = new ParseNode();
                    root.Subnodes.Add(code, nextRoot);
                }
                AddEncoding(nextRoot, encoding, nextOpcodes);
            }
        }
    }
}
