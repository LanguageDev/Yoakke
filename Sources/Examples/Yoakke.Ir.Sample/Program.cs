using System;
using Yoakke.Ir.Passes;
using Yoakke.Ir.Writers;
using Type = Yoakke.Ir.Model.Type;

namespace Yoakke.Ir.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var i32 = new Type.Int(true, 32);

            var builder = new AssemblyBuilder()
                .DefineProcedure("main", out var main)
                .DefineLocal(i32)
                .DefineParameter(i32, out var arg0)
                .DefineParameter(i32, out var arg1)
                .IntAdd(arg0, arg1, out var added)
                .Ret(added);
            main.Return = i32;

            var pass = new RemoveUnreferencedLocals();
            foreach (var proc in builder.Assembly.Procedures.Values) pass.Pass(proc);
        
            var writer = new AssemblyTextWriter();
            writer.Write(builder.Assembly);
            Console.WriteLine(writer.Result);
        }
    }
}
