using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Ast
{
    /// <summary>
    /// Utility class for pretty-printers.
    /// </summary>
    public class PrettyPrinterUtil
    {
        /// <summary>
        /// The string builder that writes the results.
        /// </summary>
        public readonly StringBuilder Builder = new StringBuilder();
        /// <summary>
        /// The width of the indentation to use.
        /// </summary>
        public int IndentationWidth { get; set; } = 2;
        /// <summary>
        /// The current indentation.
        /// </summary>
        public int Indentation { get; set; } = 0;

        /// <summary>
        /// Writes indentation spaces to the result.
        /// </summary>
        /// <returns>This, so calls can be chained.</returns>
        public PrettyPrinterUtil WriteIndentation()
        {
            Builder.Append(' ', Indentation * Indentation);
            return this;
        }

        /// <summary>
        /// Indents, calls an action, then unindents.
        /// </summary>
        /// <param name="action">The action to call already indented.</param>
        /// <returns>This, so calls can be chained.</returns>
        public PrettyPrinterUtil DoIndented(Action<PrettyPrinterUtil> action)
        {
            Indentation += 1;
            action(this);
            Indentation -= 1;
            return this;
        }

        // TODO: Doc
        public PrettyPrinterUtil DoInXmlSubnode(
            string tag, 
            IEnumerable<KeyValuePair<string, string>> attributes,
            Action<PrettyPrinterUtil> action)
        {
            WriteIndentation();
            Builder.AppendLine($"<{tag}{string.Join(string.Empty, attributes.Select(a => $" {a.Key}=\"{a.Value}\""))}>");
            DoIndented(action);
            WriteIndentation();
            Builder.AppendLine($"</{tag}>");
            return this;
        }
    }
}
