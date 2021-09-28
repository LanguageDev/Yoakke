// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.BnfAst
{
    /// <summary>
    /// Factory methods for building BNF nodes.
    /// </summary>
    public static class Bnf
    {
        /// <summary>
        /// Constructs a terminal BNF node.
        /// </summary>
        /// <param name="value">The object representing the terminal.</param>
        /// <returns>The terminal BNF nodes constructed.</returns>
        public static IBnfNode Term(object value) => new BnfTermNode(value);

        /// <summary>
        /// Constructs an alternative of BNF nodes.
        /// </summary>
        /// <param name="first">The first BNF node in the alternative.</param>
        /// <param name="rest">The remaining BNF nodes.</param>
        /// <returns>The alternatives of BNF nodes constructed.</returns>
        public static IBnfNode Or(IBnfNode first, params IBnfNode[] rest) => Or(first, rest as IEnumerable<IBnfNode>);

        /// <summary>
        /// Constructs a alternative of BNF nodes.
        /// </summary>
        /// <param name="first">The first BNF node in the alternative.</param>
        /// <param name="rest">The remaining BNF nodes.</param>
        /// <returns>The alternatives of BNF nodes constructed.</returns>
        public static IBnfNode Or(IBnfNode first, IEnumerable<IBnfNode> rest)
        {
            var result = first;
            foreach (var right in rest) result = new BnfOrNode(result, right);
            return result;
        }

        /// <summary>
        /// Constructs a sequence of BNF nodes.
        /// </summary>
        /// <param name="first">The first BNF node in the sequence.</param>
        /// <param name="rest">The remaining BNF nodes.</param>
        /// <returns>The sequence of BNF nodes constructed.</returns>
        public static IBnfNode Seq(IBnfNode first, params IBnfNode[] rest) => Seq(first, rest as IEnumerable<IBnfNode>);

        /// <summary>
        /// Constructs a sequence of BNF nodes.
        /// </summary>
        /// <param name="first">The first BNF node in the sequence.</param>
        /// <param name="rest">The remaining BNF nodes.</param>
        /// <returns>The sequence of BNF nodes constructed.</returns>
        public static IBnfNode Seq(IBnfNode first, IEnumerable<IBnfNode> rest)
        {
            var result = first;
            foreach (var right in rest) result = new BnfSeqNode(result, right);
            return result;
        }

        /// <summary>
        /// Construct a transformer BNF node.
        /// </summary>
        /// <param name="element">The BNF node to transform.</param>
        /// <param name="transformer">The transformer to use.</param>
        /// <returns>The constructed transform node.</returns>
        public static IBnfNode Transform(IBnfNode element, object transformer) => new BnfTransformNode(element, transformer);

        /// <summary>
        /// Construct a left-fold BNF node.
        /// </summary>
        /// <param name="first">The initial node of the fold.</param>
        /// <param name="second">The repeated, folded node of the fold.</param>
        /// <param name="transformer">The transformer to use.</param>
        /// <returns>The constructed transform node.</returns>
        public static IBnfNode Foldl(IBnfNode first, IBnfNode second, object transformer) =>
            new BnfFoldlNode(first, second, transformer);

        /// <summary>
        /// Constructs an optional BNF node.
        /// </summary>
        /// <param name="element">The element to wrap as optional.</param>
        /// <returns>The constructed optional BNF node.</returns>
        public static IBnfNode Opt(IBnfNode element) => new BnfOptNode(element);

        /// <summary>
        /// Constructs a 0-or-more repetition BNF node.
        /// </summary>
        /// <param name="element">The element to wrap as repeated.</param>
        /// <returns>The constructed repeated BNF node.</returns>
        public static IBnfNode Rep0(IBnfNode element) => new BnfRep0Node(element);

        /// <summary>
        /// Constructs a 1-or-more repetition BNF node.
        /// </summary>
        /// <param name="element">The element to wrap as repeated.</param>
        /// <returns>The constructed repeated BNF node.</returns>
        public static IBnfNode Rep1(IBnfNode element) => new BnfRep1Node(element);

        /// <summary>
        /// Constructs a grouping BNF node.
        /// </summary>
        /// <param name="element">The element to wrap as grouped.</param>
        /// <returns>The constructed grouping BNF node.</returns>
        public static IBnfNode Group(IBnfNode element) => new BnfGroupNode(element);
    }
}
