using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Text
{
    /// <summary>
    /// Represents a range in some source file.
    /// </summary>
    public readonly struct Location
    {
        /// <summary>
        /// The path of the file.
        /// </summary>
        public readonly string Path;
        /// <summary>
        /// The range.
        /// </summary>
        public readonly Range Range;

        /// <summary>
        /// Initializes a new <see cref="Location"/>.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="range">The range in the file.</param>
        public Location(string path, Range range)
        {
            Path = path;
            Range = range;
        }
    }
}
