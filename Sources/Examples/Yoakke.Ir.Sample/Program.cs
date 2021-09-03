using System;
using Yoakke.Ir.Model;
using Yoakke.Ir.Model.Builders;
using Yoakke.Ir.Syntax;

namespace Yoakke.Ir.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var asm = new AssemblyBuilder();
            var proc = new ProcedureBuilder("main");
            var bb = new BasicBlockBuilder("entry");

            asm.WithProcedure(proc);
            proc.WithBasicBlock(bb);
        }
    }
}
