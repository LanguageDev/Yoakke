using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.X86.Operands
{
    /// <summary>
    /// The possible widths of the x86 operands, reads or writes.
    /// </summary>
    public enum DataWidth
    {
        Byte = 1,
        Word = 2,
        Dword = 4,
        Qword = 8,
    }
}
