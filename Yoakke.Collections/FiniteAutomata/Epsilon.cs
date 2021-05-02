using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections.FiniteAutomata
{
    /// <summary>
    /// A type to represent the epsilon symbol for epsilon tranisitions.
    /// </summary>
    public struct Epsilon
    {
        public static Epsilon Default = new Epsilon();
    }
}
