using System;
using System.IO;
using System.Xml;

namespace Yoakke.Ast
{
    /// <summary>
    /// Utility for pretty-printing ASTs for debug.
    /// </summary>
    public static class PrettyPrinter
    {
        /// <summary>
        /// The settings to use for formatting XML.
        /// </summary>
        public static readonly XmlWriterSettings XmlSettings = new XmlWriterSettings
        {
            Indent = true,
        };

        /// <summary>
        /// Writes out the given AST node as some text format.
        /// </summary>
        /// <param name="node">The node to write.</param>
        /// <param name="format">The format to write the node in.</param>
        /// <returns>The string representing the AST node in the given format.</returns>
        public static string Print(IAstNode node, PrettyPrintFormat format)
        {
            var writer = new StringWriter();
            Print(node, writer, format);
            return writer.ToString();
        }

        /// <summary>
        /// Writes out the given AST node as some text format.
        /// </summary>
        /// <param name="node">The node to write.</param>
        /// <param name="writer">The writer to write results to.</param>
        /// <param name="format">The format to write the node in.</param>
        public static void Print(IAstNode node, TextWriter writer, PrettyPrintFormat format)
        {
            switch (format)
            {
            case PrettyPrintFormat.Xml:
                PrintXml(node, writer);
                break;

            default:
                throw new ArgumentException("unknown pretty-print format", nameof(format));
            }
        }

        private static void PrintXml(IAstNode node, TextWriter writer)
        {
            var xmlWriter = XmlWriter.Create(writer, XmlSettings);
            WriteXml(node, xmlWriter);
            xmlWriter.Flush();
        }

        private static void WriteXml(IAstNode node, XmlWriter writer)
        {
            if (node == null) return;
            writer.WriteStartElement(node.GetType().Name);
            // Leaves are attributes
            foreach (var (name, value) in node.LeafObjects) writer.WriteAttributeString(name, value.ToString());
            // Leaf collections are subnodes
            foreach (var (name, coll) in node.LeafObjectCollections)
            {
                writer.WriteStartElement(name);
                foreach (var item in coll)
                {
                    writer.WriteStartElement(item.ToString()!);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            // Children are subelements
            foreach (var (name, child) in node.ChildNodes)
            {
                writer.WriteStartElement(name);
                WriteXml(child, writer);
                writer.WriteEndElement();
            }
            // Child lists are subelements of the list subelement
            foreach (var (name, coll) in node.ChildNodeCollections)
            {
                writer.WriteStartElement(name);
                foreach (var item in coll) WriteXml(item, writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
