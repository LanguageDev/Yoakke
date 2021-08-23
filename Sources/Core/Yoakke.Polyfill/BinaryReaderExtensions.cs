// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace System.IO
{
    /// <summary>
    /// Extensions for <see cref="BinaryReader"/>.
    /// </summary>
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream
        /// by the number of bytes read.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read from.</param>
        /// <param name="buffer">A region of memory. When this method returns, the contents of this region are
        /// replaced by the bytes read from the current source.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes
        /// allocated in the buffer if that many bytes are not currently available, or zero (0) if the end of the
        /// stream has been reached.</returns>
        public static int Read(this BinaryReader reader, Span<byte> buffer)
        {
            // TODO: Very inefficient, but had to jump hoops because of .NET standard 2
            var bytes = new byte[buffer.Length];
            var result = reader.Read(bytes, 0, buffer.Length);
            bytes.CopyTo(buffer);
            return result;
        }
    }
}
