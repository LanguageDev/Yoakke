using System;

namespace Yoakke.Ir.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new AssemblyBuilder()
                .DefineProcedure("main")
                .Ret();
        }
    }
}
