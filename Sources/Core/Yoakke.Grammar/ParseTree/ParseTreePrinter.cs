// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Text;
using Yoakke.Collections.Graphs;

namespace Yoakke.Grammar.ParseTree
{
    /// <summary>
    /// Printer utility for the parse tree.
    /// </summary>
    public static class ParseTreePrinter
    {
        /// <summary>
        /// Converts a parse-tree node into graphviz DOT format.
        /// </summary>
        /// <param name="node">The node to convert.</param>
        /// <returns>The DOT format of <paramref name="node"/>.</returns>
        public static string ToDot(this IParseTreeNode node)
        {
            var result = new StringBuilder();
            result.AppendLine("graph parse_tree {");

            // We assign each node an ID
            var nodeIds = new Dictionary<IParseTreeNode, int>();
            foreach (var n in BreadthFirst.Search(node, n => n.Children)) nodeIds.Add(n, nodeIds.Count);

            // Define each node with the label
            foreach (var (n, id) in nodeIds)
            {
                result.Append($"  {id}[label=\"");

                if (n is LeafParseTreeNode leaf) result.Append($"{n.Symbol}[{leaf.Terminal}]");
                else result.Append(n.Symbol);

                result.AppendLine("\"];");
            }

            // Connect parent-child relations
            foreach (var (n, id) in nodeIds)
            {
                foreach (var other in n.Children) result.AppendLine($"  {id} -- {nodeIds[other]}");
            }

            result.Append('}');
            return result.ToString();
        }
    }
}
