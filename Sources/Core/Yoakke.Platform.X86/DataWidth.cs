namespace Yoakke.Platform.X86
{
    /// <summary>
    /// The possible widths of the x86 operands, reads or writes.
    /// </summary>
    public enum DataWidth
    {
        /// <summary>
        /// A single byte, or 8 bits.
        /// </summary>
        Byte = 1,

        /// <summary>
        /// 2 bytes, or 16 bits.
        /// </summary>
        Word = 2,

        /// <summary>
        /// 4 bytes, or 32 bits.
        /// </summary>
        Dword = 4,

        /// <summary>
        /// 8 bytes, or 64 bits.
        /// </summary>
        Qword = 8,
    }
}
