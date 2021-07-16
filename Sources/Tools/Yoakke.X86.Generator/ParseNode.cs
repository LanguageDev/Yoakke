// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        public IDictionary<byte, (MatchType, ParseNode)> Subnodes { get; } = new Dictionary<byte, (MatchType, ParseNode)>();

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
                // Base-case, we deal with prefixes
                AddModRmEncoding(root, encoding);
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
                    nextRoot = (MatchType.Opcode, new ParseNode());
                    root.Subnodes.Add(code, nextRoot);
                }
                AddEncoding(nextRoot.Item2, encoding, nextOpcodes);
            }
        }

        private static void AddModRmEncoding(ParseNode root, Encoding encoding)
        {
            if (encoding.ModRM is not null && !encoding.ModRM.Reg.StartsWith('#'))
            {
                // The ModRM byte contains 3 bits as an opcode extension
                // Inject a match node here
                var bits = byte.Parse(encoding.ModRM.Reg, NumberStyles.HexNumber);
                if (!root.Subnodes.TryGetValue(bits, out var nextRoot))
                {
                    nextRoot = (MatchType.ModRmReg, new ParseNode());
                    root.Subnodes.Add(bits, nextRoot);
                }

                Debug.Assert(nextRoot.Item1 == MatchType.ModRmReg, "Can't mix opcode with prefix matching");
                root = nextRoot.Item2;
            }

            AddPrefixEncoding(root, encoding);
        }

        private static void AddPrefixEncoding(ParseNode root, Encoding encoding)
        {
            // To save on branches, we order the prefix checks
            var prefixes = encoding.Prefixes.OrderBy(p => p.Code).ToList();

            // Add each prefix
            foreach (var prefix in prefixes)
            {
                if (!root.Subnodes.TryGetValue(prefix.Code, out var nextRoot))
                {
                    nextRoot = (MatchType.Prefix, new ParseNode());
                    root.Subnodes.Add(prefix.Code, nextRoot);
                }

                Debug.Assert(nextRoot.Item1 == MatchType.Prefix, "Can't mix opcode with prefix matching");
                root = nextRoot.Item2;
            }

            // Add the encoding
            root.Encodings.Add(encoding);
        }
    }
}
