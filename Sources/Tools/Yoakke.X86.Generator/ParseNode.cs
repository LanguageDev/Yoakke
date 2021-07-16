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
        /// The parent node of this one.
        /// </summary>
        public ParseNode? Parent { get; }

        /// <summary>
        /// The <see cref="MatchType"/> to get to this node.
        /// </summary>
        public MatchType Type { get; }

        /// <summary>
        /// The subnodes when we match a certain byte.
        /// </summary>
        public IDictionary<byte, ParseNode> Subnodes { get; } = new SortedDictionary<byte, ParseNode>();

        /// <summary>
        /// The encodings for this node.
        /// </summary>
        public IList<Encoding> Encodings { get; } = new List<Encoding>();

        /// <summary>
        /// Adds an <see cref="Encoding"/> to this tree.
        /// </summary>
        /// <param name="encoding">The <see cref="Encoding"/> to add.</param>
        public void AddEncoding(Encoding encoding) => AddEncoding(this, encoding, encoding.Opcodes);

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseNode"/> class.
        /// </summary>
        /// <param name="type">The <see cref="MatchType"/> for this node.</param>
        /// <param name="parent">The parent node of this one.</param>
        public ParseNode(MatchType type, ParseNode? parent = null)
        {
            this.Type = type;
            this.Parent = parent;
        }

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
                    nextRoot = new ParseNode(MatchType.Opcode, root);
                    root.Subnodes.Add(code, nextRoot);
                }
                AddEncoding(nextRoot, encoding, nextOpcodes);
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
                    nextRoot = new ParseNode(MatchType.ModRmReg, root);
                    root.Subnodes.Add(bits, nextRoot);
                }

                Debug.Assert(nextRoot.Type == MatchType.ModRmReg, "Can't mix opcode with prefix matching");
                root = nextRoot;
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
                    nextRoot = new ParseNode(MatchType.Prefix, root);
                    root.Subnodes.Add(prefix.Code, nextRoot);
                }

                Debug.Assert(nextRoot.Type == MatchType.Prefix, "Can't mix opcode with prefix matching");
                root = nextRoot;
            }

            // Add the encoding
            root.Encodings.Add(encoding);
        }
    }
}
