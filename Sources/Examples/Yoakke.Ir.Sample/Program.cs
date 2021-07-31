using System;
using Yoakke.Ir.Writers;

namespace Yoakke.Ir.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new AssemblyBuilder()
                .DefineProcedure("main", out var main)
                .DefineParameter(new Type.Int(true, 32), out var arg)
                .Ret(arg);
            main.Return = new Type.Int(true, 32);

            var writer = new AssemblyTextWriter();
            writer.Write(builder.Assembly);
            Console.WriteLine(writer.Result);
        }
    }
}
