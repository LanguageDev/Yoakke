using System.IO;

namespace Yoakke.Text
{
    /// <summary>
    /// Represents a source file.
    /// </summary>
    public interface ISourceFile
    {
        /// <summary>
        /// Retrieves a path that uniquely identifies this source file.
        /// </summary>
        public string Path { get; }
        /// <summary>
        /// Retrieves the available amount of lines in the source file.
        /// </summary>
        public int LineCount { get; }
        /// <summary>
        /// The reader for the contents of the file.
        /// </summary>
        public TextReader Reader { get; }
        /// <summary>
        /// Retrieves a given line from the source file.
        /// </summary>
        /// <param name="index">The index of the line to retrieve.</param>
        /// <returns>The line with the given index.</returns>
        public string GetLine(int index);
    }
}
